using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LiteNetLib.Utils;
using LiteNetLib;

public class ConnectionStartUp : MonoBehaviour {

    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;

    [SerializeField] private ServerLite serverInstance;
    [SerializeField] private ClientLite clientInstance;

    [SerializeField] private bool spawnClientWithServer = true;
    private void Start() {
        serverButton.onClick.AddListener(() => {
            serverInstance.StartServer();
            
            if(spawnClientWithServer)
                clientInstance.StartClient();

            DisableConnectionButtons();
        });

        clientButton.onClick.AddListener(() => {
            clientInstance.StartClient();

            DisableConnectionButtons();
        });
    }

    private void DisableConnectionButtons() {
        serverButton.gameObject.SetActive(false);
        clientButton.gameObject.SetActive(false);
    }

    public void StartAsClient() {
        clientInstance.StartClient();
    }
}
