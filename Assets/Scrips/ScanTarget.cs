using System.Collections.Generic;
using UnityEngine;

public class ScanTarget : MonoBehaviour
{
    public static readonly List<ScanTarget> AllTargets =
        new List<ScanTarget>();


    [Header("Scan Target")]
    [SerializeField] private bool canBeScanned = true;


    private GameObject currentMarker;


    private void OnEnable()
    {
        if (!AllTargets.Contains(this))
        {
            AllTargets.Add(this);
        }
    }


    private void OnDisable()
    {
        AllTargets.Remove(this);

        RemoveMarker();
    }


    public bool CanBeScanned()
    {
        return canBeScanned;
    }


    public void ShowMarker(
        GameObject markerPrefab,
        Transform markerParent,
        Camera targetCamera
    )
    {
        if (!canBeScanned)
            return;


        RemoveMarker();


        if (markerPrefab == null)
        {
            Debug.LogWarning(
                "ScanTarget ไม่มี Marker Prefab"
            );

            return;
        }


        if (markerParent == null)
        {
            Debug.LogWarning(
                "ScanTarget ไม่มี Marker Parent"
            );

            return;
        }


        GameObject marker =
            Instantiate(
                markerPrefab,
                markerParent
            );


        currentMarker = marker;


        ScanMarker scanMarker =
            marker.GetComponent<ScanMarker>();


        if (scanMarker == null)
        {
            Debug.LogWarning(
                "Marker Prefab ต้องมี ScanMarker"
            );

            return;
        }


        scanMarker.Initialize(
            transform,
            targetCamera
        );
    }


    public void RemoveMarker()
    {
        if (currentMarker == null)
            return;


        Destroy(currentMarker);

        currentMarker = null;
    }
}