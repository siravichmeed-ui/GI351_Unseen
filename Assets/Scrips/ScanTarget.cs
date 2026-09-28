using System.Collections.Generic;
using UnityEngine;

public class ScanTarget : MonoBehaviour
{
    // เก็บ ScanTarget ทั้งหมดในแมพ
    public static readonly List<ScanTarget> AllTargets =
        new List<ScanTarget>();


    [Header("Scan Target")]
    [SerializeField] private bool canBeScanned = true;


    // Marker ที่จะถูกสร้างตอน Scan
    private GameObject currentMarker;


    // =====================================================
    // ENABLE
    // =====================================================

    private void OnEnable()
    {
        if (!AllTargets.Contains(this))
        {
            AllTargets.Add(this);
        }
    }


    // =====================================================
    // DISABLE
    // =====================================================

    private void OnDisable()
    {
        AllTargets.Remove(this);

        RemoveMarker();
    }


    // =====================================================
    // CAN SCAN
    // =====================================================

    public bool CanBeScanned()
    {
        return canBeScanned;
    }


    // =====================================================
    // SHOW MARKER
    // =====================================================

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


        currentMarker =
            marker;


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


    // =====================================================
    // REMOVE MARKER
    // =====================================================

    public void RemoveMarker()
    {
        if (currentMarker == null)
            return;


        Destroy(currentMarker);

        currentMarker = null;
    }
}