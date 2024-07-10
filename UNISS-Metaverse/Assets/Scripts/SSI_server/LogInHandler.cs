using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LogInHandler : MonoBehaviour {

    // LogIn into Db and Veramo
    [SerializeField] private GameObject logInWindow;
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;

    // Access to metaverse (Server or Client choice)
    [SerializeField] private GameObject startUpWindow;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private ServerLite serverInstance;
    [SerializeField] private ClientLite clientInstance;

    private void Start() {
        usernameField.onSelect.AddListener((x) => {
            VRKeyboard.Instance.TriggerKeyboardStatus(true); // Activate keyboard
            VRKeyboard.Instance.SetInputField(usernameField); // To let user write in the correct textfield

            VRKeyboard.Instance.SetWhatToDoWhenEnterIsPressed(() => {
                UsersDbRequestHandler.Instance.LogInRequest(usernameField.text, passwordField.text, DisableLogInWindow); // When enter in the virtual keyboard is pressed, a LogInRequest is done (the action passed is the DisableLogInWindow, therefore if the login works the login window will be disable
            });
        });
        passwordField.onSelect.AddListener((x) => {
            VRKeyboard.Instance.TriggerKeyboardStatus(true);
            VRKeyboard.Instance.SetInputField(passwordField);

            VRKeyboard.Instance.SetWhatToDoWhenEnterIsPressed(() => {
                UsersDbRequestHandler.Instance.LogInRequest(usernameField.text, passwordField.text, DisableLogInWindow); // When enter in the virtual keyboard is pressed, a LogInRequest is done (the action passed is the DisableLogInWindow, therefore if the login works the login window will be disable
            });
        });

        // Metaverse login
        serverButton.onClick.AddListener(() => {
            serverInstance.StartServer(); // Start a server
            clientInstance.StartClient(); // Start a client too (host)

            DisableConnectionStartUpWindow(); // Disable metavere window
        });

        clientButton.onClick.AddListener(() => {
            clientInstance.StartClient();

            DisableConnectionStartUpWindow();
        });

        DisableConnectionStartUpWindow(); // It is disabled by default
        EnableLogInWindow(); // It is enabled by default
    }

    // Called back in UsersDbRequestHandler.cs -> it is necessary to do it there because the window must be disabled only if the database login is completed
    private void DisableLogInWindow() {
        VRKeyboard.Instance.TriggerKeyboardStatus(false);
        logInWindow.SetActive(false);
    }
    public void EnableLogInWindow() {
        VRKeyboard.Instance.TriggerKeyboardStatus(false);
        logInWindow.SetActive(true);
    }

    public void DisableConnectionStartUpWindow() {
        startUpWindow.SetActive(false);
    }
    public void EnableConnectionStartUpWindow() {
        startUpWindow.SetActive(true);
    }
}
