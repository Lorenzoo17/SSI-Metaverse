using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardButton : MonoBehaviour
{
    public enum ButtonType {
        standardButton,
        MaiuscButton,
        ChangeSideButton,
        DeleteButton,
        CloseKeyboardButton,
        EnterButton
    }

    [SerializeField] private ButtonType buttonType;
    [SerializeField] private string buttonValue;
    private string currentButtonValue;

    private void Start() {
        currentButtonValue = buttonValue;
        ChangeLetterAppearance();

        VRKeyboard.Instance.OnMaiuscTriggerPressed += Instance_OnMaiuscTriggerPressed;

        if(buttonType == ButtonType.standardButton) {
            this.GetComponent<Button>().onClick.AddListener(() => {
                if (VRKeyboard.Instance != null) {
                    VRKeyboard.Instance.AddLetterToInputField(currentButtonValue);
                }
            });
        }
        else if(buttonType == ButtonType.MaiuscButton) {
            this.GetComponent<Button>().onClick.AddListener(() => {
                if (VRKeyboard.Instance != null) {
                    VRKeyboard.Instance.MaiuscTrigger();
                }
            });
        }
        else if(buttonType == ButtonType.ChangeSideButton) {
            this.GetComponent<Button>().onClick.AddListener(() => {
                if (VRKeyboard.Instance != null) {
                    VRKeyboard.Instance.ChangeKeyboardSide();
                }
            });
        }
        else if (buttonType == ButtonType.DeleteButton) {
            this.GetComponent<Button>().onClick.AddListener(() => {
                if (VRKeyboard.Instance != null) {
                    VRKeyboard.Instance.RemoveLetterFromInputField();
                }
            });
        }
        else if (buttonType == ButtonType.CloseKeyboardButton) {
            this.GetComponent<Button>().onClick.AddListener(() => {
                if (VRKeyboard.Instance != null) {
                    VRKeyboard.Instance.TriggerKeyboardStatus(false);
                }
            });
        }
        else if (buttonType == ButtonType.EnterButton) {
            this.GetComponent<Button>().onClick.AddListener(() => {
                if (VRKeyboard.Instance != null) {
                    VRKeyboard.Instance.ExecuteEnterAction();
                }
            });
        }
    }

    private void Instance_OnMaiuscTriggerPressed(object sender, System.EventArgs e) {
        if (VRKeyboard.Instance.maiuscEnabled) {
            currentButtonValue = buttonValue.ToUpper();
        }
        else {
            currentButtonValue = buttonValue;
        }

        ChangeLetterAppearance();
    }

    private void ChangeLetterAppearance() {
        if (buttonType == ButtonType.standardButton) {
            if (this.GetComponentInChildren<TextMeshProUGUI>() != null) {
                this.GetComponentInChildren<TextMeshProUGUI>().text = currentButtonValue;
            }
        }
    }
}
