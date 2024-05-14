using Unity.MLAgents.Actuators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Unity.VisualScripting;
using System.Xml.Linq;

public class PlayerController : Agent
{
    public float speed;
    public float rotationSpeed;
    public Transform TargetTransform;

    private Animator animator;

    private float mDistance = 0; 

    public override void Initialize()
    {
        mDistance = Vector3.Distance(TargetTransform.localPosition, transform.localPosition);
    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 0.5f, 0);
        animator = GetComponent<Animator>();

        transform.localPosition = new Vector3(25.0f, 0.0f, 3.0f);

        transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // The position of the agent
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);

        // The position of the treasure prefab
        sensor.AddObservation(TargetTransform.localPosition.x);
        sensor.AddObservation(TargetTransform.localPosition.y);

        // The distance between the agent and the treasure
        sensor.AddObservation(Vector3.Distance(TargetTransform.localPosition, transform.localPosition));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

        ActionSegment<float> actions = actionsOut.ContinuousActions;

        actions[0] = -1;
        actions[1] = 0;

        if (Input.GetKey("w"))
        {
            actions[0] = 1;
            animator.SetFloat("speed", actions[0]);
        }
        if (Input.GetKey("d"))
        {
            actions[1] = +0.5f;
        }
        if (Input.GetKey("a"))
        {
            actions[1] = -0.5f;
        }
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionTaken = actions.ContinuousActions;
        float actionSpeed = (actionTaken[0] + 1) / 2;
        float actionSteering = actionTaken[1];

        transform.Rotate(0, Input.GetAxis("Horizontal") + actionSteering * speed, 0);
        var forward = transform.TransformDirection(Vector3.forward);
        float curSpeed = actionSpeed * Input.GetAxis("Vertical");

        transform.Translate(forward * actionSpeed * speed * Time.deltaTime, Space.World);

        //AddReward(-0.01f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            AddReward(-1.0f + (((mDistance - Vector3.Distance(transform.localPosition, TargetTransform.localPosition)) / mDistance) / 10.0f));
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-1.0f + (((mDistance - Vector3.Distance(transform.localPosition, TargetTransform.localPosition)) / mDistance) / 10.0f));
            EndEpisode();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            AddReward(-1.0f + (((mDistance - Vector3.Distance(transform.localPosition, TargetTransform.localPosition)) / mDistance) / 10.0f));
            EndEpisode();
        }
        else if (other.gameObject.CompareTag("Treasure"))
        {
            Debug.Log("Treasure");
            AddReward(2.0f);
            EndEpisode();
        }
    }
}
