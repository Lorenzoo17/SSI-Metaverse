using UnityEngine;
using TMPro;
using JsonClasses;
using MixedReality.Toolkit.UX;

public class VcIcon : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI vcIconTypeText;
    [SerializeField] private StandardVerifiableCredential associatedCredential;
    private VcWindow vcWindow;

    public void SetUpVcIcon(string typeDescription, StandardVerifiableCredential vc, VcWindow vcWindow) {
        vcIconTypeText.text = typeDescription;
        associatedCredential = vc;
        this.vcWindow = vcWindow;

        if(this.TryGetComponent<PressableButton>(out PressableButton pb)) {
            pb.OnClicked.AddListener(() => {
                vcWindow.gameObject.SetActive(true);
                vcWindow.SetVcText(associatedCredential);
            });
        }
    }
}
