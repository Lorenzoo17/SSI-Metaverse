using UnityEngine;
using System.Linq;
using JsonClasses;
using TMPro;

public class LineRendererTest : MonoBehaviour {

    [SerializeField] private CloudDataAcquisition.CloudDataType cloudDataToRepresent; // Data that you want to visualise through this graph
    [SerializeField] private LineRenderer lineRenderer; // LineRenderer used to represent the data along a line
    [SerializeField] private Vector3[] valuesToInsert; // Values that will be included in the graph
    [SerializeField] private Vector3 scalingValue; // How much strech the line based on the data that must be represented

    [SerializeField] private int valueCurrentlyVisualized; // Which data (in valuesToInsert) is being visualized (index)
    [SerializeField] private int valueToVisualizeEachTime; // Range of values included in x-axis
    [SerializeField] private float updateTime; // Retrieving data rate
    private float currentUpdateTime;

    [SerializeField] private TextMeshProUGUI chartTitle; // Title of the chart

    // Y axis reference values
    [SerializeField] private TextMeshProUGUI yAxis_max;
    [SerializeField] private TextMeshProUGUI yAxis_mid;
    [SerializeField] private TextMeshProUGUI yAxis_min;

    // X axis reference values
    [SerializeField] private TextMeshProUGUI xAxis_min;
    [SerializeField] private TextMeshProUGUI xAxis_mid;
    [SerializeField] private TextMeshProUGUI xAxis_max;

    private void Start() {
        //  Temporarirly data are assigned randomly on start method
        // valuesToInsert = new Vector3[30];
        // for(int i = 0; i < valuesToInsert.Length; i++) {
        //     valuesToInsert[i] = new Vector3(i, Random.Range(0f, 1f), 0f);
        // }

        chartTitle.text = cloudDataToRepresent.ToString();
    }

    private void Update() {
        if(valuesToInsert != null) { // If data has been retrieved from the cloud
            if (currentUpdateTime <= 0) { // Time to show new data (will depend on sensor acquisition rate, so on timestamp)
                currentUpdateTime = updateTime;

                if ((valueCurrentlyVisualized + valueToVisualizeEachTime) < valuesToInsert.Length) { // End of array has not reached yet
                    VisualizeValues(valueCurrentlyVisualized, (valueCurrentlyVisualized + valueToVisualizeEachTime), valueToVisualizeEachTime); // Therefore, lineRenderer is updated
                }
                else {
                    // restart (here a new call is performed and if it works well, through a boolean, the entire cycle repeats)
                    // Moreover it is necessary to assign values, texts and new scalingValues based on new ones
                    valueCurrentlyVisualized = 0;
                    SetValuesFromTimeStamp(CloudDataAcquisition.Instance.GetCloudDataBasedOnType(cloudDataToRepresent));
                    VisualizeValues(valueCurrentlyVisualized, (valueCurrentlyVisualized + valueToVisualizeEachTime), valueToVisualizeEachTime);
                }

                valueCurrentlyVisualized++;
            }
            else {
                currentUpdateTime -= Time.deltaTime;
            }
        }
        else {
            SetValuesFromTimeStamp(CloudDataAcquisition.Instance.GetCloudDataBasedOnType(cloudDataToRepresent)); // Otherwise, try to read data from the cloud again
        }
    }

    public void SetValuesFromTimeStamp(GraphData.TimeStampData[] timestampData) {
        if(timestampData != null) {
            int n_values = timestampData.Length;
            valuesToInsert = new Vector3[n_values];

            float[] values = new float[n_values]; // Values contained in timestampData
            for (int i = 0; i < timestampData.Length; i++) {
                values[i] = timestampData[i].value;
            }
            int[] seconds = new int[n_values]; // Time reference for each value
            for (int i = 0; i < timestampData.Length; i++) {
                seconds[i] = timestampData[i].GetSeconds();
            }

            if(seconds[1] != 0) {
                updateTime = seconds[1] - seconds[0]; // Update time depends on how many time sensors gather data (must be less than one minute for real-time visualisation)
            }

            float maxValue = values.Max();
            float minValue = values.Min();

            // Change y axis references number based on values
            yAxis_max.text = maxValue.ToString();
            yAxis_mid.text = ((maxValue + minValue) / 2).ToString();
            yAxis_min.text = minValue.ToString();

            for (int i = 0; i < values.Length; i++) {
                float normalizedValue = (values[i] - minValue) / (maxValue - minValue);
                valuesToInsert[i] = new Vector3(seconds[i], normalizedValue, 0f); // Update valuesToInsert
            }

            chartTitle.text = cloudDataToRepresent.ToString();
        }
        else {
            Debug.Log("No data has arrived yet");
        }
    }

    // visualization starts from valueCurrentlyVisualized and continues showing the next 5 values
    private void VisualizeValues(int startIndex, int finishIndex, int n_values) {
        lineRenderer.positionCount = n_values;
        for (int i = 0; i < n_values; i++) {
            // just i * scalingValue is enough
            if(valuesToInsert.Length > i + startIndex) {
                Vector3 value = new Vector3((i % n_values) * scalingValue.x, valuesToInsert[i + startIndex].y * scalingValue.y, valuesToInsert[i + startIndex].z * scalingValue.z);
                lineRenderer.SetPosition(i, value);
            }
        }

        // xAxis_max.text = valuesToInsert[startIndex + n_values].x.ToString();
        // xAxis_mid.text = valuesToInsert[startIndex + n_values - (int)(n_values / 2)].x.ToString();
        // xAxis_min.text = valuesToInsert[startIndex].x.ToString();

        xAxis_max.text = (valueCurrentlyVisualized + valueToVisualizeEachTime).ToString();
        xAxis_mid.text = (valueCurrentlyVisualized + (valueToVisualizeEachTime / 2)).ToString();
        xAxis_min.text = valueCurrentlyVisualized.ToString();
    }
}
