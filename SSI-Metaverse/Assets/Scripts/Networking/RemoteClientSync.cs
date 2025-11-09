using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteClientSync : MonoBehaviour
{
    public int clientId;

    [SerializeField] private Transform testHead;
    [SerializeField] private Transform testRightArm;
    [SerializeField] private Transform testLeftArm;

    [SerializeField] private PressableButton requestButton; // Pressable button or whatever it is

    [SerializeField] private SkinnedMeshRenderer materialRenderer;
    private void Start() {
        // When a local user clicks the request button can select which data wants from the user connected to the remoteclient
        requestButton.OnClicked.AddListener(() => {
            Debug.Log($"Which data do you want from client : {clientId}");

            SSIUserCommunication.Instance.EnableRequestWindow(clientId); // Enable request window in order to select data to request (clientID specified)
        });
    }

    public void SetHead(Vector3 pos, Quaternion rot) {
        testHead.position = pos;
        testHead.rotation = rot;
    }
    public void SetRightArm(Vector3 pos, Quaternion rot) {
        testRightArm.position = pos;
        testRightArm.rotation = rot;
    }
    public void SetLeftArm(Vector3 pos, Quaternion rot) {
        testLeftArm.position = pos;
        testLeftArm.rotation = rot;
    }


    // ---------- Avatar methods ----------
    public void SetAvatarType(Material materialToSet) {
        Material[] mat = new Material[1];
        mat[0] = materialToSet;

        materialRenderer.materials = mat;
    }
}
