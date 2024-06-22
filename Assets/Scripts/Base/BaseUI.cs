using UnityEngine;
using TMPro;

public class BaseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourcesCount;

    public void DrawResources(int resourcesCount)
    {
        _resourcesCount.text = "Res: " + (resourcesCount).ToString();
    }
}
