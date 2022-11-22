using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NavigationMeshAgent : MonoBehaviour
{
    private bool mustMove;
    public GameObject gameObj;
    public GameObject gameObj2;
    public GameObject gameObj3;
    float timer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 2)
        {
            if (gameObj.tag == "Bush")
            {
                GetComponent<NavMeshAgent>().SetDestination(gameObj.transform.position);
                if (timer >= 20)
                {
                    gameObj3.SetActive(false);
                    if (gameObj2.tag == "Forest")
                    {
                        mustMove = true;
                        GetComponent<NavMeshAgent>().SetDestination(gameObj2.transform.position);
                        if (timer >= 35)
                        {
                            gameObj3.SetActive(true);
                            GetComponent<NavMeshAgent>().SetDestination(gameObj.transform.position);
                            timer = 3;
                        }
                    }
                }
            }
        }
    }
}