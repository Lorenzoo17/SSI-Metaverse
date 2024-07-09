using MixedReality.Toolkit.UX;
using UnityEngine;
using UnityEngine.UI;

public class HdtMainMenu : MonoBehaviour {

    [Header("HDT Visualization")]
    [SerializeField] private PressableButton hdtVisual_btn; // Button to open hdt_visualisation_tab (to see user's parameter acquired from sensors dynamic vc visualization)
    [SerializeField] private GameObject hdtVisual_tab;

    [Header("VC Visualization")]
    [SerializeField] private PressableButton vcVisual_btn; // button to open vc_visualisation_tab (to see user's verifiable credentials list)
    [SerializeField] private GameObject vcVisual_tab;

    [Header("Info Visualization")]
    [SerializeField] private PressableButton infoVisual_btn; // button to open info_visualisation_tab (to see user's data like alias or DID)
    [SerializeField] private GameObject infoVisual_tab;

    [Header("LogOut Button")]
    [SerializeField] private PressableButton logOut_btn; // Button to log out (veramo agent stored data will be cleared)

    private void Start() {

        UsersDbRequestHandler.Instance.OnUserDbLogin += (object sender, System.EventArgs e) => { // When the event "OnUserDbLogin" in UserDbRequestHanlder.cs is Invoked, this anonymus method is executed
            // Disable all the windows
            vcVisual_tab.SetActive(false); 
            infoVisual_tab.SetActive(false);
            hdtVisual_tab.SetActive(false);
        };

        hdtVisual_btn.OnClicked.AddListener(() => {
            /*
            if (!hdtVisual_tab.activeSelf) {
                vcVisual_tab.SetActive(false);
                infoVisual_tab.SetActive(false);

                hdtVisual_tab.SetActive(true); // true
            }
            */
            vcVisual_tab.SetActive(false);
            infoVisual_tab.SetActive(false);

            hdtVisual_tab.SetActive(!hdtVisual_tab.activeSelf); // true
        });

        vcVisual_btn.OnClicked.AddListener(() => {
            /*
            if (!vcVisual_tab.activeSelf) {
                hdtVisual_tab.SetActive(false);
                infoVisual_tab.SetActive(false);

                vcVisual_tab.SetActive(true); // true
            }
            */

            hdtVisual_tab.SetActive(false);
            infoVisual_tab.SetActive(false);

            vcVisual_tab.SetActive(!vcVisual_tab.activeSelf); // true

            if (vcVisual_tab.TryGetComponent<VcMenu>(out VcMenu vcMenu)) {
                SSIRequestHandler.Instance.MakeMenuVPRequest(vcMenu.UpdateVcMenuUI); // Make a VP request and then update vcMenuUI 
            }
        });

        infoVisual_btn.OnClicked.AddListener(() => {
            /*
            if (!infoVisual_tab.activeSelf) {
                hdtVisual_tab.SetActive(false);
                vcVisual_tab.SetActive(false);

                infoVisual_tab.SetActive(true); // true
                if (infoVisual_tab.TryGetComponent<AccountInfoWindow>(out AccountInfoWindow infoWindow)) {
                    infoWindow.SetAccountInfoDescription();
                }
            }
            */

            hdtVisual_tab.SetActive(false);
            vcVisual_tab.SetActive(false);

            infoVisual_tab.SetActive(!infoVisual_tab.activeSelf); // true
            if (infoVisual_tab.TryGetComponent<AccountInfoWindow>(out AccountInfoWindow infoWindow)) {
                infoWindow.SetAccountInfoDescription();
            }
        });

        logOut_btn.OnClicked.AddListener(() => {
            SSIRequestHandler.Instance.UserLogOut(); // Log out
            InfoWindow.Instance.SpawnWindow("User has logged out", 2f);
        });
    }
}
