using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using JsonClasses;

public class HdtBodyPart : MonoBehaviour {

    // JsonClasses.VerifiableCredentialType is used instead
    /*
    public enum AssociatedCredential {
        Identity,
        DriverLicense,
        HealthCertificate
    }
    */

    [SerializeField] private GameObject descriptionWindow; // Written window
    [SerializeField] private TextMeshPro descriptionWindowText;
    [SerializeField] private VerifiableCredentialType associatedCredential;
    [SerializeField] private DynamicVisualizationBodyPart dynamicVisualizationWindow; // E.g. beating heart

    private ObjectManipulator bodyPartManipulator; // Or any other component related to click event

    private void Awake() {
        bodyPartManipulator = this.GetComponent<ObjectManipulator>();

        bodyPartManipulator.OnClicked.AddListener(() => {
            Debug.Log($"{gameObject.name} clicked");

            descriptionWindow.SetActive(!descriptionWindow.activeSelf);
            
            if(SSIRequestHandler.Instance.GetLoggedInUserDID() != null) {
                List<StandardVerifiableCredential> vc_list = SSIRequestHandler.Instance.GetUserVerifiableCredential_list();

                foreach(StandardVerifiableCredential vc in vc_list) {
                    if(vc.type[1] == associatedCredential.ToString()) {
                        //descriptionWindowText.text = $"Subject : {vc.credentialSubject.you}\nAge : {vc.credentialSubject.age}\nHeartbeat : {vc.credentialSubject.heartbeat}\nSystolic pressure : {vc.credentialSubject.diastolicPressure}\nDiastolic pressure : {vc.credentialSubject.systolicPressure}";
                        ShowBasedOnCredential(vc);
                        Debug.Log("Correct VC found");

                        // dynamicVisualizationWindow.StartHeartVisualization(2f);
                        break;
                    }
                }
            }
        });
    }

    private void ShowBasedOnCredential(StandardVerifiableCredential vc) {
        descriptionWindowText.text = vc.credentialSubject.GetSpecificContent();
        /*
        if(associatedCredential == VerifiableCredentialType.Identity) {
            descriptionWindowText.text = $"Subject : {vc.credentialSubject.you}\nAge : {vc.credentialSubject.age}\n";
        }
        else if(associatedCredential == VerifiableCredentialType.DriverLicense) {
            descriptionWindowText.text = $"Subject : {vc.credentialSubject.you}\nAge : {vc.credentialSubject.age}\nLicense : {vc.credentialSubject.license}";
        }
        else if(associatedCredential == VerifiableCredentialType.HealthCertificate) {
            descriptionWindowText.text = $"Subject : {vc.credentialSubject.you}\nHeartbeat : {vc.credentialSubject.heartbeat}\nSystolic pressure : {vc.credentialSubject.diastolicPressure}\nDiastolic pressure : {vc.credentialSubject.systolicPressure}";
        }
        */
    }
}
