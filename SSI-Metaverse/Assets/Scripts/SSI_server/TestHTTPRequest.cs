using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using JsonClasses;

public class TestHTTPRequest : MonoBehaviour {

    [SerializeField] private string server_veramo = "127.0.0.1:8000";
    [SerializeField] private string server_db = "localhost:3000";

    private async void Start() {
        /*
        using (HttpClient client = new HttpClient()) {

            try {
                
                HttpResponseMessage response = await client.GetAsync("http://" + server_ip_port + "/getVCs?did=did:ethr:sepolia:0x03d3fba90b3fef0d6cdf359d7d72d5649125d9595029f2f04951c47d0ff0a6c9f1");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                
                Debug.Log(responseBody);

                IdentityVerfiableCredential idCredential = JsonUtility.FromJson<IdentityVerfiableCredential>(responseBody);
                Debug.Log(idCredential.credentialSubject.id);
            }
            catch (HttpRequestException e) {
                Debug.Log("Error during http request : " + e);
            }
        }
        */
    }

    public async void MakeVCRequest() {
        using (HttpClient client = new HttpClient()) {

            try {

                HttpResponseMessage response = await client.GetAsync("http://" + server_veramo + "/getVCs?did=did:ethr:sepolia:0x03d3fba90b3fef0d6cdf359d7d72d5649125d9595029f2f04951c47d0ff0a6c9f1");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Debug.Log(responseBody);

                StandardVerifiableCredential idCredential = JsonUtility.FromJson<StandardVerifiableCredential>(responseBody);
                Debug.Log(idCredential.credentialSubject.id);
            }
            catch (HttpRequestException e) {
                Debug.Log("Error during VC retrieving: " + e);
            }
        }
    }

    public async void MakeVPRequest() {
        using (HttpClient client = new HttpClient()) {
            try {
                HttpResponseMessage response = await client.GetAsync("http://" + server_veramo + "/getVP?did=did:ethr:sepolia:0x03d3fba90b3fef0d6cdf359d7d72d5649125d9595029f2f04951c47d0ff0a6c9f1");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Debug.Log(responseBody);

                VerifiablePresentation presentation = JsonUtility.FromJson<VerifiablePresentation>(responseBody);

                Debug.Log($"You have {presentation.verifiableCredentials.Count} verifiable credentials in your wallet");

                foreach (VerifiableCredentialContainer vc_presentation_container in presentation.verifiableCredentials) {
                    StandardVerifiableCredential vc = vc_presentation_container.verifiableCredential;

                    if (vc.type[1] != "DriverLicense") {
                        CredentialSubject credentialSubject = vc.credentialSubject;
                        Debug.Log("Identity or Simple : " + credentialSubject.age);
                    }
                    else {
                        Debug.Log("Driver's license : " + vc.credentialSubject.license);
                    }
                }
            }
            catch(HttpRequestException e) {
                Debug.Log("Error during VP retrieving: " + e);
            }
        }
    }

    public async void MakeDBRequest() {
        using(HttpClient client = new HttpClient()) {
            try {
                HttpResponseMessage response = await client.GetAsync("http://" + server_db + "/users/Lorenzo/default/");
                response.EnsureSuccessStatusCode(); // If status code returned represents an erro, it throws an exception

                // Get here if has not thrown any error
                string responseBody = await response.Content.ReadAsStringAsync();

                if (responseBody == "1") {
                    Debug.Log("Access completed");
                }
            }
            catch(HttpRequestException e) {
                Debug.Log("Error during http request : " + e);
            }
        }
    }
}
