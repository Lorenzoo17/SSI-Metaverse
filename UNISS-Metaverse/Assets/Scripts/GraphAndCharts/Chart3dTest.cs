using UnityEngine;
using System.Linq;
using JsonClasses;
using TMPro;

public class Chart3dTest : MonoBehaviour {

    [SerializeField] private CloudDataAcquisition.CloudDataType chartTypeToVisualize;

    [SerializeField] private TextMeshPro chartTitle;
    [SerializeField] private GameObject manipulationBar;
    [SerializeField] private Transform scalingCube_template;
    [SerializeField] private GameObject scalingCubeText_template;
    [SerializeField] private float referenceHeight;
    [SerializeField] private float scalingCubeOffset;
    [SerializeField] private float textOffset;

    [SerializeField] private float updateChartTime;
    private float firstUpdate = 1f;

    private void Start() {
        InvokeRepeating("MakeChart", firstUpdate, updateChartTime);
    }

    private void MakeChart() {
        GraphData.TimeStampData[] timeStampData = CloudDataAcquisition.Instance.GetCloudDataBasedOnType(chartTypeToVisualize);

        chartTitle.text = chartTypeToVisualize.ToString();

        if (timeStampData != null) {
            ClearOldChart();

            float[] valuesToRepresent = new float[timeStampData.Length];
            for (int i = 0; i < valuesToRepresent.Length; i++) {
                valuesToRepresent[i] = timeStampData[i].value;
            }

            float startingPosition = -(((float)valuesToRepresent.Length / 2) * scalingCubeOffset);
            float maxValue = valuesToRepresent.Max();
            float minValue = valuesToRepresent.Min();

            for (int i = 0; i < valuesToRepresent.Length; i++) {
                float normalizedValue = (valuesToRepresent[i] - minValue) / (maxValue - minValue);
                GameObject scalingCube = Instantiate(scalingCube_template.gameObject, this.transform);
                GameObject scalingCubeText_Value = Instantiate(scalingCubeText_template.gameObject, this.transform);
                GameObject scalingCubeText_Time = Instantiate(scalingCubeText_template.gameObject, this.transform);

                scalingCube.SetActive(true);
                scalingCubeText_Value.SetActive(true);
                scalingCubeText_Time.SetActive(true);

                scalingCube.transform.localScale = new Vector3(scalingCube.transform.localScale.x, scalingCube.transform.localScale.y * normalizedValue + 0.01f, scalingCube.transform.localScale.z);
                scalingCube.transform.localPosition = new Vector3(startingPosition + (i * scalingCubeOffset), 0f, 0f);

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
