using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class MBattleManager : MonoBehaviourPunCallbacks
{
    PhotonView view;

    [System.Serializable]
    public class SelectedHero
    {
        public int Id;
        public string Name;
    }
    [System.Serializable]
    public class SelectedHeroList
    {
        public List<SelectedHero> SelectedHeroesList;
    }
    #region Base Game Variables

    private MGlobal.BattlePhase battlePhase = MGlobal.BattlePhase.BattleStart;

    public int currentPlayer1Character = 0;
    public int[] teamPlayer1Choices;
    public int[] teamPlayer1SkillChoices;
    public int[] teamPlayer1AllyTargets;
    public int[] teamPlayer1EnemyTargets;

    private int currentPlayer2Character = 0;
    public int[] teamPlayer2Choices;
    public int[] teamPlayer2SkillChoices;
    public int[] teamPlayer2AllyTargets;
    public int[] teamPlayer2EnemyTargets;
    private bool waitingToNextExecution = false;

    /// <summary>
    /// Ezt nézd meg
    /// </summary>
    public MCharacter[] characters;
    private string[] teams;
    public int[] choices;
    public int[] skillChoices;
    public int[] allyTargets;
    public int[] enemyTargets;
    /// <summary>
    /// 
    /// </summary>
    /// 
    #endregion
    #region Multiplayer Variables
    public SelectedHeroList selectedHeroList = new SelectedHeroList();

    public List<GameObject> PlayerHeroes;

    public List<MCharacter> playerCharactersToInit;
    public List<MCharacter> player1Characters;
    public List<MCharacter> player2Characters;

    public MBattleManagerUI battleManagerUI;
    public MBattleManager battleManager;

    //Ready Logic
    public bool isAllPlayerReady = false;
    public bool isEverythingInitialized = false;
    public bool player1Selected = false;
    public bool player2Selected = false;
    public bool everyoneSelected = false;

    #endregion
    void Start()
    {
        view = GetComponent<PhotonView>();
        InitHeroesToTheScene();
        InitChoices();
    }

    void Update()
    {
        if (isEverythingInitialized)
        {
            // Battle Ends
            if (battlePhase == MGlobal.BattlePhase.BattleEnds)
            {
                print("Battle Ends!");
                return;
            }
            
            // Team Player1 Selection
            if (battlePhase == MGlobal.BattlePhase.TeamPlayer1Selection)
            {
                if (teamPlayer1Choices[currentPlayer1Character] != 0 || player1Characters[currentPlayer1Character].state == MGlobal.CharacterState.Dead)
                {
                    if (player1Characters.Count > (currentPlayer1Character + 1))
                    {
                        currentPlayer1Character++;
                    }
                    else
                    {
                        setPlayer1Choices();
                        setPlayer1Selection();
                        currentPlayer1Character = 0;
                        currentPlayer2Character = 0;
                    }
                }
            }

            // Team Player2 Selection
            if (battlePhase == MGlobal.BattlePhase.TeamPlayer2Selection)
            {
                //print("The enemy is selecting his action!");
                if (teamPlayer2Choices[currentPlayer2Character] != 0 || player2Characters[currentPlayer2Character].state == MGlobal.CharacterState.Dead)
                {
                    if (player2Characters.Count > (currentPlayer2Character + 1))
                    {
                        currentPlayer2Character++;
                    }
                    else
                    {
                        setPlayer2Choices();
                        setPlayer2Selection();
                        GetAllProperties();
                        currentPlayer1Character = 0;
                        currentPlayer2Character = 0;
                    }
                }
                //battlePhase = MGlobal.BattlePhase.ExecuteActions;
                OrderCharactersBySpeed();
            }



            // Execute Actions
            if (battlePhase == MGlobal.BattlePhase.ExecuteActions && player1Selected && player2Selected)
            {
                //Debug.Log("ez megy");

                if (!waitingToNextExecution)
                {
                    if (characters[currentPlayer1Character].state != MGlobal.CharacterState.Dead)
                    {
                        if (choices[currentPlayer1Character] == 1)
                        {
                            // Attack
                            MCharacter currentCharacterTarget = ((teams[currentPlayer1Character] != "P1") ? player1Characters[enemyTargets[currentPlayer1Character]] : player2Characters[enemyTargets[currentPlayer1Character]]);

                            if (currentCharacterTarget.state == MGlobal.CharacterState.Dead)
                                currentCharacterTarget = GetNextCharacterOfTeam(((teams[currentPlayer1Character] != "P1") ? "P1" : "P2"));

                            if (battlePhase != MGlobal.BattlePhase.BattleEnds)
                                characters[currentPlayer1Character].Attack(
                                currentCharacterTarget
                            );

                            waitingToNextExecution = true;

                            battleManagerUI.DisplayMessage(characters[currentPlayer1Character].name + " attacks!");
                        }
                        else if (choices[currentPlayer1Character] == 2)
                        {
                            //    // Use skill
                            //    if (characters[currentPlayer1Character].skills[skillChoices[currentPlayer1Character]].targets == MGlobal.Targets.Enemy)
                            //    {
                            //        MCharacter currentCharacterTarget = ((teams[currentPlayer1Character] != "P1") ? player1Characters[enemyTargets[currentPlayer1Character]] : player2Characters[enemyTargets[currentPlayer1Character]]);

                            //        if (currentCharacterTarget.state == MGlobal.CharacterState.Dead)
                            //            currentCharacterTarget = GetNextCharacterOfTeam(((teams[currentPlayerCharacter] != "A") ? "A" : "Enemy"));

                            //        characters[currentPlayerCharacter].UseSkill(skillChoices[currentPlayerCharacter], currentCharacterTarget);
                            //    }
                            //    else
                            //    {
                            //        characters[currentPlayerCharacter].UseSkill(skillChoices[currentPlayerCharacter], teamPlayer[allyTargets[currentPlayerCharacter]]);
                            //    }

                            //    waitingToNextExecution = true;

                            //    battleGUIManager.DisplayMessage(characters[currentPlayerCharacter].name + " use " + characters[currentPlayerCharacter].skills[skillChoices[currentPlayerCharacter]].name + "!");
                            //}
                            Debug.Log("skill");
                        }
                    }
                    if (characters[currentPlayer2Character].state != MGlobal.CharacterState.Dead)
                    {
                        if (choices[currentPlayer2Character] == 1)
                        {
                            // Attack
                            MCharacter currentCharacterTarget = ((teams[currentPlayer2Character] != "P2") ? player2Characters[enemyTargets[currentPlayer2Character]] : player2Characters[enemyTargets[currentPlayer2Character]]);

                            if (currentCharacterTarget.state == MGlobal.CharacterState.Dead)
                                currentCharacterTarget = GetNextCharacterOfTeam(((teams[currentPlayer2Character] != "P2") ? "P2" : "P1"));

                            if (battlePhase != MGlobal.BattlePhase.BattleEnds)
                                characters[currentPlayer2Character].Attack(
                                currentCharacterTarget
                            );

                            waitingToNextExecution = true;

                            battleManagerUI.DisplayMessage(characters[currentPlayer1Character].name + " attacks!");
                        }
                        else if (choices[currentPlayer1Character] == 2)
                        {
                            //    // Use skill
                            //    if (characters[currentPlayer1Character].skills[skillChoices[currentPlayer1Character]].targets == MGlobal.Targets.Enemy)
                            //    {
                            //        MCharacter currentCharacterTarget = ((teams[currentPlayer1Character] != "P1") ? player1Characters[enemyTargets[currentPlayer1Character]] : player2Characters[enemyTargets[currentPlayer1Character]]);

                            //        if (currentCharacterTarget.state == MGlobal.CharacterState.Dead)
                            //            currentCharacterTarget = GetNextCharacterOfTeam(((teams[currentPlayerCharacter] != "A") ? "A" : "Enemy"));

                            //        characters[currentPlayerCharacter].UseSkill(skillChoices[currentPlayerCharacter], currentCharacterTarget);
                            //    }
                            //    else
                            //    {
                            //        characters[currentPlayerCharacter].UseSkill(skillChoices[currentPlayerCharacter], teamPlayer[allyTargets[currentPlayerCharacter]]);
                            //    }

                            //    waitingToNextExecution = true;

                            //    battleGUIManager.DisplayMessage(characters[currentPlayerCharacter].name + " use " + characters[currentPlayerCharacter].skills[skillChoices[currentPlayerCharacter]].name + "!");
                            //}
                            Debug.Log("skill");
                        }
                    }
                }

                if ((!characters[currentPlayer1Character].IsAttacking() && !characters[currentPlayer1Character].IsUsingSkill() && !characters[currentPlayer1Character].IsMovingToInitialPosition()) && (!characters[currentPlayer2Character].IsAttacking() && !characters[currentPlayer2Character].IsUsingSkill() && !characters[currentPlayer2Character].IsMovingToInitialPosition()))
                {
                    waitingToNextExecution = false;

                    if (isPlayerHaveAliveCharacter("P1") && isPlayerHaveAliveCharacter("P2"))
                    {
                        if (characters.Length > (currentPlayer1Character + 1))
                        {
                            currentPlayer1Character++;
                        }
                        if (characters.Length > (currentPlayer2Character + 1))
                        {
                            currentPlayer2Character++;
                        }
                        else
                        {
                            InitChoices();
                            battlePhase = MGlobal.BattlePhase.TeamPlayer1Selection;
                            player1Selected = false;
                            player2Selected = false;
                        }
                    }
                    else
                    {
                        battleManagerUI.DisplayMessage(((isPlayerHaveAliveCharacter("A")) ? "You win!" : "You lost!"), -1.0f, false);
                        battlePhase = MGlobal.BattlePhase.BattleEnds;
                    }
                }
            }
        }
    }

    public void setPlayer1Selection()
    {
        view.RPC("setPlayer1SelectionRPC", RpcTarget.All);
    }

    [PunRPC]
    void setPlayer1SelectionRPC()
    {
        battlePhase = MGlobal.BattlePhase.TeamPlayer2Selection;
        player1Selected = true;
    }

    public void setPlayer2Selection()
    {
        view.RPC("setPlayer2SelectionRPC", RpcTarget.All);
    }

    [PunRPC]
    void setPlayer2SelectionRPC()
    {
        player2Selected = true;
        battlePhase = MGlobal.BattlePhase.ExecuteActions;
    }

    public void setExecutAction()
    {
        view.RPC("setExecutActionRPC", RpcTarget.All);
    }

    [PunRPC]
    void setExecutActionRPC()
    {
        battlePhase = MGlobal.BattlePhase.ExecuteActions;
    }
    public void setPlayer1Choices()
    {
        view.RPC("setPlayer1ChoicesRPC", RpcTarget.All);
    }

    [PunRPC]
    void setPlayer1ChoicesRPC()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
            properties.Add("currentPlayer1Character", currentPlayer1Character);
            properties.Add("teamPlayer1Choices", teamPlayer1Choices);
            properties.Add("teamPlayer1SkillChoices", teamPlayer1SkillChoices);
            properties.Add("teamPlayer1AllyTargets", teamPlayer1AllyTargets);
            properties.Add("teamPlayer1EnemyTargets", teamPlayer1EnemyTargets);
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }
        Debug.Log("Player1 Choice registered");
        SyncPlayer1Select();
        Debug.Log("Sync Select");
    }
    
    public void setPlayer2Choices()
    {
        view.RPC("setPlayer2ChoicesRPC", RpcTarget.All);
    }

    [PunRPC]
    void setPlayer2ChoicesRPC()
    {
        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        //properties.Add("currentPlayer2Character", currentPlayer2Character);
        if (PhotonNetwork.IsMasterClient)
        {
            properties.Add("teamPlayer2Choices", teamPlayer2Choices);
            properties.Add("teamPlayer2SkillChoices", teamPlayer2SkillChoices);
            properties.Add("teamPlayer2AllyTargets", teamPlayer2AllyTargets);
            properties.Add("teamPlayer2EnemyTargets", teamPlayer2EnemyTargets);
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }
        Debug.Log("Player2 Choice registered");
        //view.RPC("SyncPlayer2SelectRPC", RpcTarget.All);
        SyncPlayer2Select();
        Debug.Log("Sync Select");
    }

    public void SyncPlayer1Select()
    {
        view.RPC("SyncPlayer1SelectRPC", RpcTarget.All);
    }

    [PunRPC]
    public void SyncPlayer1SelectRPC()
    {
        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        //currentPlayer1Character = properties["currentPlayer1Character"];
        teamPlayer1Choices = (int[])properties["teamPlayer1Choices"];
        teamPlayer1SkillChoices = (int[])properties["teamPlayer1SkillChoices"];
        teamPlayer1AllyTargets = (int[])properties["teamPlayer1AllyTargets"];
        teamPlayer1EnemyTargets = (int[])properties["teamPlayer1EnemyTargets"];
        Debug.Log("SyncPlayer1SelectRPC");
    } 
    
    public void SyncPlayer2Select()
    {
        view.RPC("SyncPlayer2SelectRPC", RpcTarget.All);
    }

    [PunRPC]
    public void SyncPlayer2SelectRPC()
    {
        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        //currentPlayer2Character = properties["currentPlayer2Character"];
        teamPlayer2Choices = (int[])properties["teamPlayer2Choices"];
        teamPlayer2SkillChoices = (int[])properties["teamPlayer2SkillChoices"];
        teamPlayer2AllyTargets = (int[])properties["teamPlayer2AllyTargets"];
        teamPlayer2EnemyTargets = (int[])properties["teamPlayer2EnemyTargets"];
        Debug.Log("SyncPlayer2SelectRPC");
    }

    public void GetAllProperties()
    {
        view.RPC("GetAllPropertiesRPC", RpcTarget.All);
    }

    [PunRPC]
    public void GetAllPropertiesRPC()
    {
        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        //currentPlayer2Character = properties["currentPlayer2Character"];
        teamPlayer1Choices = (int[])properties["teamPlayer1Choices"];
        teamPlayer1SkillChoices = (int[])properties["teamPlayer1SkillChoices"];
        teamPlayer1AllyTargets = (int[])properties["teamPlayer1AllyTargets"];
        teamPlayer1EnemyTargets = (int[])properties["teamPlayer1EnemyTargets"];
        teamPlayer2Choices = (int[])properties["teamPlayer2Choices"];
        teamPlayer2SkillChoices = (int[])properties["teamPlayer2SkillChoices"];
        teamPlayer2AllyTargets = (int[])properties["teamPlayer2AllyTargets"];
        teamPlayer2EnemyTargets = (int[])properties["teamPlayer2EnemyTargets"];

        Debug.Log("Sync for Everybody");
    }
    void OrderCharactersBySpeed()
    {
        //Group of characters, teams, choices, actions, targets.
        characters = new MCharacter[(player1Characters.Count + player2Characters.Count)];
        teams = new string[(         player1Characters.Count + player2Characters.Count)];
        choices = new int[(          player1Characters.Count + player2Characters.Count)];
        skillChoices = new int[(     player1Characters.Count + player2Characters.Count)];
        allyTargets = new int[(      player1Characters.Count + player2Characters.Count)];
        enemyTargets = new int[(     player1Characters.Count + player2Characters.Count)];

        int[] usedPositions = new int[(player1Characters.Count + player2Characters.Count)];

        for (int i = 0; i < characters.Length; i++)
        {
            MCharacter currentFastestCharacter = null;
            string currentFastestCharacterTeam = null;
            int currentFastestCharacterPosition = 0;

            for (int j = 0; j < player1Characters.Count; j++)
            {
                if (!InArray(usedPositions, teams, j, "P1"))
                {
                    if (currentFastestCharacter != null)
                    {
                        if (player1Characters[j].actionSpeed > currentFastestCharacter.actionSpeed)
                        {
                            currentFastestCharacter = player1Characters[j];
                            currentFastestCharacterTeam = "P1";
                            currentFastestCharacterPosition = j;
                        }
                    }
                    else
                    {
                        currentFastestCharacter = player1Characters[j];
                        currentFastestCharacterTeam = "P1";
                        currentFastestCharacterPosition = j;
                    }
                }
            }

            for (int j = 0; j < player2Characters.Count; j++)
            {
                if (!InArray(usedPositions, teams, j, "P2"))
                {
                    if (currentFastestCharacter != null)
                    {
                        if (player2Characters[j].actionSpeed > currentFastestCharacter.actionSpeed)
                        {
                            currentFastestCharacter = player2Characters[j];
                            currentFastestCharacterTeam = "P2";
                            currentFastestCharacterPosition = j;
                        }
                    }
                    else
                    {
                        currentFastestCharacter = player2Characters[j];
                        currentFastestCharacterTeam = "P2";
                        currentFastestCharacterPosition = j;
                    }
                }
            }

            characters[i] = currentFastestCharacter;
            teams[i] = currentFastestCharacterTeam;
            choices[i] = ((currentFastestCharacterTeam == "P1") ? teamPlayer1Choices[currentFastestCharacterPosition] : teamPlayer2Choices[currentFastestCharacterPosition]);
            skillChoices[i] = ((currentFastestCharacterTeam == "P1") ? teamPlayer1SkillChoices[currentFastestCharacterPosition] : teamPlayer2SkillChoices[currentFastestCharacterPosition]);
            enemyTargets[i] = ((currentFastestCharacterTeam == "P1") ? teamPlayer1EnemyTargets[currentFastestCharacterPosition] : teamPlayer2EnemyTargets[currentFastestCharacterPosition]);
            allyTargets[i] = ((currentFastestCharacterTeam == "P1") ? teamPlayer1AllyTargets[currentFastestCharacterPosition] : teamPlayer2AllyTargets[currentFastestCharacterPosition]);
            usedPositions[i] = currentFastestCharacterPosition;
        }
    }

    bool InArray(int[] positionsArray, string[] teamsArray, int position, string team)
    {
        for (int i = 0; i < positionsArray.Length; i++)
        {
            if (teamsArray[i] == team)
            {
                if (positionsArray[i] == position)
                    return true;
            }
        }

        return false;
    }

    public MGlobal.BattlePhase GetPhase()
    {
        return battlePhase;
    }

    public MCharacter GetCurrentCharacter(string player)
    {
        if (player == "P1")
            return player1Characters[currentPlayer1Character];
        else {
            return player2Characters[currentPlayer2Character];
        }
    }

    public int GetCurrentCharacterInArrayPosition(string player)
    {
        if (player == "P1")
            return currentPlayer1Character;
        else
        {
            return currentPlayer2Character;
        }
        
    }

    public void SetPlayer1Choice(int choice, int skill, int target, bool isEnemyTarget)
    {
        teamPlayer1Choices[currentPlayer1Character] = choice;

        if (choice == 2)
            teamPlayer1SkillChoices[currentPlayer1Character] = skill;

        if (isEnemyTarget)
            teamPlayer1EnemyTargets[currentPlayer1Character] = target;
        else
            teamPlayer1AllyTargets[currentPlayer1Character] = target;
    }

    public void SetPlayer2Choice(int choice, int skill, int target, bool isEnemyTarget)
    {
        teamPlayer2Choices[currentPlayer2Character] = choice;

        if (choice == 2)
            teamPlayer2SkillChoices[currentPlayer2Character] = skill;

        if (isEnemyTarget)
            teamPlayer2EnemyTargets[currentPlayer2Character] = target;
        else
            teamPlayer2AllyTargets[currentPlayer2Character] = target;
    }

    MCharacter GetNextCharacterOfTeam(string team)
    {
        List<MCharacter> currentTeam = ((team == "P1") ? player1Characters : player2Characters);

        for (int i = 0; i < currentTeam.Count; i++)
        {
            if (currentTeam[i].state != MGlobal.CharacterState.Dead)
            {
                return currentTeam[i];
            }
        }

        battlePhase = MGlobal.BattlePhase.BattleEnds;

        return null;
    }

    bool isPlayerHaveAliveCharacter(string team)
    {
        List<MCharacter> currentTeam = ((team == "A") ? player1Characters : player2Characters);

        for (int i = 0; i < currentTeam.Count; i++)
        {
            if (currentTeam[i].state != MGlobal.CharacterState.Dead)
            {
                return true;
            }
        }

        return false;
    }

    #region Game Start Inits
    void InitHeroesToTheScene()
    {
        string jsonPath = Application.persistentDataPath + "/SelectedHeroes.json";
        string jsonStr = File.ReadAllText(jsonPath);
        SelectedHeroList heroesInJson = JsonUtility.FromJson<SelectedHeroList>(jsonStr);
        int posCounter = 0;
        Vector3 pos;
        Quaternion rotation;

        foreach (SelectedHero hero in heroesInJson.SelectedHeroesList)
        {
            AddSelectedHeroes(hero.Id, hero.Name);
            if (PhotonNetwork.IsMasterClient)
            {
                if (posCounter == 0)
                {
                    pos = new Vector3((float)-0.74, (float)0, (float)-5);
                    rotation = Quaternion.identity;
                }
                else if (posCounter == 1)
                {
                    pos = new Vector3((float)0.47, (float)0, (float)-5);
                    rotation = Quaternion.identity;
                }
                else if (posCounter == 2)
                {
                    pos = new Vector3((float)1.7, (float)0, (float)-5);
                    rotation = Quaternion.identity;
                }
                else
                {
                    pos = new Vector3(0, 0, 0);
                    rotation = Quaternion.identity;
                }
            }
            else
            {
                if (posCounter == 0)
                {
                    pos = new Vector3((float)-0.74, 0, (float)-3.17);
                    rotation = Quaternion.Euler(0f, 180f, 0f);

                }
                else if (posCounter == 1)
                {
                    pos = new Vector3((float)0.47, 0, (float)-3.17);
                    rotation = Quaternion.Euler(0f, 180f, 0f);
                }
                else if (posCounter == 2)
                {
                    pos = new Vector3((float)1.7, 0, (float)-3.17);
                    rotation = Quaternion.Euler(0f, 180f, 0f);
                }
                else
                {
                    pos = new Vector3(0, 0, 0);
                    rotation = Quaternion.Euler(0f, 180f, 0f);
                }
            }
            InitHeroesToTeamPlayer(hero.Id, pos, rotation);
            posCounter++;
        }
    }

    public void AddSelectedHeroes(int id, string name)
    {
        SelectedHero newHero = new SelectedHero();
        newHero.Id = id;
        newHero.Name = name;
        selectedHeroList.SelectedHeroesList.Add(newHero);
    }


    public void InitHeroesToTeamPlayer(int id, Vector3 pos, Quaternion rotation)
    {
        GameObject newHero = PhotonNetwork.Instantiate(PlayerHeroes[id].name, pos, rotation);
        newHero.GetComponent<MCharacter>().battleManager = battleManager;
        newHero.GetComponent<MCharacter>().battleManagerUI = battleManagerUI;
        newHero.GetComponent<MCharacter>().id = id;
        FindObjectOfType<MBattleManager>().playerCharactersToInit.Add(newHero.GetComponent<MCharacter>());
    }

    public void InitTeamsToBattleManager()
    {
        view.RPC("InitTeamsToBattleManagerRPC", RpcTarget.All);
    }
    /// <summary>
    /// Battlemanager ilyenkor a host-é tehát aki kesobb csatlakozik be az a player2. Host a player1. tehát a ismine az a host.
    /// </summary>
    [PunRPC]
    public void InitTeamsToBattleManagerRPC()
    {
        MCharacter[] characters = FindObjectsOfType<MCharacter>();
        MBattleManager battleManager = FindObjectOfType<MBattleManager>();
        Debug.Log("All player connected... Init Heroes to BattleManager");
        if (view.IsMine)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i].gameObject.GetComponent<PhotonView>().IsMine)
                {
                    battleManager.player1Characters.Add(characters[i].GetComponent<MCharacter>());
                }
                else
                {
                    battleManager.player2Characters.Add(characters[i].GetComponent<MCharacter>());
                    characters[i].gameObject.GetComponent<MCharacter>().battleManager = battleManager;
                    characters[i].gameObject.GetComponent<MCharacter>().battleManagerUI = battleManagerUI;
                    characters[i].gameObject.tag = "Enemy";
                }
            }
            for (int i = 0; i < player2Characters.Count; i++)
            {
                player2Characters[i].id = i;
            }
        }
        else
        {
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i].gameObject.GetComponent<PhotonView>().IsMine)
                {
                    battleManager.player2Characters.Add(characters[i].GetComponent<MCharacter>());
                }
                else
                {
                    battleManager.player1Characters.Add(characters[i].GetComponent<MCharacter>());
                    characters[i].gameObject.GetComponent<MCharacter>().battleManager = battleManager;
                    characters[i].gameObject.GetComponent<MCharacter>().battleManagerUI = battleManagerUI;
                    characters[i].gameObject.tag = "Enemy";
                }
            }
            for (int i = 0; i < player1Characters.Count; i++)
            {
                player1Characters[i].id = i;
            }
        }
        CreateThePlayerUI();
    }

    public void CreateThePlayerUI()
    {
        Debug.Log("Creating the UI");
        Debug.Log("Creating the enemyHolder Panel");
        battleManagerUI.createEnemyCharactersHolder();
        Debug.Log("Creating the enemy characters into the holder");
        foreach (MCharacter character in player2Characters)
        {
            battleManagerUI.createEnemyCharactersIntoTheHolder(character.name, character.health, character.mana);
        }
        Debug.Log("Creating the action panel");
        battleManagerUI.enableActionPanel();
        Debug.Log("Creating the mycharacters panel");
        foreach (MCharacter character in player1Characters)
        {
            battleManagerUI.createMyCharactersIntoHolder(character.name, character.health, character.mana);
        }
        initPlayerVariables();
    }

    void initPlayerVariables()
    {
        teamPlayer1Choices = new int[player1Characters.Count];
        teamPlayer1SkillChoices = new int[player1Characters.Count];
        teamPlayer1AllyTargets = new int[player1Characters.Count];
        teamPlayer1EnemyTargets = new int[player1Characters.Count];
        Debug.Log(teamPlayer1Choices.ToString());
        teamPlayer2Choices = new int[player2Characters.Count];
        teamPlayer2SkillChoices = new int[player2Characters.Count];
        teamPlayer2AllyTargets = new int[player2Characters.Count];
        teamPlayer2EnemyTargets = new int[player2Characters.Count];

        battlePhase = MGlobal.BattlePhase.TeamPlayer1Selection;
        isEverythingInitialized = true;
    }

    
    public void InitChoices()
    {
        view.RPC("InitChoicesRPC", RpcTarget.All);
    }

    [PunRPC]
    void InitChoicesRPC()
    {
        currentPlayer1Character = 0;

        for (int i = 0; i < player1Characters.Count; i++)
        {
            teamPlayer1Choices[i] = 0;
            teamPlayer1SkillChoices[i] = 0;
        }

        for (int i = 0; i < player2Characters.Count; i++)
        {
            teamPlayer2Choices[i] = 0;
            teamPlayer2SkillChoices[i] = 0;
        }
    }
    #endregion


    #region Pun Callbacks
    public override void OnJoinedRoom()
    {
        Debug.Log("You joined the room.");
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log(PhotonNetwork.IsMasterClient);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("Other players joined the room.");
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount + "/2 Getting ready for the game...");
            battleManagerUI.hideWaitingForPlayersText();
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Enemy left the server. You win.");
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MultiplayerLobby");
    }
    #endregion
}
