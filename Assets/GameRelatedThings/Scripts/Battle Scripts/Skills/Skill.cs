using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
	public string name;

	public int manaCost;

	public Global.AttackType attackType;

	public Global.ActionType actionType;
	public Global.CharacterStat targetStat;

	public Global.Targets target;

	public GameObject projectile;

	public GameObject effectObject;
	public Global.Position effectObjectPosition;
	public float effectObjectDuration;

}
