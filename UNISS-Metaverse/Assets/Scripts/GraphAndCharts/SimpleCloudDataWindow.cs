using UnityEngine;
using JsonClasses;
using TMPro;

public class SimpleCloudDataWindow : MonoBehaviour {

    [SerializeField] private CloudDataAcquisition.CloudDataType dataTypeToVisualize; // Data type to visualized 
    [SerializeField] private TextMeshPro title;
    [SerializeField] private TextMeshPro valueToShow;

    private void Start() {
        InvokeRepeating("UpdateWindow", 1f, 10f); // Invoke the method UpdateWindow() every 10 seconds
    }

    private void UpdateWindow() {
        GraphData.TimeStampData[] timeStampData = CloudDataAcquisition.Instance.GetCloudDataBasedOnType(dataTypeToVisualize); // Get timestamp data related to the data type chosen to be visualised

        if(timeStampData != null) {
            title.text = dataTypeToVisualize.ToString();
            valueToShow.text = timeStampData[Random.Range(0, (int)timeStampData.Length)].value.ToString();
        }
    }
}
