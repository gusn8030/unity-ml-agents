using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Balancer : Agent {

    [SerializeField] private Transform ballTransform;

    private Rigidbody ballRigid;

    public override void Initialize() {
        ballRigid = ballTransform.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin() {
        ballRigid.velocity = Vector3.zero;
        ballRigid.angularVelocity = Vector3.zero;
        ballTransform.localPosition = new Vector3(Random.Range(-2, 2), 4, Random.Range(-2, 2));

        transform.localEulerAngles = new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));
    }

    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.localRotation); // 4
        sensor.AddObservation(ballTransform.localPosition); // 3
        sensor.AddObservation(ballRigid.velocity); // 3
        sensor.AddObservation(ballRigid.angularVelocity); // 3
    }

    public override void OnActionReceived(ActionBuffers actions) {
        if (ballTransform.localPosition.y < 0) {
            EndEpisode();
        }
        AddReward(1f / MaxStep);

        ActionSegment<float> segment = actions.ContinuousActions;
        RotateAgent(segment);
    }

    private void RotateAgent(ActionSegment<float> segment) {
        float x = segment[0];
        float z = segment[1];

        transform.Rotate(x, 0, z);
    }
}
