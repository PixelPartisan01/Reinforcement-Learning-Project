using Unity.MLAgents.Actuators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PlayerController : Agent
{
    public float speed = 1;
    public float rotationSpeed = 1.0f;

    public bool buttonPressed = false;

    public Transform TargetTransform;

    public Animator animator;

    //private enum ACTIONS
    //{
    //    LEFT = 0,
    //    FORWARD = 1,
    //    RIGHT = 2,
    //    BACKWARD = 3
    //}

    public override void OnEpisodeBegin()
    {
        
        transform.localPosition = new Vector3(0, 0.5f, 0);
        animator = GetComponent<Animator>();

        // Generate a random position for the treasure prefab 
        //float xPosition = UnityEngine.Random.Range(-9, 9);
        //float zPosition = UnityEngine.Random.Range(-9, 9);

        TargetTransform.localPosition = new Vector3(25.0f, 0.0f, 3.0f);
        //transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, rotationSpeed * Time.deltaTime);
        TargetTransform.rotation.Set(0.0f, -90.0f, 0.0f, 0.0f);
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
            buttonPressed = true;
            animator.SetBool("walk", true);
        }
        if (Input.GetKey("d"))
        {
            actions[1] = +rotationSpeed;
            buttonPressed = true;
            //animator.SetBool("walk", true);
        }
        if (Input.GetKey("a"))
        {
            actions[1] = -rotationSpeed;
            buttonPressed = true;
            //animator.SetBool("walk", true);
        }

        if(!Input.anyKey)
        {
            buttonPressed = false;
            animator.SetBool("walk", false);
        }
        
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionTaken = actions.ContinuousActions;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInptu = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInptu);

        float actionSpeed = (actionTaken[0] + 1) / 2;
        float actionSteering = actionTaken[1]; 

        transform.Translate(actionSpeed * transform.forward * speed * Time.fixedDeltaTime, Space.World);

        transform.eulerAngles = new Vector3(
                                   transform.eulerAngles.x,
                                   transform.eulerAngles.y + actionSteering,
                                   transform.eulerAngles.z
                                           ); 

        //transform.rotation = Quaternion.Euler(new Vector3(0, actionSteering, 0));

        //if(movementDirection != Vector3.zero)
        //{
        //    //transform.forward = movementDirection;
        //    Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        //}

        AddReward(-0.01f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall" || collision.collider.tag == "Water" || collision.collider.tag == "Enemy" || collision.collider.tag == "Weapon")
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
