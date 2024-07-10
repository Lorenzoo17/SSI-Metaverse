using UnityEngine;
using JsonClasses;
using TMPro;

public class SimpleCloudDataWindow : MonoBehaviour {

    [SerializeField] private CloudDataAcquisition.CloudDataType dataTypeToVisualize;
    [SerializeField] private TextMeshPro title;
    [SerializeField] private TextMeshPro valueToShow;

    private void Start() {
        InvokeRepeating("UpdateWindow", 1f, 10f);
    }

    private void UpdateWindow() {
        GraphData.TimeStampData[] timeStampData = CloudDataAcquisition.Instance.GetCloudDataBasedOnType(dataTypeToVisualize);

        if(timeStampData != null) {
            title.text = dataTypeToVisualize.ToString();
            valueToShow.text = timeStampData[Random.Range(0, (int)timeStampData.Length)].value.ToString();
        }
    }
}
