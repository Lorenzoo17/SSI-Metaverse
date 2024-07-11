using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using Extension;

public class ServerLite : MonoBehaviour {
    private NetManager server; // Server object
    private EventBasedNetListener listener; // Listener to manage all the incoming requests

    [SerializeField] private int port = 45632; // Port where the connection will start
    [SerializeField] private string connectionKey = "key"; // Key to enter into the lobby
    [SerializeField] private int maxNumberOfClients = 10;

    [SerializeField] private bool enableDebug = true;

    private List<NetPeer> peersConnected = new List<NetPeer>();

    private Dictionary<int, int> avatarTypeForEachClient = new Dictionary<int, int>();
    public void StartServer() {
        listener = new();
        server = new NetManager(listener); // Create netManager passing the listener

        server.Start(port); // Server starts with its own ip and with the specialized port
        if (enableDebug) Debug.Log($"Server started listening from port : {port}");

        listener.ConnectionRequestEvent += (request) => {
            if (server.ConnectedPeersCount < maxNumberOfClients) {
                request.AcceptIfKey(connectionKey);
            }
            else {
                request.Reject();
            }
        };

        listener.PeerConnectedEvent += (NetPeer peer) => {
            if (enableDebug) Debug.Log($"New peer {peer.Id} connected!");

            // Add new connected client to the peersConnected list
            if (!peersConnected.Contains(peer))
                peersConnected.Add(peer);

            // Spawn player prefab for other clients connected
            SpawnPlayerRemotePrefab(peer);
            // Spawn all clients prefab to the new client connected
            SpawnOtherPlayerPrefabs(peer);
        };

        listener.PeerDisconnectedEvent += (NetPeer peer, DisconnectInfo disconnectInfo) => {
            if (enableDebug) Debug.Log($"Peer {peer.Id} disconnected for reasons : {disconnectInfo.Reason}");

            // Remove client from peersConnected list
            if (peersConnected.Contains(peer))
                peersConnected.Remove(peer);

            // Despawn playerRemotePrefab from other clients
            DespawnPlayerRemotePrefab(peer);
        };

        listener.NetworkReceiveEvent += (NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod) => {
            NetworkDataType packetType = (NetworkDataType)reader.GetInt(); // Read packetType

            if (packetType == NetworkDataType.PingPacket) {
                int dimension = reader.GetInt(); // Get number of data to read
                byte[] data = new byte[dimension];

                reader.GetBytes(data, dimension);
                if (enableDebug) Debug.Log($"Server has received ping data from client n� {peer.Id} of lenght {dimension}");

                // Send data to all the other clients
            }
            else if (packetType == NetworkDataType.RemoteVisualClientPacket) {
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
                        if (client.clientId == peer.Id) {
                            // if (enableDebug) Debug.Log($"Found client {client.clientId}");

                            client.SetHead(headPosition, headRotation);
                            client.SetRightArm(rightArmPosition, rightArmRotation);
                            client.SetLeftArm(leftArmPosition, leftArmRotation);
                        }
                    }
                }
                // Forward to all clients
                ForwardRemoteVisualSync(peer.Id, headPosition, rightArmPosition, leftArmPosition, headRotation, rightArmRotation, leftArmRotation);
            }
            else if (packetType == NetworkDataType.VcRequest) {
                int clientToSendRequest = reader.GetInt();
                JsonClasses.VerifiableCredentialType credentialToAsk = (JsonClasses.VerifiableCredentialType)reader.GetInt();

                if (enableDebug) Debug.Log($"Send {credentialToAsk.ToString()} request to {clientToSendRequest}");

                foreach (NetPeer client in peersConnected) {
                    if (client.Id == clientToSendRequest) { // Looking for the client with the same ID of the one whose vc is requested

                        int clientWhoSentRequest = peer.Id; // CLIENT WHO SENT THE REQUEST!!!!!!!!!!
                        NetDataWriter writer = new NetDataWriter();
                        writer.Put((int)packetType); // packetType
                        writer.Put(clientWhoSentRequest); // CLIENT WHO SENT THE REQUEST!!!!!!!!!!
                        writer.Put((int)credentialToAsk);

                        SendDataClient(client, writer, DeliveryMethod.ReliableUnordered); // Send data only to the requested client
                    }
                }
            }
            else if (packetType == NetworkDataType.VcResponse) {
                int clientToForwardData = reader.GetInt();
                JsonClasses.StandardVerifiableCredential vcToForward = reader.GetVerifiableCredential();

                int clientWhoSentResponse = peer.Id;
                NetDataWriter writer = new();
                writer.Put((int)packetType);
                writer.Put(clientWhoSentResponse);
                writer.Put(vcToForward);

                foreach (NetPeer client in peersConnected) {
                    if (client.Id == clientToForwardData) {
                        SendDataClient(client, writer, DeliveryMethod.ReliableUnordered);
                    }
                }
            }
            else if (packetType == NetworkDataType.VcStringResponse) {
                int clientToForwardData = reader.GetInt();
                string vcString = reader.GetString();

                int clientWhoSentResponse = peer.Id;
                NetDataWriter writer = new();
                writer.Put((int)packetType);
                writer.Put(clientWhoSentResponse);
                writer.Put(vcString);

                foreach (NetPeer client in peersConnected) {
                    if (client.Id == clientToForwardData) {
                        SendDataClient(client, writer, DeliveryMethod.ReliableUnordered);
                    }
                }
            }
            else if (packetType == NetworkDataType.AvatarType) {
                int clientAvatarOwnerID = peer.Id;
                int avatarTypeMaterialIndex = reader.GetInt();

                if (enableDebug) Debug.Log($"Client {clientAvatarOwnerID} has avatar of type {avatarTypeMaterialIndex}");

                if (avatarTypeForEachClient.ContainsKey(clientAvatarOwnerID)) {
                    avatarTypeForEachClient.Remove(clientAvatarOwnerID);
                }
                avatarTypeForEachClient.Add(clientAvatarOwnerID, avatarTypeMaterialIndex);

                foreach(NetPeer client in peersConnected) {
                    if(client.Id != clientAvatarOwnerID) {
                        ForwardAvatarType(clientAvatarOwnerID, client, avatarTypeMaterialIndex);

                        ForwardAvatarType(client.Id, peer, avatarTypeForEachClient[client.Id]);
                    }
                }
            }
        };
    }

    private void Update() {
        if (server != null && server.IsRunning) {
            server.PollEvents();
        }
    }

    private void OnDisable() {
        if (server != null && server.IsRunning)
            server.Stop();
    }

    public void StopServer() {
        if (server != null && server.IsRunning) {
            // Despawn all remote players in scene
            RemoteClientSync[] remoteClients = FindObjectsOfType<RemoteClientSync>();
            if (remoteClients.Length > 0) {
                foreach (RemoteClientSync client in remoteClients) {
                    Destroy(client.gameObject);
                    Debug.Log($"Destroyed : {client.gameObject.name}");
                }
            }
            server.Stop();
        }
    }

    // -------------------- OTHER METHODS ------------------

    // -------------------- Send methods
    private void SendDataClient(NetPeer peerToSend, NetDataWriter writer, DeliveryMethod deliveryMethod) {
        if (peerToSend != null) {
            peerToSend.Send(writer, deliveryMethod);
        }
    }
    private void SendDataAllClients(NetDataWriter writer, DeliveryMethod deliveryMethod) {
        foreach (NetPeer client in peersConnected) {
            SendDataClient(client, writer, deliveryMethod);
        }
    }
    private void SendSpawnPlayerRemote(NetPeer peerToSend, int idClientToSpawn) {
        NetworkDataType packetType = NetworkDataType.SpawnPlayerRemote;

        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)packetType);
        writer.Put(idClientToSpawn);

        SendDataClient(peerToSend, writer, DeliveryMethod.Unreliable);
    }
    private void SendDespawnPlayerRemote(NetPeer peerToSend, int idClientToDespawn) {
        NetworkDataType packetType = NetworkDataType.DespawnPlayerRemote;

        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)packetType);
        writer.Put(idClientToDespawn);

        SendDataClient(peerToSend, writer, DeliveryMethod.Unreliable);
    }

    // -------------------- Spawn methods
    // Spawn playerRemote prefab of the specified client to all the other clients
    private void SpawnPlayerRemotePrefab(NetPeer clientWhosePrefabMustBeSpawned) {
        int idPrebaToSpawn = clientWhosePrefabMustBeSpawned.Id;

        foreach (NetPeer client in peersConnected) {
            if (client != clientWhosePrefabMustBeSpawned) {
                SendSpawnPlayerRemote(client, idPrebaToSpawn);
            }
        }
    }
    // Spawn all the connected client prefabs to the client specified
    private void SpawnOtherPlayerPrefabs(NetPeer clientWhereToSpawn) {
        foreach (NetPeer client in peersConnected) {
            if (client != clientWhereToSpawn) {
                int id = client.Id;
                SendSpawnPlayerRemote(clientWhereToSpawn, id);
            }
        }
    }
    // Despawn playerRemotePrefab specified to all the clients
    private void DespawnPlayerRemotePrefab(NetPeer clientWhosePrefabMustBeDespawned) {
        int idPrebaToDespawn = clientWhosePrefabMustBeDespawned.Id;

        foreach (NetPeer client in peersConnected) {
            if (client != clientWhosePrefabMustBeDespawned) {
                SendDespawnPlayerRemote(client, idPrebaToDespawn);
            }
        }
    }

    // -------------------- Sync methods
    private void ForwardRemoteVisualSync(int idClientToSync, Vector3 headPos, Vector3 rightArmPos, Vector3 leftArmPos, Quaternion headRotation, Quaternion rightArmRotation, Quaternion leftArmRotation) {
        NetworkDataType packetType = NetworkDataType.RemoteVisualClientPacket;

        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)packetType);
        writer.Put(idClientToSync);
        writer.Put(headPos, headRotation, rightArmPos, rightArmRotation, leftArmPos, leftArmRotation);
        SendDataAllClients(writer, DeliveryMethod.ReliableUnordered);
    }

    // -------------------- Avatar methods

    private void ForwardAvatarType(int clientAvatarOwnerID, NetPeer clientWhoWillReceivedAvatarType, int AvatarTypeIndex) {
        NetworkDataType packetType = NetworkDataType.AvatarType;

        NetDataWriter writer = new();
        writer.Put((int)packetType);
        writer.Put(clientAvatarOwnerID);
        writer.Put(AvatarTypeIndex);

        SendDataClient(clientWhoWillReceivedAvatarType, writer, DeliveryMethod.ReliableUnordered);
    }
}
