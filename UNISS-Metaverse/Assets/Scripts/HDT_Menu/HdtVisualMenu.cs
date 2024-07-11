using UnityEngine;

public class HdtVisualMenu : MonoBehaviour
{
    [SerializeField] private ClientLite localClient;
    [SerializeField] private SkinnedMeshRenderer hdtAvatarVisualRenderer;

    private void OnEnable() {
        Material[] mat = new Material[1];
        mat[0] = localClient.GetLocalAvatarType();
        hdtAvatarVisualRenderer.materials = mat;
    }
}
