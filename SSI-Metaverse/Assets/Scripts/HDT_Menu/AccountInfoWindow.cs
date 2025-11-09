using UnityEngine;
using TMPro;
public class AccountInfoWindow : MonoBehaviour {

    [SerializeField] private TextMeshPro infoDescription;

    public void SetAccountInfoDescription() {
        infoDescription.text = $"Account Name: {SSIRequestHandler.Instance.GetLoggedInUserAlias()}\nUser DID: {SSIRequestHandler.Instance.GetLoggedInUserDID()}";
    }
}
