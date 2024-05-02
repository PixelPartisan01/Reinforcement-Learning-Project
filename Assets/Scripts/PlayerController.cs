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

    public GameObject treasure;

    public Transform TargetTransform;
    public Transform Goal;

    private Animator animator;

    private bool checkpont = false;

    private float reward = 0.0f;

    public override void OnEpisodeBegin()
    {
        checkpont = false;
        transform.localPosition = new Vector3(0, 0.5f, 0);
        animator = GetComponent<Animator>();

        //Debug.Log(reward);
        //reward = 0.0f;

        //Debug.Log(transform.localRotation.x + "; " + transform.localRotation.y + "; " + transform.localRotation.z );

        // Generate a random position for the treasure prefab 
        //float xPosition = UnityEngine.Random.Range(-9, 9);
        //float zPosition = UnityEngine.Random.Range(-9, 9);

        TargetTransform.localPosition = new Vector3(25.0f, 0.0f, 3.0f);
        //transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, rotationSpeed * Time.deltaTime);
        //TargetTransform.rotation.Set(0.0f, -90.0f, 0.0f, 0.0f);

        transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

        //transform.eulerAngles = new Vector3(0.0f, UnityEngine.Random.Range(0,181) * UnityEngine.Random.Range(-1, 2), 0.0f);
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
            //buttonPressed = true;
            animator.SetBool("walk", true);
        }
        if (Input.GetKey("d"))
        {
            actions[1] = +0.5f;
            //actions[1] = +rotationSpeed;
            //buttonPressed = true;
            //animator.SetBool("walk", true);
        }
        if (Input.GetKey("a"))
        {
            actions[1] = -0.5f;
            //actions[1] = -rotationSpeed;
            //buttonPressed = true;
            //animator.SetBool("walk", true);
        }

        if(!Input.GetKey("w"))
        {
            animator.SetBool("walk", false);
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

        //reward += -0.01f;
        AddReward(-0.01f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (/*collision.gameObject.CompareTag("Wall") ||*/ collision.gameObject.CompareTag("Water"))
        {
            AddReward(-1.0f);
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.01f);
            //AddReward((1.0f - Vector3.Distance(transform.localPosition, treasure.transform.localPosition) / 100.0f) / 10);
            //EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Treasure"))
        {
            Debug.Log("Treasure");
            SetReward(2.0f);
            EndEpisode();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log(Vector3.Distance(treasure.transform.localPosition, transform.localPosition));
            //Debug.Log();
            AddReward(-1.0f);
            EndEpisode();
        }
    }
}
