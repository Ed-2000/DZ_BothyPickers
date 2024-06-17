using UnityEngine;
using TMPro;

public class BaseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourcesCount;

    public TextMeshProUGUI ResourcesCount { get => _resourcesCount; }

    public void DrawResources(int resourcesCount)
    {
        _resourcesCount.text = (resourcesCount).ToString();
    }

    public void Init(TextMeshProUGUI resourcesCountText)
    {
        _resourcesCount = resourcesCountText;
    }
}
