using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MarkerSetter : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Marker _markerPrefab;

    private Marker _marker;
    private bool _canSetMarker = false;

    public Camera Camera { get => _camera;}

    public event Action<Vector3> Set;

    public void OnPointerClick(PointerEventData eventData)
    {
        _canSetMarker = true;
    }

    private void Awake()
    {
        _marker = Instantiate(_markerPrefab);
        _marker.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_canSetMarker == true && Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.TryGetComponent(out Ground _))
                SetMarker(hit.point);
        }
    }

    public void Init(Camera camera)
    {
        _camera = camera;
    }

    public void RemoveMarker()
    {
        _canSetMarker = true;
        _marker.gameObject.SetActive(false);
    }
    
    private void SetMarker(Vector3 position)
    {
        _canSetMarker = false;
        _marker.gameObject.SetActive(true);
        _marker.transform.position = position;
        Set?.Invoke(position);
    }
}