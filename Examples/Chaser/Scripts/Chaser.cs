using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using Unity.MLAgentsExamples;

public class Chaser : Agent {

    [SerializeField] private Transform targetTransform;
    [SerializeField] private float xRange, zRange;
    [SerializeField] private Renderer floorRenderer;
    [SerializeField] private Material success, failure;

    private Rigidbody rigid;
    private Material basic;
    private bool isSucceeded = true;

    public override void Initialize() {
        rigid = GetComponent<Rigidbody>();
        basic = floorRenderer.material;
    }

    private Vector3 GetRandomPosition(float xRange, float zRange) {
        return new Vector3(Random.Range(-xRange, xRange), 0.5f, Random.Range(-zRange, zRange));
    }

    public override void OnEpisodeBegin() {
        if (!isSucceeded) {
            Failure();
        }
        isSucceeded = false;

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        transform.localPosition = GetRandomPosition(xRange, zRange);
        Vector3 targetPosition = GetRandomPosition(xRange, zRange);
        while (Vector3.Distance(targetPosition, transform.localPosition) < 2) {
            targetPosition = GetRandomPosition(xRange, zRange);
        }
        targetTransform.localPosition = targetPosition;
    }

    public override void OnActionReceived(ActionBuffers actions) {
        AddReward(-1f / MaxStep);

        ActionSegment<int> segment = actions.DiscreteActions;
        MoveAgent(segment);
    }

    private void MoveAgent(ActionSegment<int> segment) {
        int d = segment[0];
        int r = segment[1];
        Vector3 direction = Vector3.zero;
        Vector3 rotation = Vector3.zero;

        switch (d) {
            case 1:
                direction = transform.forward;
                break;
            case 2:
                direction = -transform.forward;
                break;
        }
        switch (r) {
            case 1:
                rotation = transform.up;
                break;
            case 2:
                rotation = -transform.up;
                break;
        }

        transform.Rotate(rotation, 100 * Time.deltaTime);
        rigid.AddForce(direction, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Target")) {
            Success();
            EndEpisode();
        }
    }

    private void Success() {
        isSucceeded = true;
        floorRenderer.material = success;
        Invoke("ResetFloor", 1f);
    }

    private void Failure() {
        floorRenderer.material = failure;
        Invoke("ResetFloor", 1f);
    }

    private void ResetFloor() {
        floorRenderer.material = basic;
    }
}
