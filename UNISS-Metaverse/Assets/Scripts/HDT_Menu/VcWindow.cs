using UnityEngine;
using TMPro;
using MixedReality.Toolkit.UX;
using MixedReality.Toolkit.SpatialManipulation;

public class VcWindow : MonoBehaviour {

    [SerializeField] private TextMeshPro vcText;
    [SerializeField] private bool followingWindow;

    private void Start() {
        if (!followingWindow) {
            if(TryGetComponent<Follow>(out Follow followComponent)) {
                followComponent.enabled = false;
            }
        }
    }

    public void SetVcText(JsonClasses.StandardVerifiableCredential vc) {
        vcText.text = $"Issuer: {vc.issuer.id}\nIssuanceDate: {vc.issuanceDate}\nCredential type: {vc.type[1]}\n{vc.credentialSubject.ToString()}";
    }
    public void SetVcText(string vc) {
        vcText.text = vc;
    }
}
