using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedZone : MonoBehaviour
{
    [SerializeField] private bool _isDrawing;
    [SerializeField] private Vector2 _minPosition;
    [SerializeField] private Vector2 _maxPosition;
    [SerializeField] private float _yPosition;
    [SerializeField] private Color _colorOFLine;
    [SerializeField] [Range(0.25f, 5)] private float _spawnStep;

    private List<Vector3> _freeSpawnPositions;
    private List<Vector3> _occupiedSpawnPositions;
    private int _maxCountOfSpawnedObjects;
    private int _halfCoefficient = 2;

    public int MaxCountOfSpawnedObjects { get => _maxCountOfSpawnedObjects; private set => _maxCountOfSpawnedObjects = value; }

    private void Awake()
    {
        _occupiedSpawnPositions = new List<Vector3>();
        CalculateSpawnPositions();
    }

    private void OnValidate()
    {
        CalculateSpawnPositions();
    }

    private void OnDrawGizmos()
    {
        if (_isDrawing)
        {
            for (int i = 0; i < _freeSpawnPositions.Count; i++)
                DrawRectangle(_freeSpawnPositions[i], _spawnStep, _spawnStep);
        }
    }

    public Vector3 GetPosition()
    {
        Vector3 randomPosition = _freeSpawnPositions[UnityEngine.Random.Range(0, _freeSpawnPositions.Count)];
        _freeSpawnPositions.Remove(randomPosition);
        _occupiedSpawnPositions.Add(randomPosition);

        return randomPosition;
    }

    public void CalculateSpawnPositions()
    {
        _freeSpawnPositions = new List<Vector3>();

        int xSpawnPositionsCount = Convert.ToInt32((_maxPosition.x - _minPosition.x) / _spawnStep);
        int zSpawnPositionsCount = Convert.ToInt32((_maxPosition.y - _minPosition.y) / _spawnStep);

        for (int i = 0; i < zSpawnPositionsCount; i++)
        {
            for (int j = 0; j < xSpawnPositionsCount; j++)
            {
                float xPosition = _minPosition.x + _spawnStep * j + _spawnStep / _halfCoefficient;
                float zPosition = _minPosition.y + _spawnStep * i + _spawnStep / _halfCoefficient;

                Vector3 origin = new Vector3(xPosition, _yPosition, zPosition);

                if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit))
                {
                    if (hit.transform.TryGetComponent(out Ground ground))
                        _freeSpawnPositions.Add(hit.point);
                }
            }
        }

        MaxCountOfSpawnedObjects = _freeSpawnPositions.Count;
    }

    private void DrawRectangle(Vector3 center, float sideLengthX, float sideLengthZ)
    {
        List<Vector3> points = new List<Vector3>();
        
        float halfSideLengthX = sideLengthX / _halfCoefficient;
        float halfSideLengthZ = sideLengthZ / _halfCoefficient;

        points.Add(new Vector3(center.x - halfSideLengthX, center.y, center.z - halfSideLengthZ));
        points.Add(new Vector3(center.x - halfSideLengthX, center.y, center.z + halfSideLengthZ));
        points.Add(new Vector3(center.x + halfSideLengthX, center.y, center.z + halfSideLengthZ));
        points.Add(new Vector3(center.x + halfSideLengthX, center.y, center.z - halfSideLengthZ));

        DrawByPoints(points);
    }

    private void DrawByPoints(List<Vector3> points)
    {
        for (int i = 0; i < points.Count - 1; i++)
            Debug.DrawLine(points[i], points[i + 1], _colorOFLine);

        Debug.DrawLine(points[points.Count - 1], points[0], _colorOFLine);
    }
}