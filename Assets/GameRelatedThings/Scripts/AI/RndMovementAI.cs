using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RndMovementAI : MonoBehaviour
{
    private NavMeshAgent npc;

    public float movement_radius;

    private void Start()
    {
        npc = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!npc.hasPath)
        {
            npc.SetDestination(Point.Instance.GetRandomPoint());
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, movement_radius);
    }
}