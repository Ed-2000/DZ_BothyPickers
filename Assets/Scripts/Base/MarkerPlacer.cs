using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MarkerPlacer : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Marker _markerPrefab;

    private Camera _camera;
    private Marker _marker;
    [SerializeField] private bool _canPlacedMarker = false;

    public Camera Camera { get => _camera; }

    public event Action<Vector3> Placed;

    public void OnPointerClick(PointerEventData eventData)
    {
        _canPlacedMarker = true;
    }

    private void Awake()
    {
        _marker = Instantiate(_markerPrefab);
        _marker.transform.SetParent(transform);
        _marker.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_canPlacedMarker == true && Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.TryGetComponent(out Ground _))
                PlacedMarker(hit.point);
        }
    }

    public void Init(Camera camera)
    {
        _camera = camera;
    }

    public void RemoveMarker()
    {
        _canPlacedMarker = false;
        _marker.gameObject.SetActive(false);
    }

    private void PlacedMarker(Vector3 position)
    {
        _canPlacedMarker = false;
        _marker.gameObject.SetActive(true);
        _marker.transform.position = position;
        Placed?.Invoke(position);
    }
}