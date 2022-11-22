using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BattleManager : MonoBehaviour
{
    private Global.BattlePhase battlePhase = Global.BattlePhase.BattleStart;

    public Player Player;

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

	public SelectedHeroList selectedHeroList = new SelectedHeroList();

	public List<Character> teamPlayer;
	private int currentPlayerCharacter = 0;
	private int[] teamPlayerChoices;
	private int[] teamPlayerSkillChoices;
	private int[] teamPlayerAllyTargets;
	private int[] teamPlayerEnemyTargets;

	public List<Character> teamEnemy;
	private int currentEnemyCharacter = 0;
	private int[] teamEnemyChoices;
	private int[] teamEnemySkillChoices;
	private int[] teamEnemyAllyTargets;
	private int[] teamEnemyEnemyTargets;
	
	private bool waitingToNextExecution = false;

	public Character[] characters;
	public string[] teams;
	public int[] choices;
	public int[] skillChoices;
	public int[] allyTargets;
	public int[] enemyTargets;

	public List<GameObject> AllyHeroes;
	public List<GameObject> EnemyHeroes; 

	public BattleManager battleManager;
	public BattleUIManager battleUIManager;

	public GameObject characterPos1;
	public GameObject characterPos2;
	public GameObject characterPos3;

	void Start()
	{
		if(SceneManager.GetActiveScene().name == "Tutorial")
        {
			GameObject position = new GameObject();
			position.transform.localPosition = new Vector3(-12.32f, -1.39f, -11.06f);
			
			InitHeroesToTeamPlayer(1, position);
            
		}
        else
        {
			InitHeroesToTheScene();
		}
		teamPlayerChoices = new int[teamPlayer.Count];
		teamPlayerSkillChoices = new int[teamPlayer.Count];
		teamPlayerAllyTargets = new int[teamPlayer.Count];
		teamPlayerEnemyTargets = new int[teamPlayer.Count];

		teamEnemyChoices = new int[teamEnemy.Count];
		teamEnemySkillChoices = new int[teamEnemy.Count];
		teamEnemyAllyTargets = new int[teamEnemy.Count];
		teamEnemyEnemyTargets = new int[teamEnemy.Count];

		InitChoices();

		//print("Battle Start!");
		//battleGUIManager.DisplayMessage("Battle start!");

		battlePhase = Global.BattlePhase.TeamPlayerSelection;
		CreateThePlayerUI();
	}

	void InitHeroesToTheScene()
    {
		string jsonPath = Application.persistentDataPath + "/SelectedHeroes.json";
		string jsonStr = File.ReadAllText(jsonPath);
		SelectedHeroList heroesInJson = JsonUtility.FromJson<SelectedHeroList>(jsonStr);
		int posCounter = 0;
		GameObject pos;
		
		foreach (SelectedHero hero in heroesInJson.SelectedHeroesList)
        {
            AddSelectedHeroes(hero.Id, hero.Name);
			if (posCounter == 0)
			{
				pos = characterPos1;
			}
			else if (posCounter == 1)
			{
				pos = characterPos2;
			}
			else if (posCounter == 2)
			{
				pos = characterPos3;
			}
			else
			{
				pos = characterPos1;
			}
			InitHeroesToTeamPlayer(hero.Id, pos);
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

	public void InitHeroesToTeamPlayer(int id, GameObject pos)
    {
		GameObject newHero = Instantiate(AllyHeroes[id], new Vector3(pos.transform.localPosition.x, pos.transform.localPosition.y, pos.transform.localPosition.z), Quaternion.identity);
		newHero.GetComponent<Character>().battleManager = battleManager;
		newHero.GetComponent<Character>().battleUIManager = battleUIManager;
		newHero.GetComponent<Character>().id = teamPlayer.Count;
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
			Quaternion rotation;
			rotation = Quaternion.Euler(0f, 90f, 0f);
			newHero.transform.localRotation = rotation;
			newHero.transform.localScale = new Vector3(1, 1, 1);
		}
		FindObjectOfType<BattleManager>().teamPlayer.Add(newHero.GetComponent<Character>());
		FindObjectOfType<Player>().characters.Add(newHero.GetComponent<Character>());
	}

	void InitChoices()
	{
		currentPlayerCharacter = 0;

		for (int i = 0; i < teamPlayer.Count; i++)
		{
			teamPlayerChoices[i] = 0;
			teamPlayerSkillChoices[i] = 0;
		}

		for (int i = 0; i < teamEnemy.Count; i++)
		{
			teamEnemyChoices[i] = 0;
			teamEnemySkillChoices[i] = 0;
		}
	}

    public void CreateThePlayerUI()
    {
        Debug.Log("Creating the UI");
        Debug.Log("Creating the enemyHolder Panel");
		battleUIManager.createEnemyCharactersHolder();
        Debug.Log("Creating the enemy characters into the holder");
        foreach (Character character in teamEnemy)
        {
			battleUIManager.createEnemyCharactersIntoTheHolder(character.name, character.health, character.mana);
        }
        Debug.Log("Creating the mycharacters panel");
        foreach (Character character in teamPlayer)
        {
			battleUIManager.createMyCharactersIntoHolder(character.name, character.health, character.mana);
        }
    }

    void Update()
	{
		// Battle Ends
		if (battlePhase == Global.BattlePhase.BattleEnds)
		{
			print("Battle Ends!");	
			return;
		}

		// Team Player Selection
		if (battlePhase == Global.BattlePhase.TeamPlayerSelection)
		{
			//print("Select your action!");

			if (teamPlayerChoices[currentPlayerCharacter] != 0 || teamPlayer[currentPlayerCharacter].state == Global.CharacterState.Dead)
			{
				if (teamPlayer.Count > (currentPlayerCharacter + 1))
				{
					currentPlayerCharacter++;
				}
				else
				{
					battlePhase = Global.BattlePhase.TeamEnemySelection;

					currentPlayerCharacter = 0;
					currentEnemyCharacter = 0;
				}
			}
		}

		// Team Enemy Selection
		if (battlePhase == Global.BattlePhase.TeamEnemySelection)
		{
			print("The enemy is selecting his action!");

			AIEnemySelection();

			currentPlayerCharacter = 0;
			currentEnemyCharacter = 0;
			battlePhase = Global.BattlePhase.ExecuteActions;

			OrderCharactersBySpeed();
		}

		// Execute Actions
		if (battlePhase == Global.BattlePhase.ExecuteActions)
		{
			//print("Executing actions!");

			if (!waitingToNextExecution)
			{
				if (characters[currentPlayerCharacter].state != Global.CharacterState.Dead)
				{
					if (choices[currentPlayerCharacter] == 1)
					{
						// Attack
						Character currentCharacterTarget = ((teams[currentPlayerCharacter] != "A") ? teamPlayer[enemyTargets[currentPlayerCharacter]] : teamEnemy[enemyTargets[currentPlayerCharacter]]);

						if (currentCharacterTarget.state == Global.CharacterState.Dead)
							currentCharacterTarget = GetNextCharacterOfTeam(((teams[currentPlayerCharacter] != "A") ? "A" : "Enemy"));

						if (battlePhase != Global.BattlePhase.BattleEnds)
							characters[currentPlayerCharacter].Attack(
							currentCharacterTarget
						);

						waitingToNextExecution = true;

						battleUIManager.DisplayMessage(characters[currentPlayerCharacter].name + " attacks!");
					}
					else if (choices[currentPlayerCharacter] == 2)
					{
						// Use skill
						if (characters[currentPlayerCharacter].skills[skillChoices[currentPlayerCharacter]].target == Global.Targets.Enemy)
						{
							Character currentCharacterTarget = ((teams[currentPlayerCharacter] != "A") ? teamPlayer[enemyTargets[currentPlayerCharacter]] : teamEnemy[enemyTargets[currentPlayerCharacter]]);

							if (currentCharacterTarget.state == Global.CharacterState.Dead)
								currentCharacterTarget = GetNextCharacterOfTeam(((teams[currentPlayerCharacter] != "A") ? "A" : "Enemy"));

							characters[currentPlayerCharacter].UseSkill(skillChoices[currentPlayerCharacter], currentCharacterTarget);
						}
						else
						{
							characters[currentPlayerCharacter].UseSkill(skillChoices[currentPlayerCharacter], teamPlayer[allyTargets[currentPlayerCharacter]]);
						}

						waitingToNextExecution = true;

						battleUIManager.DisplayMessage(characters[currentPlayerCharacter].name + " use " + characters[currentPlayerCharacter].skills[skillChoices[currentPlayerCharacter]].name + "!");
					}
				}
			}

			if ((!characters[currentPlayerCharacter].IsAttacking() && !characters[currentPlayerCharacter].IsUsingSkill() && !characters[currentPlayerCharacter].IsMovingToInitialPosition()))
			{
				waitingToNextExecution = false;

				if (isPlayerHaveAliveCharacter("A") && isPlayerHaveAliveCharacter("Enemy"))
				{
					if (characters.Length > (currentPlayerCharacter + 1))
					{
						currentPlayerCharacter++;
					}
					else
					{
						InitChoices();
						battlePhase = Global.BattlePhase.TeamPlayerSelection;
					}
				}
				else
				{
					battleUIManager.DisplayMessage(((isPlayerHaveAliveCharacter("A")) ? "You win!" : "You lost!"), -1.0f, false);
					battlePhase = Global.BattlePhase.BattleEnds;
				}
			}
		}
	}
	// Csaba
	void AIEnemySelection()
	{
		for (int i = 0; i < teamEnemy.Count; i++)
		{
			teamEnemyChoices[i] = Random.Range(1, 3);

			if (teamEnemyChoices[i] == 2)
			{
				if (teamEnemy[i].skills.Length > 0)
				{
					teamEnemySkillChoices[i] = Random.Range(0, teamEnemy[i].skills.Length);
				}
				else
				{
					teamEnemyChoices[i] = 1;
				}
			}

            if (teamPlayer[i].GetCurrentHealth() >= (teamEnemy[i].GetCurrentHealth())/3)
            {
				teamEnemyEnemyTargets[i] = i;
			}
            else
            {
				teamEnemyEnemyTargets[i] = Random.Range(0, teamPlayer.Count);
			}

			if (teamPlayer[teamEnemyEnemyTargets[i]].state == Global.CharacterState.Dead)
			{
				for (int j = 0; j < teamPlayer.Count; j++)
				{
					if (teamPlayer[j].state != Global.CharacterState.Dead)
					{
						teamEnemyEnemyTargets[i] = j;
						break;
					}
				}
			}
		}
	}

	void OrderCharactersBySpeed()
	{
		//Group of characters, teams, choices, actions, targets.
		characters = new Character[(teamPlayer.Count + teamEnemy.Count)];
		teams = new string[(teamPlayer.Count + teamEnemy.Count)];
		choices = new int[(teamPlayer.Count + teamEnemy.Count)];
		skillChoices = new int[(teamPlayer.Count + teamEnemy.Count)];
		allyTargets = new int[(teamPlayer.Count + teamEnemy.Count)];
		enemyTargets = new int[(teamPlayer.Count + teamEnemy.Count)];

		int[] usedPositions = new int[(teamPlayer.Count + teamEnemy.Count)];

		for (int i = 0; i < characters.Length; i++)
		{
			Character currentFastestCharacter = null;
			string currentFastestCharacterTeam = null;
			int currentFastestCharacterPosition = 0;

			for (int j = 0; j < teamPlayer.Count; j++)
			{
				if (!InArray(usedPositions, teams, j, "A"))
				{
					if (currentFastestCharacter != null)
					{
						if (teamPlayer[j].actionSpeed > currentFastestCharacter.actionSpeed)
						{
							currentFastestCharacter = teamPlayer[j];
							currentFastestCharacterTeam = "A";
							currentFastestCharacterPosition = j;
						}
					}
					else
					{
						currentFastestCharacter = teamPlayer[j];
						currentFastestCharacterTeam = "A";
						currentFastestCharacterPosition = j;
					}
				}
			}

			for (int j = 0; j < teamEnemy.Count; j++)
			{
				if (!InArray(usedPositions, teams, j, "Enemy"))
				{
					if (currentFastestCharacter != null)
					{
						if (teamEnemy[j].actionSpeed > currentFastestCharacter.actionSpeed)
						{
							currentFastestCharacter = teamEnemy[j];
							currentFastestCharacterTeam = "Enemy";
							currentFastestCharacterPosition = j;
						}
					}
					else
					{
						currentFastestCharacter = teamEnemy[j];
						currentFastestCharacterTeam = "Enemy";
						currentFastestCharacterPosition = j;
					}
				}
			}

			characters[i] = currentFastestCharacter;
			teams[i] = currentFastestCharacterTeam;
			choices[i] = ((currentFastestCharacterTeam == "A") ? teamPlayerChoices[currentFastestCharacterPosition] : teamEnemyChoices[currentFastestCharacterPosition]);
			skillChoices[i] = ((currentFastestCharacterTeam == "A") ? teamPlayerSkillChoices[currentFastestCharacterPosition] : teamEnemySkillChoices[currentFastestCharacterPosition]);
			enemyTargets[i] = ((currentFastestCharacterTeam == "A") ? teamPlayerEnemyTargets[currentFastestCharacterPosition] : teamEnemyEnemyTargets[currentFastestCharacterPosition]);
			allyTargets[i] = ((currentFastestCharacterTeam == "A") ? teamPlayerAllyTargets[currentFastestCharacterPosition] : teamEnemyAllyTargets[currentFastestCharacterPosition]);
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

	public Global.BattlePhase GetPhase()
	{
		return battlePhase;
	}

	public Character GetCurrentCharacter()
	{
		return teamPlayer[currentPlayerCharacter];
	}
	
	public int GetCurrentCharacterInArrayPosition()
	{
		return currentPlayerCharacter;
	}
	
	public void SetPlayerChoice(int choice, int skill, int target, bool isEnemyTarget)
	{
		teamPlayerChoices[currentPlayerCharacter] = choice;

		if (choice == 2)
			teamPlayerSkillChoices[currentPlayerCharacter] = skill;

		if (isEnemyTarget)
			teamPlayerEnemyTargets[currentPlayerCharacter] = target;
		else
			teamPlayerAllyTargets[currentPlayerCharacter] = target;
	}

	Character GetNextCharacterOfTeam(string team)
	{
		List<Character> currentTeam = ((team == "A") ? teamPlayer : teamEnemy);

		for (int i = 0; i < currentTeam.Count; i++)
		{
			if (currentTeam[i].state != Global.CharacterState.Dead)
			{
				return currentTeam[i];
			}
		}

		battlePhase = Global.BattlePhase.BattleEnds;

		return null;
	}

	bool isPlayerHaveAliveCharacter(string team)
	{
		List<Character> currentTeam = ((team == "A") ? teamPlayer : teamEnemy);

		for (int i = 0; i < currentTeam.Count; i++)
		{
			if (currentTeam[i].state != Global.CharacterState.Dead)
			{
				return true;
			}
		}

		return false;
	}
}

