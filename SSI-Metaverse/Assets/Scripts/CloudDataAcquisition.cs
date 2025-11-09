using UnityEngine;
using JsonClasses;
using System.Collections;
using UnityEngine.Networking;
using System;

public class CloudDataAcquisition : MonoBehaviour {
    public static CloudDataAcquisition Instance { get; set; } // Singleton pattern

    // Type of data to read from the cloud
    public enum CloudDataType {
        bodyTemperature,
        noiseLevel,
        sleepDuration,
        bloodOxygen
    }

    // Link to cloud's resources
    [Header("Links")]
    private string bodyTemperatureUrl = "https://drive.google.com/uc?export=download&id=1-XWrMwG759JkQ_QqJDtfSBXZGiZ2tWdh";
    private string noiseLevelUrl = "https://drive.google.com/uc?export=download&id=1GPQHICEbrS_8KeaMyS_Lo0Clq7AyW2mp";
    private string sleepDurationUrl = "https://drive.google.com/uc?export=download&id=1OorOZ5wYp4hrKbPACNWiC-fgawfKVUYu";
    private string bloodOxygenUrl = "https://drive.google.com/uc?export=download&id=1FoAXOGG9HIIemA5K5vSZGCf16Cb8nk4F";

    [SerializeField] private float cloudQueryRate; // Cloud query frequency
    private float cloudQueryCurrentTime; // Current cloud query frequency
    private GraphData.TimeStampData[] bodyTemperatureData; // Collection of body temperature data (array of timestamp)
    private GraphData.TimeStampData[] noiseLevelData;
    private GraphData.TimeStampData[] sleepDurationData;
    private GraphData.TimeStampData[] bloodOxygenData;

    // Get timestap array based on CloudDataType inserted
    public GraphData.TimeStampData[] GetCloudDataBasedOnType(CloudDataType type) {
        if(type == CloudDataType.bodyTemperature) {
            return bodyTemperatureData;
        }
        else if(type == CloudDataType.noiseLevel) {
            return noiseLevelData;
        }
        else if(type == CloudDataType.sleepDuration) {
            return sleepDurationData;
        }
        else if(type == CloudDataType.bloodOxygen) {
            return bloodOxygenData;
        }
        else {
            return bodyTemperatureData; // default
        }
    }

    private void Awake() {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        //StartCoroutine(GetData(bodyTemperatureUrl));
    }

    private void Update() {
        if (cloudQueryCurrentTime <= 0) { // Every cloud query rate

            StopAllCoroutines(); // Stop all previous coroutines
            // Start all coroutine associated with the data you want to retrieve from the cloud
            StartCoroutine(GetDataFromCloud(CloudDataType.bodyTemperature, bodyTemperatureUrl));
            StartCoroutine(GetDataFromCloud(CloudDataType.bloodOxygen, bloodOxygenUrl));
            StartCoroutine(GetDataFromCloud(CloudDataType.noiseLevel, noiseLevelUrl));
            StartCoroutine(GetDataFromCloud(CloudDataType.sleepDuration, sleepDurationUrl));

            cloudQueryCurrentTime = cloudQueryRate;
        }
        else {
            cloudQueryCurrentTime -= Time.deltaTime;
        }
    }

    private IEnumerator GetDataFromCloud(CloudDataType dataType, string url) {
        // Json reading

        UnityWebRequest request = UnityWebRequest.Get(url); // Url of the cloud resource you want to request

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError) {
            Debug.Log("Error retrieving data");
        }
        else {
            GraphData dataOverTime = JsonUtility.FromJson<GraphData>(request.downloadHandler.text); // Read data as JSON based on GraphData format

            // Assing data to an array based on the cloudDataType used
            if (dataType == CloudDataType.bodyTemperature) {
                bodyTemperatureData = dataOverTime.timestampData;
            }
            else if (dataType == CloudDataType.noiseLevel) {
                noiseLevelData = dataOverTime.timestampData;
            }
            else if (dataType == CloudDataType.sleepDuration) {
                sleepDurationData = dataOverTime.timestampData;
            }
            else if (dataType == CloudDataType.bloodOxygen) {
                bloodOxygenData = dataOverTime.timestampData;
            }
            else {
                bodyTemperatureData = dataOverTime.timestampData; // default
            }
        }
    }
}
