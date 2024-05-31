using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScanner : MonoBehaviour
{
    [SerializeField] private float _repeatRate;
    [SerializeField] private float _scanningRadius;
    [SerializeField] private LayerMask _scannLayerMask;

    private void Start()
    {
        InvokeRepeating(nameof(Scan), 0.0f, _repeatRate);
    }

    private void Scan()
    {
        Collider[] resourcesColliders = Physics.OverlapSphere(transform.position, _scanningRadius, _scannLayerMask);

        if (true)
        {

        }
    }
}