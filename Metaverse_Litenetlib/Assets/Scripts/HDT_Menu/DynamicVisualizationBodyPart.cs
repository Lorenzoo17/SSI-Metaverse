using UnityEngine;
using System.Collections;
using System;
using System.Threading.Tasks;

public class DynamicVisualizationBodyPart : MonoBehaviour {
    
    [SerializeField] private GameObject heartModel;

    /*
    private IEnumerator currentCoroutine; // If I start a visualization I have to stop all the others (?)
    private void Start() {
        heartModel.SetActive(false);
    }

    public void StartHeartVisualization(float frequency) {
        heartModel.SetActive(true);

        currentCoroutine = HeartAnimation(frequency);
        StartCoroutine(currentCoroutine);
    }
    
    private IEnumerator HeartAnimation(float frequency = 0.2f) {
        while (true) {
            heartModel.transform.localScale = new Vector3(heartModel.transform.localScale.x + .1f, heartModel.transform.localScale.y, heartModel.transform.localScale.z);
            yield return new WaitForSeconds(frequency);
            heartModel.transform.localScale = new Vector3(heartModel.transform.localScale.x - .1f, heartModel.transform.localScale.y, heartModel.transform.localScale.z);
        }
    }

    public void StopHeartVisualization() {
        StopCoroutine(currentCoroutine);

        heartModel.SetActive(false);
    }

    private void OnDisable() {
        StopAllCoroutines();
        Debug.Log("All coroutine stopped");
    }
    */
}
