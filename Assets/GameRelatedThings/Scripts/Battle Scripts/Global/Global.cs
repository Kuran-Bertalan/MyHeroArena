using UnityEngine;
using System.Collections;

public class Global : MonoBehaviour
{
	public enum AttackType
	{
		Ranged,
		Melee
	}

	public enum Targets
	{
		Ally,
		Enemy,
	}

	public enum ActionType
	{
		Damage
	}

	public enum CharacterStat
	{
		Health,
		Mana,
		Attack,
		Defense,
		Speed,
	}

	public enum CharacterState
	{
		Alive,
		Dead
	}

	public enum BattlePhase
	{
		BattleStart,
		TeamPlayerSelection,
		TeamEnemySelection,
		ExecuteActions,
		BattleEnds
	}

	public enum Position
	{
		Top,
		Center,
		Bottom
	}
}
