using Unity.MLAgents.Actuators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.AI;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms;

public class Enemy : MonoBehaviour
{
    //GameObject player;
    NavMeshAgent agent;
    [SerializeField] LayerMask groundLayer;//, playerLayer;

    Vector3 desPoint;
    bool walkpontSet;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SearchForDestination();
        agent.SetDestination(desPoint);
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        //if (!walkpontSet) SearchForDestination();
        //if (walkpontSet) agent.SetDestination(desPoint);
        //if (Vector3.Distance(transform.localPosition, desPoint) < 10) walkpontSet = false;
        if ((agent.remainingDistance < 0.1) || !agent.isOnNavMesh ||agent.isPathStale)
        {
            SearchForDestination();
            agent.SetDestination(desPoint);
        }
    }

    void SearchForDestination()
    {
        //float z = UnityEngine.Random.Range(-2.5f, 7.5f);
        //float x = UnityEngine.Random.Range(-15.0f, 15.0f);

        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 20;

        randomDirection += transform.position;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomDirection, out hit, 20, 1);

        desPoint = hit.position;

        

        //desPoint = new Vector3(transform.localPosition.x + ( transform.localPosition.x - UnityEngine.Random.Range(-15.0f, 15.0f)), 0, transform.localPosition.z + UnityEngine.Random.Range(-2.5f, 7.5f));




        //walkpontSet = true;

        //if (Physics.Raycast(desPoint, Vector3.down, groundLayer))
        //{
        //    walkpontSet = true;
        //}
    }
}
