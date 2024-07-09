using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestIkSync : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private Transform right_arm;
    [SerializeField] private Transform left_arm;

    [SerializeField] private Transform test_head;
    [SerializeField] private Transform test_right_arm;
    [SerializeField] private Transform test_left_arm;

    private void Update() {
        test_head.position = head.position;
        test_head.rotation = head.rotation;

        test_right_arm.position = right_arm.position;
        test_right_arm.rotation = right_arm.rotation;

        test_left_arm.position = left_arm.position;
        test_left_arm.rotation = left_arm.rotation;
    }
}
