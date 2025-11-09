using UnityEngine;
using System.Linq;
using JsonClasses;
using TMPro;

public class Chart3dTest : MonoBehaviour {

    [SerializeField] private CloudDataAcquisition.CloudDataType chartTypeToVisualize; // Data that you want to visualise through this graph

    [SerializeField] private TextMeshPro chartTitle; // reference to the title to show
    [SerializeField] private GameObject manipulationBar;
    [SerializeField] private Transform scalingCube_template; // Template of the cube which is used to represent a piece of data
    [SerializeField] private GameObject scalingCubeText_template; // Text which is used as container for the value the cube will represent
    [SerializeField] private float referenceHeight; // Base scalingCube height
    [SerializeField] private float scalingCubeOffset; // Distance between cubes
    [SerializeField] private float textOffset; // Distance of the text from the cube

    [SerializeField] private float updateChartTime; // Chart update frequency
    private float firstUpdate = 1f;

    private void Start() {
        InvokeRepeating("MakeChart", firstUpdate, updateChartTime);
    }

    private void MakeChart() {
        GraphData.TimeStampData[] timeStampData = CloudDataAcquisition.Instance.GetCloudDataBasedOnType(chartTypeToVisualize); // Get data from the cloud through the CloudDataAcquisition script

        chartTitle.text = chartTypeToVisualize.ToString(); // The title of the chart is the same of the chart type to visualize

        if (timeStampData != null) { // If data from the cloud have been acquired correctly
            ClearOldChart(); // Clear old chart spawned

            float[] valuesToRepresent = new float[timeStampData.Length];
            for (int i = 0; i < valuesToRepresent.Length; i++) {
                valuesToRepresent[i] = timeStampData[i].value; // Get value to represent from the timestamo array
            }

            float startingPosition = -(((float)valuesToRepresent.Length / 2) * scalingCubeOffset); // Starting cube position
            float maxValue = valuesToRepresent.Max(); // Max value a cube has to represent
            float minValue = valuesToRepresent.Min(); // Min value a cube has to represent

            // Generate a cube for each value that must be represented
            for (int i = 0; i < valuesToRepresent.Length; i++) {
                float normalizedValue = (valuesToRepresent[i] - minValue) / (maxValue - minValue); // Get the normalized value [0-1] in order to scale the cube between 0 and 1 based on this value
                GameObject scalingCube = Instantiate(scalingCube_template.gameObject, this.transform); // Spawn the cube
                GameObject scalingCubeText_Value = Instantiate(scalingCubeText_template.gameObject, this.transform); // Spawn text value
                GameObject scalingCubeText_Time = Instantiate(scalingCubeText_template.gameObject, this.transform); // Spawn text time

                scalingCube.SetActive(true);
                scalingCubeText_Value.SetActive(true);
                scalingCubeText_Time.SetActive(true);

                scalingCube.transform.localScale = new Vector3(scalingCube.transform.localScale.x, scalingCube.transform.localScale.y * normalizedValue + 0.01f, scalingCube.transform.localScale.z); // Update cube scale based on the value the cube is associated with
                scalingCube.transform.localPosition = new Vector3(startingPosition + (i * scalingCubeOffset), 0f, 0f); // Change cube position based on the offset and the i-th data it has to represent

                scalingCubeText_Value.transform.localPosition = new Vector3(startingPosition + (i * scalingCubeOffset), scalingCube.transform.localScale.y + textOffset, 0f);
                scalingCubeText_Value.GetComponent<TMPro.TextMeshPro>().text = valuesToRepresent[i].ToString();
                scalingCubeText_Time.transform.localPosition = new Vector3(startingPosition + (i * scalingCubeOffset), -textOffset, textOffset);
                scalingCubeText_Time.GetComponent<TMPro.TextMeshPro>().text = timeStampData[i].GetDay() + "/" + timeStampData[i].GetMonth();
            }
        }
        else {
            Debug.Log(chartTypeToVisualize.ToString() + " data not available yet");
        }
    }

    private void ClearOldChart() {
        foreach(Transform singleChart in this.transform) {
            if(singleChart != scalingCube_template && singleChart.gameObject != scalingCubeText_template && singleChart.gameObject != chartTitle.gameObject && singleChart.gameObject != manipulationBar) {
                Destroy(singleChart.gameObject);
            }
        }
    }
}
