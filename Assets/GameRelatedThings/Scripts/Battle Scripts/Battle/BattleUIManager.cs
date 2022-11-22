using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[ExecuteInEditMode]
public class BattleUIManager : MonoBehaviour
{
	public GameObject nextLevelWindow;

	public BattleManager battleManager;

	public List<Sprite> enemyAvatars;
	public List<Sprite> allyAvatars;

	private bool displaySkillsWindow = false;

	private bool isSelectingTarget = false;
	private bool isSelectingEnemyTarget = false;

	private int cacheChoice = 0;
	private int cacheSkillChoice = 0;

	public Texture2D hpBarTexture;
	public Texture2D mpBarTexture;
	public Texture2D backgroundBarTexture;

	private bool isDisplayingDamage;
	private ArrayList displayedDamage;
	private ArrayList displayedDamagePosition;
	private ArrayList displayedDamageColor;
	private ArrayList timeToDisplay;

	private string displayedMessage;
	private bool isDisplayingMessage;
	private float displayMessageTime;
	private bool displayMessageFullScreenWidth;

	public GUIStyle textStyle;

	public Canvas Canvas;

	//Enemy Characters Holder
	public GameObject panelHolderObject;
    public GameObject characterHolderObject;
    GameObject panelHolder;

    // Action Panel Holder
    public GameObject myCharactersPanel;
    public GameObject myCharacterHolder;

    void Start()
	{
		displayedDamage = new ArrayList();
		displayedDamagePosition = new ArrayList();
		displayedDamageColor = new ArrayList();
		timeToDisplay = new ArrayList();
	}

    public void createEnemyCharactersHolder()
    {
        Vector3 position = new Vector3(0, 425, 0);
        Quaternion quaternion = new Quaternion(0, 0, 0, 1);
        var createdPanel = Instantiate(panelHolderObject, position, quaternion);
        createdPanel.transform.SetParent(Canvas.transform, false);
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
				characterImageHolder.sprite = enemyAvatars[0];
				break;
			case "Berserker":
				characterImageHolder.sprite = enemyAvatars[1];
				break;
			case "Rogue":
				characterImageHolder.sprite = enemyAvatars[2];
				break;
			case "Skeleton":
				characterImageHolder.sprite = enemyAvatars[3];
				break;
			case "Warrior":
				characterImageHolder.sprite = enemyAvatars[4];
				break;
			case "Boss":
				characterImageHolder.sprite = enemyAvatars[5];
				break;
			default:
				characterImageHolder.sprite = enemyAvatars[0];
				break;
		}
		
        healthText.text = "Health: " + health.ToString();
        manaText.text = "Mana: " + mana.ToString();
        Debug.Log(avatarName + health + mana);
    }

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
                myCharacterImageHolder.sprite = allyAvatars[0];
                break;
            case "Berserker":
                myCharacterImageHolder.sprite = allyAvatars[1];
                break;
            case "Mage":
                myCharacterImageHolder.sprite = allyAvatars[2];
                break;
            case "Rogue":
                myCharacterImageHolder.sprite = allyAvatars[3];
                break;
            case "Tank":
                myCharacterImageHolder.sprite = allyAvatars[4];
                break;
            default:
                myCharacterImageHolder.sprite = allyAvatars[0];
                break;
        }
        nameText.text = avatarName;
        healthText.text = health.ToString();
        manaText.text = mana.ToString();
    }

	// MANA HIÁNYZIK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	public void updateEnemyMana(int mana, int enemyId)
	{
		var enemyHolder = Canvas.transform.GetChild(1).GetChild(enemyId);
		var text = enemyHolder.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
		text.text = "Mana: " + mana.ToString();
	}

	public void updatePlayerCharactersMana(int mana, int characterId)
	{
		var mycharacterholder = myCharactersPanel.transform.GetChild(characterId);
		var text = mycharacterholder.transform.GetChild(5).GetComponent<TextMeshProUGUI>();
		text.text = mana.ToString();
	}

	public void updateEnemyHealth(int health, int enemyId)
	{
		var enemyHolder = Canvas.transform.GetChild(1).GetChild(enemyId);
		var text = enemyHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
		text.text = "Health: " + health.ToString();
	}

	public void updatePlayerCharactersHealth(int health, int characterId)
	{
		var mycharacterholder = myCharactersPanel.transform.GetChild(characterId);
		var text = mycharacterholder.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
		text.text = health.ToString();
	}

	void OnGUI()
	{
		if (isDisplayingMessage)
			if (displayMessageTime < Time.time && displayMessageTime > 0.0f)
				isDisplayingMessage = false;
			else
				GUI.Window(0, new Rect(20, Screen.height - 210, ((displayMessageFullScreenWidth) ? Screen.width - 40 : 400), 200), MessageWindow, "Actions");

		if (battleManager.GetPhase() != Global.BattlePhase.BattleEnds)
		{
			if (battleManager.GetPhase() == Global.BattlePhase.TeamPlayerSelection && !isDisplayingMessage)
				GUI.Window(1, new Rect(20, Screen.height - 210, 200, 200), ActionsWindow, battleManager.GetCurrentCharacter().name);

			if (displaySkillsWindow && !isDisplayingMessage)
				GUI.Window(2, new Rect(230, Screen.height - 210, 200, 200), SkillsWindow, battleManager.GetCurrentCharacter().name + " skills");

			if (isSelectingTarget && !isDisplayingMessage)
				GUI.Window(3, new Rect(((displaySkillsWindow) ? 440 : 230), Screen.height - 210, 200, 200), SelectTargetWindow, "Select the target");
		}

		if (isDisplayingDamage)
		{
			for (int i = 0; i < displayedDamage.Count; i++)
			{
				if ((float)timeToDisplay[i] < Time.time)
				{
					displayedDamage.RemoveAt(i);
					displayedDamagePosition.RemoveAt(i);
					displayedDamageColor.RemoveAt(i);
					timeToDisplay.RemoveAt(i);
				}
				else
				{
					Vector3 screenPosition = Camera.main.WorldToScreenPoint((Vector3)displayedDamagePosition[i]);

					textStyle.fontSize = 20;
					textStyle.fontStyle = FontStyle.Bold;

					DrawOutlineLabel(
						new Rect(screenPosition.x - 25, Screen.height - screenPosition.y - 50 + (((float)timeToDisplay[i] - Time.time) * 30), 100, 100),
						"" + (int)displayedDamage[i],
						Color.black,
						(Color)displayedDamageColor[i],
						textStyle
					);

					textStyle.fontStyle = FontStyle.Normal;
				}
			}
			if (displayedDamage.Count == 0)
				isDisplayingDamage = false;
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
			SceneManager.LoadScene("Map");
		}
	}

	void SkillsWindow(int windowID)
	{
		for (int i = 0; i < battleManager.GetCurrentCharacter().skills.Length; i++)
		{
			Skill skill = battleManager.GetCurrentCharacter().skills[i];

			if (battleManager.GetCurrentCharacter().GetCurrentMana() < skill.manaCost)
				GUI.enabled = false;

			if (GUI.Button(new Rect(10, 20 + (35 * i), 180, 30), skill.name))
			{

				if (skill.target == Global.Targets.Ally || skill.target == Global.Targets.Enemy)
				{
					isSelectingTarget = true;
					cacheChoice = 2;
					cacheSkillChoice = i;

					if (skill.target == Global.Targets.Enemy)
					{
						isSelectingEnemyTarget = true;
					}
					else if (skill.target == Global.Targets.Ally)
					{
						isSelectingEnemyTarget = false;
					}
				}
				else
				{
					isSelectingTarget = false;
					displaySkillsWindow = false;

					battleManager.SetPlayerChoice(2, i, 0, false);
				}
			}
			GUI.enabled = true;
		}
	}

	void SelectTargetWindow(int windowID)
	{
		if (isSelectingEnemyTarget)
		{
			for (int i = 0; i < battleManager.teamEnemy.Count; i++)
			{
				if (battleManager.teamEnemy[i].state == Global.CharacterState.Dead)
					GUI.enabled = false;

				if (GUI.Button(new Rect(10, 20 + (35 * i), 180, 30), battleManager.teamEnemy[i].name))
				{
					isSelectingTarget = false;
					displaySkillsWindow = false;
					Debug.Log("katt");
					battleManager.SetPlayerChoice(cacheChoice, cacheSkillChoice, i, true);
				}

				GUI.enabled = true;
			}
		}
		else
		{
			for (int i = 0; i < battleManager.teamPlayer.Count; i++)
			{
			//	if (battleManager.teamPlayer[i].state == Global.State.Dead)
			//	{
			//		if (!battleManager.GetCurrentCharacter().skills[cacheSkillChoice].overDead)
			//			GUI.enabled = false;
			//	}

				if (GUI.Button(new Rect(10, 20 + (35 * i), 180, 30), battleManager.teamPlayer[i].name))
				{
					isSelectingTarget = false;
					displaySkillsWindow = false;
					
					battleManager.SetPlayerChoice(cacheChoice, cacheSkillChoice, i, false);
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

		if (battleManager.GetPhase() == Global.BattlePhase.BattleEnds)
		{
			nextLevelWindow.SetActive(true);
		}
	}

	public void DisplayDamage(int damage, Vector3 position, Color color)
	{
		isDisplayingDamage = true;
		displayedDamage.Add(damage);
		displayedDamagePosition.Add(position);
		displayedDamageColor.Add(color);
		timeToDisplay.Add(Time.time + 2);
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
}
