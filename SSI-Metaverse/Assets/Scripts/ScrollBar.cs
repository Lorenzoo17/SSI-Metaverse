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
        if (content.anchoredPosition.y < startPosition.y) { // In order to avoid scrolling past the startPosition
            content.anchoredPosition = startPosition;
        }

        if (content.anchoredPosition.y > GetItemNumber() * itemSize) {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, GetItemNumber() * itemSize); // Scroll position in based on the number of child and their size
        }
    }

    private int GetItemNumber() {
        return content.childCount;
    }
    
    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.name);
        if(other.GetComponent<ResponseVcIcon>() != null) { // If the trigger is touched by one of the buttons
            // Disable interact and visual component in order to avoid seeing the button 
            other.transform.GetComponent<PressableButton>().enabled = false; 
            other.transform.Find("CompressableButtonVisuals").gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<ResponseVcIcon>() != null) { // If the trigger notices that one of the button has exited its collider
            // Enable interact and visual component in order to interact with the button once again
            other.transform.GetComponent<PressableButton>().enabled = true;
            other.transform.Find("CompressableButtonVisuals").gameObject.SetActive(true);
        }
    }
}
