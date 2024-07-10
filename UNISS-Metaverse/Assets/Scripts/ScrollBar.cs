using MixedReality.Toolkit.UX;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBar : MonoBehaviour {

    [SerializeField] private RectTransform scrollBarObject;
    [SerializeField] private RectTransform content;

    [SerializeField] private float itemSize;
    private Vector2 startPosition;

    private void Start() {
        startPosition = content.anchoredPosition;
    }
    private void Update() {
        if (content.anchoredPosition.y < startPosition.y) {
            content.anchoredPosition = startPosition;
        }

        if (content.anchoredPosition.y > GetItemNumber() * itemSize) {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, GetItemNumber() * itemSize);
        }
    }

    private int GetItemNumber() {
        return content.childCount;
    }
    
    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.name);
        if(other.GetComponent<ResponseVcIcon>() != null) {
            other.transform.GetComponent<PressableButton>().enabled = false;
            other.transform.Find("CompressableButtonVisuals").gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<ResponseVcIcon>() != null) {
            other.transform.GetComponent<PressableButton>().enabled = true;
            other.transform.Find("CompressableButtonVisuals").gameObject.SetActive(true);
        }
    }
}
