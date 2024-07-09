using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using JsonClasses;

public class TestDrive : MonoBehaviour
{
    [System.Serializable]
    public class DataItem {
        public string timestamp;
        public int value;
    }

    [System.Serializable]
    public class DataOverTime {
        public DataItem[] DataItem;
    } 

    private string url = "https://drive.google.com/uc?export=download&id=1GPQHICEbrS_8KeaMyS_Lo0Clq7AyW2mp";
    [SerializeField] private RawImage testDriveRawImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(GetData(url));
    }

    private IEnumerator GetData(string url) {
        // Json reading
        
        UnityWebRequest request = UnityWebRequest.Get(url);
        
        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.ConnectionError) {
            Debug.Log("Error retrieving data");
        }
        else {
            GraphData dataOverTime = JsonUtility.FromJson<GraphData>(request.downloadHandler.text);

            // Access the data
            foreach (var dataItem in dataOverTime.timestampData) {

                Debug.Log("Timestamp: " + dataItem.GetHours() + ", " + dataItem.GetMinutes() + ", " + dataItem.GetSeconds()  + ", Value: " + dataItem.value);
            }
        }
        

        // Image reading
        /*
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url)) {
            
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success) {
                Debug.Log(request.error);
            }
            else {
                // Get downloaded asset bundle
                var texture = DownloadHandlerTexture.GetContent(request);

                Debug.Log(texture.width + " x " + texture.height);
                testDriveRawImage.texture = texture;
            }
        }
        */
    }
}
