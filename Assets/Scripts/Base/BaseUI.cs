using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourcesName;
    [SerializeField] private TextMeshProUGUI _resourcesCount;

    public void DrawResources(int resourcesCount)
    {
        _resourcesCount.text = (resourcesCount).ToString();
    }
}
