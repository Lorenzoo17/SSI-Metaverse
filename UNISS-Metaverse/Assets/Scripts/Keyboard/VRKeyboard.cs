using UnityEngine;
using TMPro;
using System;

public class VRKeyboard : MonoBehaviour
{
    public static VRKeyboard Instance { get; set; }

    [SerializeField] private GameObject lettersSide;
    [SerializeField] private GameObject numbersSide;

    [SerializeField] private GameObject[] statusComponents;

    public TMP_InputField inputField;
    public bool maiuscEnabled;

    public event EventHandler OnMaiuscTriggerPressed;
    private Action doThisWhenEnterIsPressed;

    private void Awake() {
        Instance = this;
    }

    public void SetInputField(TMP_InputField in_field) {
        this.inputField = in_field;
    }
    public void AddLetterToInputField(string letter) {
        inputField.text += letter;
    }
    public void RemoveLetterFromInputField() {
        string removed = inputField.text.Remove(inputField.text.Length - 1);
        inputField.text = removed;
    }
    public void ClearInputField() {
        inputField.text = "";
    }
    public string GetInputFieldText() {
        return inputField.text;
    }

    public void ChangeKeyboardSide() {
        if (lettersSide.activeSelf) {
            numbersSide.SetActive(true);
            lettersSide.SetActive(false);
        }
        else if (numbersSide.activeSelf) {
            numbersSide.SetActive(false);
            lettersSide.SetActive(true);
        }
    }
    public void MaiuscTrigger() {
        maiuscEnabled = !maiuscEnabled;
        OnMaiuscTriggerPressed?.Invoke(this, EventArgs.Empty);
    }

    public void TriggerKeyboardStatus(bool status) {
        foreach (GameObject component in statusComponents) {
            component.SetActive(status);
            if (numbersSide.activeSelf) {
                numbersSide.SetActive(false);
            }
        }
    }


    public void SetWhatToDoWhenEnterIsPressed(Action newAction) {
        doThisWhenEnterIsPressed = newAction;
    }
    public void ExecuteEnterAction() {
        doThisWhenEnterIsPressed();
    }
}
