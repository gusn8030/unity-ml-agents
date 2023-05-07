using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearningManager : MonoBehaviour {

    [Range(0.01f, 1f)] public float fixedTimeStep = 0.02f;

    void Update() {
        Time.fixedDeltaTime = fixedTimeStep;
    }
}
