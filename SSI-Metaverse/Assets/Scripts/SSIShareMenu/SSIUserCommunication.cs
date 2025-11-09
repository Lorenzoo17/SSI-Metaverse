using MixedReality.Toolkit.UX;
using UnityEngine;
using System;
using System.Collections.Generic;
using JsonClasses;
public class SSIUserCommunication : MonoBehaviour {
    
    [Serializable]
    private struct RequestButton {
        public PressableButton button; // Button to click
        public VerifiableCredentialType credential; // Credential associated to that button
    }

    public static SSIUserCommunication Instance { get; set; }

    [SerializeField] private ClientLite localClient;
    [SerializeField] private RequestButton[] requestButtons; // Buttons in request window
    [SerializeField] private Transform requestWindow;

    [SerializeField] private Transform responseWindow;
    [SerializeField] private Transform responseWindowContent;
    [SerializeField] private TMPro.TextMeshPro responseWindowTitle;
    [SerializeField] private Transform responseIcon;
    [SerializeField] private PressableButton responseWindowCloseButton;

    [SerializeField] private VcWindow verifiableCredentialWindow;

    [Header("Selection window")]
    [SerializeField] private Transform selectWindow;
    [SerializeField] private Transform selectWindowContent;
    [SerializeField] private Transform toggleSelectButton;
    [SerializeField] private PressableButton selectWindowSubmitButton;
    [SerializeField] private PressableButton selectWindowCloseButton;
    private string[] credentialSubjectFields = null;
    private List<PressableButton> toggleButtons = new List<PressableButton>();
    private StandardVerifiableCredential currentOriginalVc;

    private int clientToSendRequest;
    private int clientToRespond;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        foreach(RequestButton requestButton in requestButtons) {
            requestButton.button.OnClicked.AddListener(() => {
                Debug.Log($"Pressing this button you will ask for a {requestButton.credential.ToString()} credential");
                localClient.SendVcRequestToSpecificClient(requestButton.credential, clientToSendRequest);

                DisableRequestWindow();
            });
        }

        responseWindowCloseButton.OnClicked.AddListener(() => {
            DisableResponseWindow();
        });

        selectWindowCloseButton.OnClicked.AddListener(() => {
            ClearSelectWindow();
            selectWindow.gameObject.SetActive(false);
        });

        selectWindowSubmitButton.OnClicked.AddListener(() => {
            string newCredentialSubjectString = "";

            for(int i = 0; i < credentialSubjectFields.Length; i++) {
                if (toggleButtons[i].IsToggled) {
                    newCredentialSubjectString += credentialSubjectFields[i] + ",\n";
                }
            }

            Debug.Log("NEW CS : " + newCredentialSubjectString);

            // Delete last comma
            int penultimateCharIndex = newCredentialSubjectString.Length - 2;
            newCredentialSubjectString = newCredentialSubjectString.Remove(penultimateCharIndex, 1);

            string jsonString = currentOriginalVc.GetJsonString(newCredentialSubjectString);
            // Debug.Log(jsonString);
            // StandardVerifiableCredential svc = JsonUtility.FromJson<StandardVerifiableCredential>(jsonString);
            // Debug.Log(svc.issuanceDate);

            localClient.SendVcStringResponseToSpecificClient(clientToRespond, jsonString); // Use ClientLite.cs to send the response
            // Disable select window
            ClearSelectWindow();
            selectWindow.gameObject.SetActive(false);
        });
    }

    public void EnableRequestWindow(int remoteClient) {
        requestWindow.gameObject.SetActive(true);

        clientToSendRequest = remoteClient;
    }
    public void DisableRequestWindow() {
        requestWindow.gameObject.SetActive(false);
    }

    public void EnableResponseWindow(int clientThatMadeRequest, VerifiableCredentialType credentialTypeAsked) { // Calledback in ClientLite.cs when a VcRequest is received
        responseWindow.gameObject.SetActive(true);
        List<StandardVerifiableCredential> verifiableCredentials = SSIRequestHandler.Instance.GetUserVerifiableCredential_list();

        responseWindowTitle.text = $"Client {clientThatMadeRequest} has requested a {credentialTypeAsked} credential. Select the data you want to share";

        foreach(StandardVerifiableCredential vc in verifiableCredentials) {
            if(vc.type[1] == credentialTypeAsked.ToString()) { // For now like in HdtBodyPart the comparison is done between the string format of VerifiableCredentialType
                GameObject responseButtonIcon = Instantiate(responseIcon.gameObject, responseWindowContent);
                responseButtonIcon.SetActive(true);
                responseButtonIcon.GetComponent<ResponseVcIcon>().SetVcInfo(credentialTypeAsked.ToString());

                if (responseButtonIcon.TryGetComponent<PressableButton>(out PressableButton responseButtonIcon_button)) {
                    responseButtonIcon_button.OnClicked.AddListener(() => {
                        Debug.Log($"Send to {clientThatMadeRequest} vc of type {credentialTypeAsked.ToString()}");
                        // localClient.SendVcResponseToSpecificClient(clientThatMadeRequest, vc); // Use ClientLite.cs to send the response --- Uncomment this line if you do not want selective disclosure
                        EnableSelectWindow(vc); // Delete this line if you do not want selective disclosure
                        clientToRespond = clientThatMadeRequest;

                        DisableResponseWindow();
                    });
                }
            }
        }
    }

    private void EnableSelectWindow(StandardVerifiableCredential originalVc) {
        ClearSelectWindow(); // clear before 

        selectWindow.gameObject.SetActive(true);
        credentialSubjectFields = originalVc.credentialSubject.GetFields();
        currentOriginalVc = originalVc;

        foreach (string field in credentialSubjectFields) {
            GameObject toggleButton = Instantiate(toggleSelectButton.gameObject, selectWindowContent);
            toggleButton.SetActive(true);
            toggleButton.GetComponent<ResponseVcIcon>().SetVcInfo(field);
            toggleButtons.Add(toggleButton.GetComponent<PressableButton>());
        }
    }

    public void ClearSelectWindow() {
        currentOriginalVc = null;
        credentialSubjectFields = null;
        toggleButtons.Clear();
        foreach (Transform responseIcon in selectWindowContent) {
            if (responseIcon.TryGetComponent<ResponseVcIcon>(out ResponseVcIcon isAToggleIcon)) {
                if (isAToggleIcon.gameObject.activeSelf) { // In order to avoid deleting the one used as template
                    Destroy(isAToggleIcon.gameObject);
                    // Debug.Log("Destroyed : " + isAResponseVcIcon.gameObject.name);
                }
            }
        }
    }

    public void DisableResponseWindow() {
        foreach(Transform responseIcon in responseWindowContent) {
            if(responseIcon.TryGetComponent<ResponseVcIcon>(out ResponseVcIcon isAResponseVcIcon)) {
                if (isAResponseVcIcon.gameObject.activeSelf) { // In order to avoid deleting the one used as template
                    Destroy(isAResponseVcIcon.gameObject);
                    Debug.Log("Destroyed : " + isAResponseVcIcon.gameObject.name);
                }
            }
        }
        responseWindow.gameObject.SetActive(false);
    }

    public void SetUpVerifiableCredentialWindow(StandardVerifiableCredential verifiableCredentialToShow) {
        verifiableCredentialWindow.gameObject.SetActive(true);
        verifiableCredentialWindow.SetVcText(verifiableCredentialToShow);
    }
    public void SetUpVerifiableCredentialWindow(string verifiableCredentialToShowString) {
        verifiableCredentialWindow.gameObject.SetActive(true);
        StandardVerifiableCredential svc = JsonUtility.FromJson<StandardVerifiableCredential>(verifiableCredentialToShowString);
        // verifiableCredentialWindow.SetVcText(verifiableCredentialToShowString);
        verifiableCredentialWindow.SetVcText(svc);
    }
}
