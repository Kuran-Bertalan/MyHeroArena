using UnityEngine;
using UnityEngine.AI;

public enum State { None, Idle, Walk, Thirsty, Drink, Full}



[System.Serializable]
public struct AIData
{
    public NavMeshAgent npc;
    public Transform target;
    public State state;
    [HideInInspector] public State next;


    [Range(0, 100)] public float speed;
    [Range(0, 100)] public float runspeed;

    [Range(1, 50)] public float radius;

    [Range(0, 100)] public float minThirstyDistance;
    [Range(0, 100)] public float maxThirstyDistance;
    [Range(0, 100)] public double IsFull;

    public bool full;
}
