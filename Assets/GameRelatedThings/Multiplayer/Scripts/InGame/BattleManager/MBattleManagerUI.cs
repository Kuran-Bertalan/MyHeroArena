using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MBattleManagerUI : MonoBehaviour
{
    PhotonView view;
    public TMP_Text waitingForPlayersText;

    public Canvas multiplayerCanvas;

    //Enemy Characters Holder
    public List<Sprite> avatars;
    public GameObject panelHolderObject;
    public GameObject characterHolderObject;
    GameObject panelHolder;
    TMP_Text healthText;
    TMP_Text manaText;

    public MBattleManager battleManager;

    // Action Panel Holder
    public GameObject actionPanel;
    public GameObject enemyTurnPanel;
    public GameObject myCharactersPanel;
    public GameObject attackActionCharacter;
    public GameObject skillAction;
    public GameObject myCharacterHolder;

    //New ui

    private bool displaySkillsWindow = false;

    private bool isSelectingTarget = false;
    private bool isSelectingEnemyTarget = false;
    private string displayedMessage;
    private bool isDisplayingMessage;
    private float displayMessageTime;
    private bool displayMessageFullScreenWidth;
    private int cacheChoice = 0;
    private int cacheSkillChoice = 0;
    public GUIStyle textStyle;

    public void Start()
    {
        view = GetComponent<PhotonView>();
    }

    void OnGUI()
    {
        if (isDisplayingMessage)
            if (displayMessageTime < Time.time && displayMessageTime > 0.0f)
                isDisplayingMessage = false;
            else
                GUI.Window(0, new Rect(20, Screen.height - 210, ((displayMessageFullScreenWidth) ? Screen.width - 40 : 400), 200), MessageWindow, "Actions");

        if (battleManager.GetPhase() != MGlobal.BattlePhase.BattleEnds)
        {
            if (view.IsMine)
            {
                if (battleManager.GetPhase() == MGlobal.BattlePhase.TeamPlayer1Selection && !isDisplayingMessage)
                    GUI.Window(1, new Rect(20, Screen.height - 210, 200, 200), ActionsWindow, battleManager.GetCurrentCharacter("P1").name);

                //if (displaySkillsWindow && !isDisplayingMessage)
                //    GUI.Window(2, new Rect(230, Screen.height - 210, 200, 200), SkillsWindow, battleManager.GetCurrentCharacter("P1").name + " skills");

                if (isSelectingTarget && !isDisplayingMessage)
                    GUI.Window(3, new Rect(((displaySkillsWindow) ? 440 : 230), Screen.height - 210, 200, 200), SelectPlayer1Target, "Select the target");
                
                if(battleManager.GetPhase() == MGlobal.BattlePhase.TeamPlayer2Selection && !isDisplayingMessage)
                {
                    EnableTurnText();
                }
                else
                {
                    DisableTurnText();
                }
            }
            else
            {
                if (battleManager.GetPhase() == MGlobal.BattlePhase.TeamPlayer2Selection && !isDisplayingMessage)
                    GUI.Window(1, new Rect(20, Screen.height - 210, 200, 200), ActionsWindow, battleManager.GetCurrentCharacter("P2").name);

                //if (displaySkillsWindow && !isDisplayingMessage)
                //    GUI.Window(2, new Rect(230, Screen.height - 210, 200, 200), SkillsWindow, battleManager.GetCurrentCharacter("P2").name + " skills");

                if (isSelectingTarget && !isDisplayingMessage)
                    GUI.Window(3, new Rect(((displaySkillsWindow) ? 440 : 230), Screen.height - 210, 200, 200), SelectPlayer2Target, "Select the target");

                if (battleManager.GetPhase() == MGlobal.BattlePhase.TeamPlayer1Selection && !isDisplayingMessage)
                {
                    EnableTurnText();
                }
                else
                {
                    DisableTurnText();
                }
            }
        }

    }

    void ActionsWindow(int windowID)
    {
        if (GUI.Button(new Rect(10, 20, 180, 30), "Attack"))
        {
            isSelectingTarget = true;
            isSelectingEnemyTarget = true;
            displaySkillsWindow = false;

            cacheChoice = 1;
        }

        if (GUI.Button(new Rect(10, 55, 180, 30), "Skills"))
        {
            displaySkillsWindow = true;
            isSelectingTarget = false;
        }

        if (GUI.Button(new Rect(10, 160, 180, 30), "Surrender"))
        {
            SceneManager.LoadScene("Menu");
        }
    }

    //void SkillsWindow(int windowID)
    //{
    //    string player;

    //    if (view.IsMine)
    //    {
    //        player = "P1";
    //    }
    //    else
    //    {
    //        player = "P2";
    //    }
    //    for (int i = 0; i < battleManager.GetCurrentCharacter(player).skills.Length; i++)
    //    {
    //        Skill skill = battleManager.GetCurrentCharacter(player).skills[i];

    //        if (battleManager.GetCurrentCharacter(player).GetCurrentMana() < skill.manaCost)
    //            GUI.enabled = false;

    //        if (GUI.Button(new Rect(10, 20 + (35 * i), 180, 30), skill.name))
    //        {

    //            if (skill.targets == Global.Targets.Ally || skill.targets == Global.Targets.Enemy)
    //            {
    //                isSelectingTarget = true;
    //                cacheChoice = 2;
    //                cacheSkillChoice = i;

    //                if (skill.targets == Global.Targets.Enemy)
    //                {
    //                    isSelectingEnemyTarget = true;
    //                }
    //                else if (skill.targets == Global.Targets.Ally)
    //                {
    //                    isSelectingEnemyTarget = false;
    //                }
    //            }
    //            else
    //            {
    //                isSelectingTarget = false;
    //                displaySkillsWindow = false;

    //                battleManager.SetPlayer1Choice(2, i, 0, false);
    //            }
    //        }
    //        GUI.enabled = true;
    //    }
    //}

    void SelectPlayer1Target(int windowID)
    {
        if (isSelectingEnemyTarget)
        {
            for (int i = 0; i < battleManager.player2Characters.Count; i++)
            {
                if (battleManager.player2Characters[i].state == MGlobal.CharacterState.Dead)
                    GUI.enabled = false;

                if (GUI.Button(new Rect(10, 20 + (35 * i), 180, 30), battleManager.player2Characters[i].name))
                {
                    isSelectingTarget = false;
                    displaySkillsWindow = false;
                    Debug.Log("player1 choose");
                    battleManager.SetPlayer1Choice(cacheChoice, cacheSkillChoice, i, true);
                }

                GUI.enabled = true;
            }
        }
    }

    void SelectPlayer2Target(int windowID)
    {
        if (isSelectingEnemyTarget)
        {
            for (int i = 0; i < battleManager.player2Characters.Count; i++)
            {
                if (battleManager.player2Characters[i].state == MGlobal.CharacterState.Dead)
                    GUI.enabled = false;

                if (GUI.Button(new Rect(10, 20 + (35 * i), 180, 30), battleManager.player1Characters[i].name))
                {
                    isSelectingTarget = false;
                    displaySkillsWindow = false;
                    Debug.Log("player2 choose");
                    battleManager.SetPlayer2Choice(cacheChoice, cacheSkillChoice, i, true);
                }

                GUI.enabled = true;
            }
        }
    }

    void MessageWindow(int windowID)
    {
        textStyle.fontSize = 13;
        textStyle.fontStyle = FontStyle.Normal;

        DrawOutlineLabel(
            new Rect(10, 20, 700, 200),
            displayedMessage,
            Color.black,
            Color.white,
            textStyle
        );

        if (battleManager.GetPhase() == MGlobal.BattlePhase.BattleEnds)
        {
            //nextLevelWindow.SetActive(true);
            Debug.Log("GG");
        }
    }

    public void DisplayMessage(string message, float time = 2.0f, bool fullScreenWidth = false)
    {
        displayedMessage = message;
        isDisplayingMessage = true;

        if (time > 0.0f)
            displayMessageTime = time + Time.time;
        else
            displayMessageTime = -1.0f; // Infinite message time

        displayMessageFullScreenWidth = fullScreenWidth;
    }

    public void DrawOutlineLabel(Rect position, string text, Color outColor, Color inColor, GUIStyle style)
    {
        GUIStyle backupStyle = style;
        style.normal.textColor = outColor;

        position.x--;

        GUI.Label(position, text, style);
        position.x += 2;

        GUI.Label(position, text, style);
        position.x--;
        position.y--;

        GUI.Label(position, text, style);
        position.y += 2;

        GUI.Label(position, text, style);
        position.y--;

        style.normal.textColor = inColor;

        GUI.Label(position, text, style);

        style = backupStyle;
    }

    #region Base UI
    public void hideWaitingForPlayersText()
    {
        view.RPC("hideWaitingForPlayersTextRPC", RpcTarget.All);
    }

    [PunRPC]
    void hideWaitingForPlayersTextRPC()
    {
        waitingForPlayersText.gameObject.SetActive(false);
    }

    public void createEnemyCharactersHolder()
    {
        Vector3 position = new Vector3(0, 425, 0);
        Quaternion quaternion = new Quaternion(0, 0, 0, 1);
        var createdPanel = Instantiate(panelHolderObject, position, quaternion);
        createdPanel.transform.SetParent(multiplayerCanvas.transform, false);
        panelHolder = createdPanel;
    }

    public void createEnemyCharactersIntoTheHolder(string avatarName, int health, int mana)
    {
        var createdCharacterHolder = Instantiate(characterHolderObject);
        createdCharacterHolder.transform.SetParent(panelHolder.transform, false);
        Image characterImageHolder = createdCharacterHolder.transform.GetChild(3).GetComponent<Image>();
        TextMeshProUGUI healthText = createdCharacterHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI manaText = createdCharacterHolder.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        switch (avatarName)
        {
            case "Archer":
                characterImageHolder.sprite = avatars[0];
                break;
            case "Berserker":
                characterImageHolder.sprite = avatars[1];
                break;
            case "Mage":
                characterImageHolder.sprite = avatars[2];
                break;
            case "Rogue":
                characterImageHolder.sprite = avatars[3];
                break;
            case "Tank":
                characterImageHolder.sprite = avatars[4];
                break;
            default:
                characterImageHolder.sprite = avatars[0];
                break;
        }
        healthText.text = "Health: " + health.ToString();
        manaText.text = "Mana: " + mana.ToString();
        Debug.Log(avatarName + health + mana);
    }

    public void enableActionPanel()
    {
        actionPanel.SetActive(true);
    }

    //public void enableAttackActionPanel()
    //{
    //    attackPanelHolder.SetActive(true);
    //    List<MCharacter> characters = view.IsMine ? battleManager.player2Characters : battleManager.player1Characters;
    //    foreach (MCharacter character in characters)
    //    {
    //        var myAttackEnemyHolder = Instantiate(attackActionCharacter);
    //        myAttackEnemyHolder.transform.SetParent(attackActionPanel.transform, false);
    //        TextMeshProUGUI nameText = myAttackEnemyHolder.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    //        //for (int i = 0; i < characters.Count; i++)
    //        //{
    //        //    myAttackEnemyHolder.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { battleManager.SetPlayerChoice(1,0,i,true); });
    //        //}
    //        nameText.text = character.name;
    //        myAttackEnemyHolder.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { battleManager.SetPlayerChoice(1, 0, 1, true); });

    //    }

    //}

    public void createMyCharactersIntoHolder(string avatarName, int health, int mana)
    {
        var myCreateCharacterHolder = Instantiate(myCharacterHolder);
        myCreateCharacterHolder.transform.SetParent(myCharactersPanel.transform, false);
        Image myCharacterImageHolder = myCreateCharacterHolder.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI nameText = myCreateCharacterHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI healthText = myCreateCharacterHolder.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI manaText = myCreateCharacterHolder.transform.GetChild(5).GetComponent<TextMeshProUGUI>();
        switch (avatarName)
        {
            case "Archer":
                myCharacterImageHolder.sprite = avatars[0];
                break;
            case "Berserker":
                myCharacterImageHolder.sprite = avatars[1];
                break;
            case "Mage":
                myCharacterImageHolder.sprite = avatars[2];
                break;
            case "Rogue":
                myCharacterImageHolder.sprite = avatars[3];
                break;
            case "Tank":
                myCharacterImageHolder.sprite = avatars[4];
                break;
            default:
                myCharacterImageHolder.sprite = avatars[0];
                break;
        }
        nameText.text = avatarName;
        healthText.text = health.ToString();
        manaText.text = mana.ToString();
    }
    #endregion


    
    void EnableTurnText()
    {
        enemyTurnPanel.SetActive(true);
    }
    
    void DisableTurnText()
    {
        enemyTurnPanel.SetActive(false);
    }

    

    // MANA HIÁNYZIK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public void updateEnemyHealth(int health, int enemyId)
    {
        var enemyHolder = multiplayerCanvas.transform.GetChild(2).GetChild(enemyId);
        var text = enemyHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        text.text = "Health: " + health.ToString();
    }

    public void updatePlayerCharactersHealth(int health, int characterId)
    {
        var mycharacterholder = myCharactersPanel.transform.GetChild(characterId);
        var text = mycharacterholder.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        text.text = health.ToString();
    }

}
