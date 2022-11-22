using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicController : MonoBehaviour
{
    [SerializeField] protected AIData data;

    private void Idle() => RunAgent(transform.position, 0.0f);
    private void Walk() => RunAgent(RandomNavMeshLocation(), data.speed);
    private void Thirsty() => RunAgent(data.target.transform.position, data.speed);
    private void Drink() => RunAgent(data.target.transform.position, data.runspeed);
    private void Full() => RunAgent(RandomNavMeshLocation(), data.speed);

    protected void SetState(State state)
    {
        data.next = state;
        if(data.next != data.state)
        {
            data.state = data.next;
        }
    }

    protected void RunState()
    {
        switch (data.state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Walk:
                Walk();
                break;
            case State.Thirsty:
                Thirsty();
                break;
            case State.Drink:
                Drink();
                break;
            case State.Full:
                Full();
                break;
        }
    }
       private void RunAgent(Vector3 dest, float speed)
    {
        if(data.npc != null && data.npc.remainingDistance <= data.npc.stoppingDistance)
        {
            data.npc.speed = speed;
            data.npc.SetDestination(dest);
        }
    }


        protected Vector3 RandomNavMeshLocation()
    {
        Vector3 finalPos = Vector3.zero;
        Vector3 rndDirect= Random.insideUnitSphere * data.radius;
        rndDirect += transform.position;
        if(NavMesh.SamplePosition(rndDirect, out NavMeshHit hit, data.radius, 1))
        {
            finalPos = hit.position;
        }
        return finalPos;
    }
    }