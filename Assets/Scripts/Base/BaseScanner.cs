using System.Collections.Generic;
using UnityEngine;

public class BaseScanner : MonoBehaviour
{
    [SerializeField] private bool _isDrawing;
    [SerializeField] private float _scanningRadius;
    [SerializeField] private LayerMask _scannLayerMask;
    [SerializeField] [Range(6, 100)] private int _countOfCirclePoints;

    private void OnDrawGizmos()
    {
        if (_isDrawing)
            DrawScanZone(_countOfCirclePoints, Color.blue);
    }

    public List<Resource> Scan()
    {
        List<Resource> resources = new List<Resource>();

        Collider[] resourcesColliders = Physics.OverlapSphere(transform.position, _scanningRadius, _scannLayerMask);

        foreach (var collider in resourcesColliders)
        {
            if (collider.TryGetComponent(out Resource resource))
                resources.Add(resource);
        }

        return resources;
    }

    private void DrawScanZone(int pointsCount, Color color)
    {
        List<Vector3> circlePoints = new List<Vector3>();
        float angleStep = 360.0f / pointsCount * Mathf.Deg2Rad;
        Vector3 center = transform.position;

        for (int i = 0; i < pointsCount; i++)
        {
            float angle = angleStep * i;
            Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _scanningRadius;
            circlePoints.Add(new Vector3(center.x + point.x, center.y, center.z + point.y));
        }

        for (int i = 0; i < circlePoints.Count - 1; i++)
            Debug.DrawLine(circlePoints[i], circlePoints[i + 1], color);

        Debug.DrawLine(circlePoints[0], circlePoints[circlePoints.Count - 1], color);
    }
}