using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPrefabs : MonoBehaviour { 

    public static NetworkPrefabs Instance { get; set; }

    public GameObject playerRemotePrefab;

    private void Awake() {
        Instance = this;
    }
}
