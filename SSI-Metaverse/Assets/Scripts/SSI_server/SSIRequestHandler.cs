using UnityEngine;
using System.Net.Http;
using JsonClasses;
using System.Collections.Generic;

public class SSIRequestHandler : MonoBehaviour {

    public static SSIRequestHandler Instance { get; set; } // For singleton pattern

    [SerializeField] private LogInHandler logInHandlerWindow; // Window containing username and password field
    [SerializeField] private string server_veramo = "127.0.0.1:8000"; // Ip address of veramo server (veramo agent)
    public string loggedInUserDID = null; // Contains DID of the current user
    public string userAlias = null; // Contains current user's alias
    private List<StandardVerifiableCredential> userVerifiableCredential_list = new List<StandardVerifiableCredential>(); // Current user's verifiable credentials

    [SerializeField] private ClientLite clientLocal; // ClientLite reference (must be in the scene)
    [SerializeField] private ServerLite serverLocal; // ServerLite reference (must be in the scene)

    public int n_vcs; // Number of verifiable credentials of the current user

    // --- GET AND SET METHODS
    public string GetLoggedInUserDID() => loggedInUserDID;
    public string GetLoggedInUserAlias() => userAlias;
    public List<StandardVerifiableCredential> GetUserVerifiableCredential_list() => userVerifiableCredential_list;

    public void UserLogOut() { // Method used to disconnect the current user
        loggedInUserDID = null; // DID and alias = null
        userAlias = null;
        userVerifiableCredential_list.Clear(); // Remove all the elements in vc list
        n_vcs = 0;

        logInHandlerWindow.EnableLogInWindow();

        clientLocal.StopClient();
        serverLocal.StopServer();
    }

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            if(userVerifiableCredential_list.Count > 0) {
                Debug.Log(JsonUtility.ToJson(userVerifiableCredential_list[0]));
            }
        }
    }

    // -------------------- METHODS USED TO GATHER VCs AND DID FROM VERAMO AGENT----------------------
    public async void MakeDidRequest(string alias) { // Called back in UserDbRequestHandler.cs after accessing to database
        using (HttpClient client = new HttpClient()) {

            try {

                HttpResponseMessage response = await client.GetAsync("http://" + server_veramo + "/getDID?name=" + alias); // Get request
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync(); // Response from get 

                Debug.Log(responseBody);

                if(responseBody != "-1") { // If the alias does not exist in veramo agent the GET method returns -1, otherwise the method returns a DID
                    // Update alias and DID
                    loggedInUserDID = responseBody;
                    userAlias = alias;

                    Debug.Log(loggedInUserDID);

                    MakeVPRequest(); // A startup vp request is made (in this way HdtBodyPart.cs is updated too)
                    logInHandlerWindow.EnableConnectionStartUpWindow(); // Enable startUp window, user can now enter the metaverse
                }
                else { // Associated DID does not exist
                    logInHandlerWindow.EnableLogInWindow(); // User can repeat the login 
                    InfoWindow.Instance.SpawnWindow("Username or password are not correct", 2f);
                }
            }
            catch (HttpRequestException e) {
                Debug.Log("Error during Did retrieving: " + e);
            }
        }
    }

    public async void MakeVCRequest() {
        using (HttpClient client = new HttpClient()) {

            try {

                HttpResponseMessage response = await client.GetAsync("http://" + server_veramo + "/getVCs?did=did:ethr:sepolia:0x03d3fba90b3fef0d6cdf359d7d72d5649125d9595029f2f04951c47d0ff0a6c9f1");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Debug.Log(responseBody);

                StandardVerifiableCredential idCredential = JsonUtility.FromJson<StandardVerifiableCredential>(responseBody);
                // Debug.Log(idCredential.credentialSubject.id);
            }
            catch (HttpRequestException e) {
                Debug.Log("Error during VC retrieving: " + e);
            }
        }
    }

    public async void MakeVPRequest() {
        using (HttpClient client = new HttpClient()) {
            try {
                if(loggedInUserDID != null) {
                    HttpResponseMessage response = await client.GetAsync("http://" + server_veramo + "/getVP?did=" + loggedInUserDID);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Debug.Log(responseBody);

                    VerifiablePresentation presentation = JsonUtility.FromJson<VerifiablePresentation>(responseBody);

                    Debug.Log($"You have {presentation.verifiableCredentials.Count} verifiable credentials in your wallet");

                    userVerifiableCredential_list.Clear(); // Reset current list of verifiable credentials

                    foreach (VerifiableCredentialContainer vc_presentation_container in presentation.verifiableCredentials) {
                        StandardVerifiableCredential vc = vc_presentation_container.verifiableCredential;

                        if (vc.type[1] != "DriverLicense") {
                            CredentialSubject credentialSubject = vc.credentialSubject;
                            // Debug.Log("Identity or Simple : " + credentialSubject.age);
                        }
                        else {
                            // Debug.Log("Driver's license : " + vc.credentialSubject.license);
                        }

                        userVerifiableCredential_list.Add(vc);
                    }

                    n_vcs = userVerifiableCredential_list.Count;
                }
                else {
                    Debug.Log("No user specified");
                }
            }
            catch (HttpRequestException e) {
                Debug.Log("Error during VP retrieving: " + e);
            }
        }
    }

    public async void MakeMenuVPRequest(System.Action updateVcMenu) { // Called back in HdtMainMenu.cs in order to update the vc Menu (the latter contains all the user's verifiable credentials)
        using (HttpClient client = new HttpClient()) {
            try {
                if(loggedInUserDID != null) {
                    HttpResponseMessage response = await client.GetAsync("http://" + server_veramo + "/getVP?did=" + loggedInUserDID);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Debug.Log(responseBody);

                    VerifiablePresentation presentation = JsonUtility.FromJson<VerifiablePresentation>(responseBody);

                    Debug.Log($"You have {presentation.verifiableCredentials.Count} verifiable credentials in your wallet");

                    userVerifiableCredential_list.Clear(); // Reset current list of verifiable credentials

                    foreach (VerifiableCredentialContainer vc_presentation_container in presentation.verifiableCredentials) {
                        StandardVerifiableCredential vc = vc_presentation_container.verifiableCredential;

                        // Just for debugging
                        if (vc.type[1] != "DriverLicense") {
                            CredentialSubject credentialSubject = vc.credentialSubject;
                            // Debug.Log("Identity or Simple : " + credentialSubject.age);
                        }
                        else {
                            // Debug.Log("Driver's license : " + vc.credentialSubject.license);
                        }

                        userVerifiableCredential_list.Add(vc); // Add vcs in the list
                    }

                    n_vcs = userVerifiableCredential_list.Count;

                    updateVcMenu(); // In order to update VcUi consequently
                }
                else {
                    Debug.Log("User not logged");
                    InfoWindow.Instance.SpawnWindow("You are not logged", 2f);
                    updateVcMenu(); // In order to delete saved VC after log out
                }
            }
            catch (HttpRequestException e) {
                Debug.Log("Error during VP retrieving: " + e);
                InfoWindow.Instance.SpawnWindow("Error retrieving new Credentials", 2f);
            }
        }
    }

    // ------------------------------------------
}
