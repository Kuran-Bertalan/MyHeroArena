using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControlling : BasicController
{
    private float timeRemaining = 10;
    // Start is called before the first frame update
    void Start()
    {
        data.npc = GetComponent<NavMeshAgent>();
        data.target = GameObject.FindGameObjectWithTag("Drink").transform;
        if (data.npc != null)
        {
            SetState(State.Walk);

            data.full = false;
            data.npc.speed = data.speed;
            data.npc.SetDestination(RandomNavMeshLocation());
        }
    }

    // Update is called once per frame
    void Update()
    {
            if (data.npc != null && data.npc.remainingDistance <= data.npc.stoppingDistance)
            {
                data.npc.SetDestination(RandomNavMeshLocation());
            }
            float distance = Vector3.Distance(data.target.transform.position, transform.position);
            if (distance > data.maxThirstyDistance)
            {
                if (data.full == true)
                {                    
                data.full = false;
                SetState(State.Walk);
                }
            }
            if (distance <= data.maxThirstyDistance && data.full == false)
            {
                SetState(State.Thirsty);

                if (distance <= data.minThirstyDistance)
                {
                    SetState(State.Drink);
                }

                if (distance <= data.IsFull)
                {
                    data.full = true;
                    SetState(State.Full);
                }
            }
            RunState();
        }
    }