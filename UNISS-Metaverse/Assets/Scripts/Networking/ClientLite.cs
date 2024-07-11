using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using Extension;
using JsonClasses;

public class ClientLite : MonoBehaviour {

    private NetManager client;
    private NetPeer serverPeer;
    private EventBasedNetListener listener;

    [SerializeField] private GameObject test_go;

    [SerializeField] private int portToConnect = 45632;
    [SerializeField] private string connectionKey = "key";
    [SerializeField] private string server_ip;

    [SerializeField] private bool enableDebug = true;

    [SerializeField] private Transform clientHead;
    [SerializeField] private Transform clientRightArm;
    [SerializeField] private Transform clientLeftArm;

    private float currentTransmissionTime;

    [SerializeField] private Material[] avatarTypeMaterials;
    private int localClientMaterialIndex;

    public Material GetLocalAvatarType() {
        return avatarTypeMaterials[localClientMaterialIndex];
    }

    public void StartClient() {
        listener = new();
        client = new(listener);

        client.Start();
        client.Connect(server_ip, portToConnect, connectionKey);
        //GameObject test = Instantiate(test_go, new Vector3(0, 10f, 5f), Quaternion.identity);

        listener.PeerConnectedEvent += (peer) => {
            serverPeer = peer;
            if (enableDebug) Debug.Log("Client connected to " + serverPeer.Address + ", " + serverPeer.Port);
            // GameObject test = Instantiate(test_go, new Vector3(0, 10f, 5f), Quaternion.identity);

            // Send Server information about client avatar
            // For now avatar type assigned randomly
            localClientMaterialIndex = Random.Range(0, avatarTypeMaterials.Length);
            SendAvatarType(localClientMaterialIndex);
        };

        listener.NetworkReceiveEvent += (NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod) => {
            NetworkDataType packetType = (NetworkDataType)reader.GetInt(); // Read packetType

            if (packetType == NetworkDataType.PingPacket) {
                int dimension = reader.GetInt(); // Get number of data to read
                byte[] data = new byte[dimension];

                reader.GetBytes(data, dimension);
                if (enableDebug) Debug.Log($"Client has received ping data of lenght {dimension}");
            }
            else if (packetType == NetworkDataType.RemoteVisualClientPacket) {
                int idClientToSync = reader.GetInt(); // Read id to understand which client I have to sync
                reader.GetRemoteVisualSync(out Vector3 headPosition, out Quaternion headRotation, out Vector3 rightArmPosition, out Quaternion rightArmRotation, out Vector3 leftArmPosition, out Quaternion leftArmRotation);

                if (enableDebug) {
                    /*
                    Debug.Log($"Received : {headPosition}" +
                        $"{headRotation}" +
                        $"{rightArmPosition}" +
                        $"{rightArmRotation}" +
                        $"{leftArmPosition}" +
                        $"{leftArmRotation}");
                    */
                }

                RemoteClientSync[] clientsVisuals = FindObjectsOfType<RemoteClientSync>();
                if (clientsVisuals.Length > 0) {
                    foreach (RemoteClientSync client in clientsVisuals) {
                        if (client.clientId == idClientToSync) {
                            // if (enableDebug) Debug.Log($"Found client {client.clientId}");

                            client.SetHead(headPosition, headRotation);
                            client.SetRightArm(rightArmPosition, rightArmRotation);
                            client.SetLeftArm(leftArmPosition, leftArmRotation);
                        }
                    }
                }
            }
            else if (packetType == NetworkDataType.SpawnPlayerRemote) {
                int clientIdToSpawn = reader.GetInt();

                if (enableDebug) Debug.Log($"Need to spawn client with ID : {clientIdToSpawn}");

                GameObject remoteClientPrefab = Instantiate(NetworkPrefabs.Instance.playerRemotePrefab, transform.position, Quaternion.identity);
                if (remoteClientPrefab.TryGetComponent<RemoteClientSync>(out RemoteClientSync remoteClient)) {
                    remoteClient.clientId = clientIdToSpawn;
                }
            }
            else if (packetType == NetworkDataType.DespawnPlayerRemote) {
                int clientIdToDespawn = reader.GetInt();

                if (enableDebug) Debug.Log($"Need to despawn client with ID : {clientIdToDespawn}");


                RemoteClientSync[] remoteClients = FindObjectsOfType<RemoteClientSync>();
                if (remoteClients.Length > 0) {
                    foreach (RemoteClientSync client in remoteClients) {
                        if (client.clientId == clientIdToDespawn) {
                            if (enableDebug) Debug.Log($"Found client {client.clientId}");

                            Destroy(client.gameObject);
                        }
                    }
                }
            }
            else if (packetType == NetworkDataType.VcRequest) {
                int clientWhoSentRequest = reader.GetInt(); // It is added server side
                VerifiableCredentialType credentialAsked = (VerifiableCredentialType)reader.GetInt();

                Debug.Log($"Credential {credentialAsked.ToString()} requested from client {clientWhoSentRequest}");
                SSIUserCommunication.Instance.EnableResponseWindow(clientWhoSentRequest, credentialAsked);

                // Response is sent in SSIUserCommunication.cs
            }
            else if (packetType == NetworkDataType.VcResponse) {
                int clientWhoSentResponse = reader.GetInt();
                JsonClasses.StandardVerifiableCredential vcReceived = reader.GetVerifiableCredential();

                Debug.Log($"Received from client {clientWhoSentResponse} a credential of type : {vcReceived.type[1]}");

                SSIUserCommunication.Instance.SetUpVerifiableCredentialWindow(vcReceived); // In order to show the verifiable credential just received
            }
            else if (packetType == NetworkDataType.VcStringResponse) {
                int clientWhoSentResponse = reader.GetInt();
                string vcStringReceived = reader.GetString();

                SSIUserCommunication.Instance.SetUpVerifiableCredentialWindow(vcStringReceived); // In order to show the verifiable credential just received
            }
            else if (packetType == NetworkDataType.AvatarType) {
                int clientAvatarOwnerID = reader.GetInt();
                int avatarTypeMaterialIndex = reader.GetInt();

                ReceivedAvatarType(clientAvatarOwnerID, avatarTypeMaterialIndex);
            }
        };
    }
    private void Update() {
        if (client != null && client.IsRunning) {
            client.PollEvents();
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            // SendPing();
            // SendRemoteVisualSync();
        }

        if (currentTransmissionTime <= 0) {
            SendRemoteVisualSync();
            currentTransmissionTime = 0.05f;
        }
        else {
            currentTransmissionTime -= Time.deltaTime;
        }
    }

    private void OnDisable() {
        if (client != null && client.IsRunning)
            client.Stop();
    }

    public void StopClient() {
        if (client != null && client.IsRunning) {
            // Despawn all remote players in scene
            RemoteClientSync[] remoteClients = FindObjectsOfType<RemoteClientSync>();
            if (remoteClients.Length > 0) {
                foreach (RemoteClientSync client in remoteClients) {
                    Destroy(client.gameObject);
                    Debug.Log($"Destroyed : {client.gameObject.name}");
                }
            }

            client.Stop();
        }
    }

    // ------------------------- OTHER METHODS -------------------------

    private void SendData(NetDataWriter writer, DeliveryMethod deliveryMethod) {
        if (serverPeer != null && client.IsRunning) {
            serverPeer.Send(writer, deliveryMethod);
            // if (enableDebug) Debug.Log("Client has sent data to server");
        }
        else {
            // if (enableDebug) Debug.Log("Server not found");
        }
    }

    private void SendPing(int packetDimension = 1000) {
        NetworkDataType packetType = NetworkDataType.PingPacket; // Specify type of packet

        byte[] dataToSend = new byte[packetDimension]; // Data to send
        NetDataWriter writer = new NetDataWriter(); // Initialize writer
        writer.Put((int)packetType); // Insert type of packet (to make receiver understand what data has been sent)
        writer.Put(packetDimension); // Dimension
        writer.Put(dataToSend); // Data

        SendData(writer, DeliveryMethod.Unreliable); // Send specifying delivery method
    }

    private void SendRemoteVisualSync() {
        NetworkDataType packetType = NetworkDataType.RemoteVisualClientPacket;

        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)packetType);
        writer.Put(clientHead.position, clientHead.rotation, clientRightArm.position, clientRightArm.rotation, clientLeftArm.position, clientLeftArm.rotation);
        SendData(writer, DeliveryMethod.ReliableUnordered);
    }

    // Method used in SSIUserCommunication in order to ask clientID to send this client the requested vc
    public void SendVcRequestToSpecificClient(VerifiableCredentialType credentialToAsk, int clientID) {
        NetworkDataType packetType = NetworkDataType.VcRequest;

        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)packetType); // packetType
        writer.Put(clientID); // client who will receive the data
        writer.Put((int)credentialToAsk);

        SendData(writer, DeliveryMethod.ReliableUnordered);
    }

    public void SendVcResponseToSpecificClient(int clientID, JsonClasses.StandardVerifiableCredential verifiableCredentialToSend) {
        NetworkDataType packetType = NetworkDataType.VcResponse;

        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)packetType); // packetType
        writer.Put(clientID); // client who will receive the data
        writer.Put(verifiableCredentialToSend); // the full credential

        SendData(writer, DeliveryMethod.ReliableUnordered);
    }

    public void SendVcStringResponseToSpecificClient(int clientID, string vcStringToSend) {
        NetworkDataType packetType = NetworkDataType.VcStringResponse;

        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)packetType); // packetType
        writer.Put(clientID); // client who will receive the data
        writer.Put(vcStringToSend); // the full credential

        SendData(writer, DeliveryMethod.ReliableUnordered);
    }

    // ------------- Avatar methods --------------

    public void SendAvatarType(int avatarIndex) {
        NetworkDataType packetType = NetworkDataType.AvatarType;

        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)packetType);
        writer.Put(avatarIndex);

        SendData(writer, DeliveryMethod.ReliableUnordered);
    }

    public void ReceivedAvatarType(int clientAvatarOwnerID, int avatarMaterialIndex) {
        RemoteClientSync[] remoteClients = FindObjectsOfType<RemoteClientSync>();

        Debug.Log($"Client {clientAvatarOwnerID} has avatar {avatarMaterialIndex}");

        if(remoteClients.Length > 0) {
            foreach(RemoteClientSync client in remoteClients) {
                if(client.clientId == clientAvatarOwnerID) {
                    client.SetAvatarType(avatarTypeMaterials[avatarMaterialIndex]);
                    break;
                }
            }
        }
    }
}
