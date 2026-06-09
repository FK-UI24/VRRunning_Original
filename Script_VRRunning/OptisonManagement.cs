using UnityEngine;

public class OptisonManagement : MonoBehaviour
{
    [Header("センターマーカー")]
    [SerializeField] private GameObject centerMarkar;
    [Header("フレーム")]
    [SerializeField] private GameObject frame;

    private void Start()
    {
        if (ReadyManagement.UseCenterMarkar)
        {
            centerMarkar.SetActive(true);
        }
        else
        {
            centerMarkar.SetActive(false);
        }
        if (ReadyManagement.UseFrameMarkar)
        {
            frame.SetActive(true);
        }
        else
        {
            frame.SetActive(false);
        }
    }
}
