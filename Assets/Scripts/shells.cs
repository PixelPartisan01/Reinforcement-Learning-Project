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

public class shells : MonoBehaviour
{

    private Vector3 _destination;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] GameObject _waypointsParentGO;
    [SerializeField] GameObject[] _waypointsGO;
    [SerializeField] int _numOfWaypoints;
    [SerializeField] private bool _reachedLastWP = false;
    [SerializeField] private bool _reachedCurrentWP = false;
    private Vector3 _currentWPPosition;
    private int _currentWP;
    private int _lastWP;

    public Transform goal;

    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _waypointsParentGO = GameObject.Find("Waypoints");

        _numOfWaypoints = _waypointsParentGO.GetComponentInChildren<Transform>().childCount + 1;
        _waypointsGO = new GameObject[_numOfWaypoints];

        int i = 0;
        foreach(Transform wp in _waypointsParentGO.GetComponentInChildren<Transform>())
        {
            _waypointsGO[i] = wp.GameObject();
            i++;
        }

        //GetRandomWaypoint();
        //_navMeshAgent.destination = _destination;

        //NavMeshAgent agent = GetComponent<NavMeshAgent>();
        //agent.destination = goal.position;

        _currentWP = 0;
    }

    private void GetRandomWaypoint()
    {
        int rand = UnityEngine.Random.Range(1, _numOfWaypoints);
        _destination = _waypointsGO[rand].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
      if(_navMeshAgent.remainingDistance > 0.1f)
        {
            return;
        }
      else
        {
            _reachedCurrentWP = true;
            UpdateCurrentWP();
        }

      if(! _reachedCurrentWP && _reachedLastWP)
        {
            MoveToNextWaypoint();
            _reachedCurrentWP = false;
        }
    }

    private void UpdateCurrentWP()
    {
        if(_currentWP < _lastWP)
        {
            _currentWP += 1;
            _currentWPPosition = _waypointsGO[_currentWP].transform.position;
        }
        else
        {
            _reachedLastWP = true;
        }
    }

    private void MoveToNextWaypoint()
    {
        _navMeshAgent.destination = _currentWPPosition;
    }
}
