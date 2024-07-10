using UnityEngine;
using TMPro;

public class ShowKeyboard : MonoBehaviour {

    private void Start() {
        this.GetComponent<TMP_InputField>().onSelect.AddListener((x) => {
            VRKeyboard.Instance.TriggerKeyboardStatus(true);
        });

        VRKeyboard.Instance.SetWhatToDoWhenEnterIsPressed(() => {
            Debug.Log("Simple action");
        });
    }
}
