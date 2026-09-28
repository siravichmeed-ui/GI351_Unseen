using System.Collections;
using UnityEngine;

public class PlayerScanSkill : MonoBehaviour
{
    [Header("Scan")]
    [SerializeField]
    private KeyCode scanKey =
        KeyCode.Q;


    [Header("Scan Duration")]
    [SerializeField]
    private float scanDuration =
        5f;


    [Header("Cooldown")]
    [SerializeField]
    private float cooldown =
        15f;


    [Header("Marker")]
    [SerializeField] private GameObject markerPrefab;


    [Header("Marker Parent")]
    [SerializeField] private Transform markerParent;


    [Header("Camera")]
    [SerializeField] private Camera targetCamera;


    private bool isScanning = false;

    private bool isCooldown = false;


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera =
                Camera.main;
        }


        if (markerParent == null)
        {
            Debug.LogWarning(
                "PlayerScanSkill ยังไม่ได้ใส่ Marker Parent"
            );
        }


        if (markerPrefab == null)
        {
            Debug.LogWarning(
                "PlayerScanSkill ยังไม่ได้ใส่ Marker Prefab"
            );
        }
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (Input.GetKeyDown(scanKey))
        {
            TryUseScan();
        }
    }


    // =====================================================
    // TRY USE
    // =====================================================

    private void TryUseScan()
    {
        if (isScanning)
            return;


        if (isCooldown)
            return;


        StartCoroutine(
            ScanRoutine()
        );
    }


    // =====================================================
    // SCAN ROUTINE
    // =====================================================

    private IEnumerator ScanRoutine()
    {
        isScanning = true;


        Debug.Log(
            "SCAN START"
        );


        // =================================================
        // SCAN ทุก Target ในแมพ
        // =================================================

        for (int i = 0;
             i < ScanTarget.AllTargets.Count;
             i++)
        {
            ScanTarget target =
                ScanTarget.AllTargets[i];


            if (target == null)
                continue;


            if (!target.CanBeScanned())
                continue;


            target.ShowMarker(
                markerPrefab,
                markerParent,
                targetCamera
            );
        }


        // =================================================
        // เปิด Scan ตามเวลาที่กำหนด
        // =================================================

        yield return new WaitForSeconds(
            scanDuration
        );


        // =================================================
        // ปิด Marker
        // =================================================

        for (int i = 0;
             i < ScanTarget.AllTargets.Count;
             i++)
        {
            ScanTarget target =
                ScanTarget.AllTargets[i];


            if (target == null)
                continue;


            target.RemoveMarker();
        }


        isScanning = false;


        Debug.Log(
            "SCAN END"
        );


        // =================================================
        // START COOLDOWN
        // =================================================

        StartCoroutine(
            CooldownRoutine()
        );
    }


    // =====================================================
    // COOLDOWN
    // =====================================================

    private IEnumerator CooldownRoutine()
    {
        isCooldown = true;


        yield return new WaitForSeconds(
            cooldown
        );


        isCooldown = false;


        Debug.Log(
            "SCAN READY"
        );
    }


    // =====================================================
    // PUBLIC
    // =====================================================

    public bool IsScanning()
    {
        return isScanning;
    }


    public bool IsOnCooldown()
    {
        return isCooldown;
    }
}