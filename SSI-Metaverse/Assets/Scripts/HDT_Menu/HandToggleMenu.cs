using MixedReality.Toolkit.UX;
using UnityEngine;
using TMPro;
public class HandToggleMenu : MonoBehaviour
{
    [SerializeField] private GameObject hdtMainMenu;
    [SerializeField] private PressableButton toggleMenuButton; // Here is PressableButton but it can be button or whatever
    [SerializeField] private FontIconSelector iconSelector;
    [SerializeField] private Vector3 mainMenuPositionOffset; // Position offset from mainCamera (player)

    [SerializeField] private string openIconName;
    [SerializeField] private string closeIconName;
    private void Start() {
        iconSelector.name = openIconName;

        toggleMenuButton.OnClicked.AddListener(() => {
            if (hdtMainMenu.gameObject.activeSelf) {
                iconSelector.CurrentIconName = openIconName;

                hdtMainMenu.gameObject.SetActive(false);
            }
            else {
                iconSelector.CurrentIconName = closeIconName;
                hdtMainMenu.gameObject.SetActive(true);
                // Set menu position 
                hdtMainMenu.transform.position = Camera.main.transform.position + Camera.main.transform.forward * mainMenuPositionOffset.z;
                // hdtMainMenu.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
            }
        });
    }

}
