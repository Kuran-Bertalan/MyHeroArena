using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class patrollingAgent : MonoBehaviour
{
    public Transform[] patrolPoints;
    public int speed;

    private int patrolPointIndex=0;
    private float distance;

    private NavMeshAgent patroller;

    // Start is called before the first frame update
    void Start()
    {
        patroller = GetComponent<NavMeshAgent>();
        patroller.autoBraking = false;
        patroller.transform.LookAt(patrolPoints[patrolPointIndex].position);
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, patrolPoints[patrolPointIndex].position);
        if(distance <1f)
        {
            IncrementIndex();
        }
        patroller.transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void IncrementIndex()
    {
        if (patrolPoints.Length == 0)
            return;

        patrolPointIndex++;
        if(patrolPointIndex >= patrolPoints.Length)
        {
            patrolPointIndex = 0;
        }
        patroller.transform.LookAt(patrolPoints[patrolPointIndex].position);
    }
}
