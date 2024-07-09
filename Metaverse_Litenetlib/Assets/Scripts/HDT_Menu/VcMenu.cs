using UnityEngine;
using JsonClasses;
using System.Collections.Generic;
using UnityEngine.UI;

public class VcMenu : MonoBehaviour {

    [SerializeField] private GameObject vcIconPrefab;
    [SerializeField] private Transform vcMenuContent;
    [SerializeField] private Vector3 vcIconStartingPosition;
    [SerializeField] private Vector3 vcIconOffset;
    [SerializeField] private int vcInRow;

    [SerializeField] private VcWindow vcWindow;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.L)) { // Testing
            // UpdateVcMenuUI();
        }
    }

    public void UpdateVcMenuUI() {
        if(SSIRequestHandler.Instance != null) {
            
            if(SSIRequestHandler.Instance.GetUserVerifiableCredential_list().Count > 0) { // If number of credentials is greater than 0
                List<StandardVerifiableCredential> vc_list = SSIRequestHandler.Instance.GetUserVerifiableCredential_list();

                // SpawnVerifiableCredentialIcon(vc_list);
                RefreshVerifiableCredentials(vc_list); // Update Menu
            }
            else {
                Debug.Log("No VC available");

                // Reset UI if not logged In
                for (int i = 0; i < vcMenuContent.childCount; i++) {
                    if (vcMenuContent.GetChild(i).TryGetComponent<VcIcon>(out VcIcon vcIcon)) {
                        Destroy(vcIcon.gameObject);
                    }
                }

                // GameObject vcIcon_go = Instantiate(vcIconPrefab, vcMenuContent);
            }
        }
        else {
            Debug.Log("SSI Handler not found");
        }
    }

    private void RefreshVerifiableCredentials(List<StandardVerifiableCredential> vc_list) {
        for(int i = 0; i < vcMenuContent.childCount; i++) { // Check each element in MenuContent and if an element with VcIcon script attached is found, it will be destroyed
            if(vcMenuContent.GetChild(i).TryGetComponent<VcIcon>(out VcIcon vcIcon)) {
                Destroy(vcIcon.gameObject);
            }
        }

        int numberOfIconToSpawn = vc_list.Count;
        // New Icons are spawned
        for (int i = 0; i < numberOfIconToSpawn; i++) {
            GameObject vcIcon = Instantiate(vcIconPrefab, vcMenuContent);
            vcIcon.GetComponent<VcIcon>().SetUpVcIcon(vc_list[i].type[1], vc_list[i], vcWindow);
        }
    }

    private void SpawnVerifiableCredentialIcon(List<StandardVerifiableCredential> vc_list) {

        int numberOfIconToSpawn = vc_list.Count;
        int rowCount = 0;
        int colCount = 0;
        for(int i = 0; i < numberOfIconToSpawn; i++) {
            if(rowCount >= vcInRow) {
                rowCount = 0;
                colCount++;
            }

            GameObject vcIcon = Instantiate(vcIconPrefab, vcMenuContent);
            float posX = vcIconStartingPosition.x + rowCount * vcIconOffset.x;
            float posY = vcIconStartingPosition.y + colCount * vcIconOffset.y;
            // float posZ = vcIconStartingPosition.z;
            vcIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
            vcIcon.GetComponent<VcIcon>().SetUpVcIcon(vc_list[i].type[1], vc_list[i], vcWindow);

            rowCount++;
        }
    }
}
