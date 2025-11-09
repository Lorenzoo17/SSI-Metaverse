using UnityEngine;
using System.Net.Http;
using JsonClasses;
using System;

public class UsersDbRequestHandler : MonoBehaviour {
    public static UsersDbRequestHandler Instance { get; set; }

    public event EventHandler OnUserDbLogin;
    [SerializeField] private string server_db = "localhost:3000";
    
    public string alias; // Alias of the current user connected (Alias from db must match the alias from veramo agent!!)
    public bool loggedIn;
    public string GetLoggedInAlias() => alias;
    public bool IsLoggedIn() => loggedIn;

    private void Awake() {
        Instance = this;
    }

    public async void LogInRequest(string alias, string password, System.Action DisableLogInWindow) { // Send a login request to users' database
        using (HttpClient client = new HttpClient()) {
            try {
                HttpResponseMessage response = await client.GetAsync("http://" + server_db + "/users/" + alias + "/" + password+ "/"); // Alias and password in texfields are used as parameters in get request
                response.EnsureSuccessStatusCode(); // If status code returned represents an erro, it throws an exception

                // Get here if has not thrown any error
                string responseBody = await response.Content.ReadAsStringAsync();

                if (responseBody == "1") { // If the response is 1 (everything is ok)
                    Debug.Log("Access completed");

                    this.alias = alias; // Assign new alias
                    loggedIn = true;
                    DisableLogInWindow(); // DisableLogInWindow (from LogInHandler.cs)

                    OnUserDbLogin?.Invoke(this, EventArgs.Empty); // Event that is used in HdtMainMenu.cs

                    SSIRequestHandler.Instance.MakeDidRequest(this.alias); // A Did request is made with the alias the user has written in texfield
                }
            }
            catch (HttpRequestException e) {
                Debug.Log("Error during http request : " + e);
                InfoWindow.Instance.SpawnWindow("Username or password are not correct", 2f);
            }
        }
    }

    public async void MakeDBRequest() { // Just for testing
        using (HttpClient client = new HttpClient()) {
            try {
                HttpResponseMessage response = await client.GetAsync("http://" + server_db + "/users/Lorenzo/default/");
                response.EnsureSuccessStatusCode(); // If status code returned represents an erro, it throws an exception

                // Get here if has not thrown any error
                string responseBody = await response.Content.ReadAsStringAsync();

                if (responseBody == "1") {
                    Debug.Log("Access completed");
                }
            }
            catch (HttpRequestException e) {
                Debug.Log("Error during http request : " + e);
            }
        }
    }
}
