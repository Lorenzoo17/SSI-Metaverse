using UnityEngine;

public class ResponseVcIcon : MonoBehaviour {

    [SerializeField] private TMPro.TextMeshPro vcInfo;

    public void SetVcInfo(string text) {
        vcInfo.text = text;
    }
}
