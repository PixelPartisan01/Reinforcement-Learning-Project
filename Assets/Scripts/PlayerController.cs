using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PlayerController : Agent
{
    public float speed = 5f;

    public Transform TargetTransform;

    public Animator animator;

    private enum ACTIONS
    {
        LEFT = 0,
        FORWARD = 1,
        RIGHT = 2,
        BACKWARD = 3
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 0.5f, 0);
        animator = GetComponent<Animator>();

        // Generate a random position for the treasure prefab 
        //float xPosition = UnityEngine.Random.Range(-9, 9);
        //float zPosition = UnityEngine.Random.Range(-9, 9);



        // Assign the randomly generated position to the treasure prefab
        TargetTransform.localPosition = new Vector3(25.0f, 0.0f, 3.0f);
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
        ActionSegment<int> actions = actionsOut.DiscreteActions;

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == +1)
        {
            actions[0] = (int)ACTIONS.LEFT;
        }
        else if (horizontal == -1)
        {
            actions[0] = (int)ACTIONS.RIGHT;
        }
        else if (vertical == +1)
        {
            actions[0] = (int)ACTIONS.FORWARD;
        }
        else if (vertical == -1)
        {
            actions[0] = (int)ACTIONS.BACKWARD;
        }
        else
        {
            actions[0] = 3;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionTaken = actions.DiscreteActions[0];
        

        switch (actionTaken)
        {
            case (int)ACTIONS.FORWARD:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                animator.SetBool("walk", true);
                break;
            case (int)ACTIONS.LEFT:
                transform.rotation = Quaternion.Euler(0, -90, 0);
                animator.SetBool("walk", true);
                break;
            case (int)ACTIONS.RIGHT:
                transform.rotation = Quaternion.Euler(0, +90, 0);
                animator.SetBool("walk", true);
                break;
            case (int)ACTIONS.BACKWARD:
                transform.rotation = Quaternion.Euler(0, 180, 0);
                animator.SetBool("walk", true);
                break;
        }

        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);

        AddReward(-0.01f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall" || collision.collider.tag == "Water")
        {
            Debug.Log("dead");
            AddReward(-1);
            //animator.SetBool("alive", false);
            EndEpisode();
        }
        else if (collision.collider.tag == "Treasure")
        {
            AddReward(1);
            EndEpisode();
        }
    }
}
