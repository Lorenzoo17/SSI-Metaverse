using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Net.Http;
public class ManageRequest : MonoBehaviour { 

    [SerializeField] private string server_ip_port = "127.0.0.1:8000"; 
    [SerializeField] private Button requestButton;

    private void Start() {
        requestButton.onClick.AddListener(MakeRequest);
    }

    public async void MakeRequest() {
        using (HttpClient client = new HttpClient()) {

            try {

                HttpResponseMessage response = await client.GetAsync("http://" + server_ip_port + "/getVCs?did=did:ethr:sepolia:0x03d3fba90b3fef0d6cdf359d7d72d5649125d9595029f2f04951c47d0ff0a6c9f1");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Debug.Log(responseBody);
            }
            catch (HttpRequestException e) {
                Debug.Log("Error during http request : " + e);
            }
        }
    }
}
