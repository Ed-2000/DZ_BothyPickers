using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedZone : MonoBehaviour
{
    [SerializeField] private Vector2 _minPosition;
    [SerializeField] private Vector2 _maxPosition;
    [SerializeField] private float _yPosition;
    [SerializeField] private Color _colorOFLine;
    [SerializeField] [Range(0.25f, 5)] private float _spawnStep;

    private List<Vector3> _freeSpawnPositions;
    private List<Vector3> _occupiedSpawnPositions;
    private int _maxCountOfSpawnedObjects;
    private int _variableForHalving = 2;

    public int MaxCountOfSpawnedObjects { get => _maxCountOfSpawnedObjects; private set => _maxCountOfSpawnedObjects = value; }

    private void Awake()
    {
        _occupiedSpawnPositions = new List<Vector3>();
    }

    private void OnValidate()
    {
        _freeSpawnPositions = new List<Vector3>();

        int xSpawnPositionsCount = Convert.ToInt32((_maxPosition.x - _minPosition.x) / _spawnStep);
        int zSpawnPositionsCount = Convert.ToInt32((_maxPosition.y - _minPosition.y) / _spawnStep);

        for (int i = 0; i < zSpawnPositionsCount; i++)
        {
            for (int j = 0; j < xSpawnPositionsCount; j++)
            {
                float xPosition = _minPosition.x + _spawnStep * j + _spawnStep / _variableForHalving;
                float zPosition = _minPosition.y + _spawnStep * i + _spawnStep / _variableForHalving;

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

    private void OnDrawGizmos()
    {
        for (int i = 0; i < _freeSpawnPositions.Count; i++)
            DrawRectangle(_freeSpawnPositions[i], _spawnStep, _spawnStep);

        Vector3 center = new Vector3();
        center.x = (_minPosition.x + _maxPosition.x) / _variableForHalving;
        center.y = _yPosition;
        center.z = (_minPosition.y + _maxPosition.y) / _variableForHalving;

        DrawRectangle(center, _maxPosition.x - _minPosition.x, _maxPosition.y - _minPosition.y);
    }

    public Vector3 GetPosition()
    {
        Vector3 randomPosition = _freeSpawnPositions[UnityEngine.Random.Range(0, _freeSpawnPositions.Count)];
        _freeSpawnPositions.Remove(randomPosition);
        _occupiedSpawnPositions.Add(randomPosition);

        return randomPosition;
    }

    private void DrawRectangle(Vector3 center, float sideLengthX, float sideLengthZ)
    {
        float halfSideLengthX = sideLengthX / 2;
        float halfSideLengthZ = sideLengthZ / 2;

        Vector3 pointA = new Vector3(center.x - halfSideLengthX, center.y, center.z - halfSideLengthZ);
        Vector3 pointB = new Vector3(center.x - halfSideLengthX, center.y, center.z + halfSideLengthZ);
        Vector3 pointC = new Vector3(center.x + halfSideLengthX, center.y, center.z + halfSideLengthZ);
        Vector3 pointD = new Vector3(center.x + halfSideLengthX, center.y, center.z - halfSideLengthZ);

        DrawByPoints(pointA, pointB, pointC, pointD);
    }

    private void DrawByPoints(Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD)
    {
        Debug.DrawLine(pointA, pointB, _colorOFLine);
        Debug.DrawLine(pointB, pointC, _colorOFLine);
        Debug.DrawLine(pointC, pointD, _colorOFLine);
        Debug.DrawLine(pointD, pointA, _colorOFLine);
    }
}