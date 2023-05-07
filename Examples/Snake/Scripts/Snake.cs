using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Snake : Agent {

    [SerializeField] private Transform food;
    [SerializeField] private GameObject bodyPrefab;

    private List<Transform> bodies;
    private Vector3 tailPos;
    private bool die;
    private bool grow;

    public override void Initialize() {
        bodies = new List<Transform>();
        GameObject body = Instantiate(bodyPrefab, transform.localPosition - transform.forward, Quaternion.identity, transform.parent);
        bodies.Add(body.transform);
    }

    public override void OnEpisodeBegin() {
        if (die) {
            foreach (Transform b in bodies) {
                Destroy(b.gameObject);
            }
            bodies.Clear();

            transform.localPosition = Vector3.up * 0.45f;
            grow = false;
            GameObject body = Instantiate(bodyPrefab, transform.localPosition - transform.forward, Quaternion.identity, transform.parent);
            bodies.Add(body.transform);
            die = false;
        }
        FoodTeleport();
    }

    public override void OnActionReceived(ActionBuffers actions) {
        AddReward(-1 / MaxStep);

        tailPos = bodies[bodies.Count - 1].localPosition;
        for (int i = bodies.Count - 1; i >= 0; i--) {
            if (i == 0) {
                bodies[i].localPosition = transform.localPosition;
            } else {
                bodies[i].localPosition = bodies[i - 1].localPosition;
            }
        }

        Vector3 direction = Vector3.zero;
        switch (actions.DiscreteActions[0]) {
            case 0:
                direction = transform.forward;
                break;
            case 1:
                direction = -transform.forward;
                break;
            case 2:
                direction = transform.right;
                break;
            case 3:
                direction = -transform.right;
                break;
        }
        transform.localPosition += direction;

        if (grow)
            Grow();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Food")) {
            grow = true;
            AddReward(1f);
            EndEpisode();
        } else if (other.CompareTag("Wall") || other.CompareTag("Body")) {
            die = true;
            SetReward(-1f);
            EndEpisode();
        }
    }

    private void Grow() {
        GameObject body = Instantiate(bodyPrefab, tailPos, Quaternion.identity, transform.parent);
        bodies.Add(body.transform);
        grow = false;
    }

    private void FoodTeleport() {
        System.Random random = new System.Random();
        int x = random.Next(-14, 15);
        int z = random.Next(-14, 15);
        Vector3 pos = new Vector3(x, 0.45f, z);

        foreach (Transform body in bodies) {
            if (body.localPosition == pos) {
                FoodTeleport();
            }
        }
        food.localPosition = pos;
    }
}
