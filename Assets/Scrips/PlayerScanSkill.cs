using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScanSkill : MonoBehaviour
{
    [Header("Scan")]
    [SerializeField] private KeyCode scanKey = KeyCode.Q;

    [SerializeField] private float scanRange = 8f;


    [Header("Scan Duration")]
    [SerializeField] private float scanDuration = 5f;


    [Header("Cooldown")]
    [SerializeField] private float cooldown = 15f;

    [SerializeField] private float minimumCooldown = 1f;


    [Header("Marker")]
    [SerializeField] private GameObject markerPrefab;


    [Header("Marker Parent")]
    [SerializeField] private Transform markerParent;


    [Header("Camera")]
    [SerializeField] private Camera targetCamera;


    [Header("Radar Effect")]
    [SerializeField] private RadarScanEffect radarEffect;


    private bool isScanning;

    private bool isCooldown;


    // Object ที่ Radar เจอแล้ว
    private HashSet<ScanTarget> detectedTargets =
        new HashSet<ScanTarget>();


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
            GameObject scanMarkers =
                GameObject.Find(
                    "ScanMarkers"
                );


            if (scanMarkers != null)
            {
                markerParent =
                    scanMarkers.transform;
            }
        }


        if (radarEffect == null)
        {
            radarEffect =
                FindFirstObjectByType<RadarScanEffect>();
        }


        if (targetCamera == null)
        {
            Debug.LogError(
                "PlayerScanSkill: หา Main Camera ไม่เจอ"
            );
        }


        if (markerPrefab == null)
        {
            Debug.LogWarning(
                "PlayerScanSkill: Marker Prefab ยังไม่ได้ใส่"
            );
        }


        if (markerParent == null)
        {
            Debug.LogWarning(
                "PlayerScanSkill: หา ScanMarkers ไม่เจอ"
            );
        }


        if (radarEffect == null)
        {
            Debug.LogError(
                "PlayerScanSkill: หา RadarScanEffect ไม่เจอ"
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
    // TRY USE SCAN
    // =====================================================

    private void TryUseScan()
    {
        if (isScanning)
        {
            return;
        }


        if (isCooldown)
        {
            return;
        }


        StartCoroutine(
            ScanRoutine()
        );
    }


    // =====================================================
    // SCAN ROUTINE
    // =====================================================

    private IEnumerator ScanRoutine()
    {
        isScanning =
            true;


        Debug.Log(
            "========== SCAN START =========="
        );


        // ล้าง Object ที่เคยเจอจาก Scan รอบก่อน
        detectedTargets.Clear();


        // =================================================
        // ตั้ง Radar
        // =================================================

        if (radarEffect != null)
        {
            radarEffect.SetMaxRadius(
                scanRange
            );


            radarEffect.PlayRadar();
        }


        // =================================================
        // ระยะเวลาที่ Radar ทำงาน
        // =================================================

        float radarTime = 0f;


        float radarDuration =
            radarEffect != null
                ? radarEffect.GetDuration()
                : 0f;


        // =================================================
        // RADAR กำลังขยาย
        // =================================================

        while (
            radarTime <
            radarDuration
        )
        {
            radarTime +=
                Time.deltaTime;


            // คำนวณรัศมีปัจจุบัน
            float currentRadius =
                radarEffect.GetCurrentRadius(
                    radarTime
                );


            // ตรวจ Object ที่ Radar ไปถึงแล้ว
            CheckTargetsInRadar(
                currentRadius
            );


            yield return null;
        }


        // =================================================
        // RADAR จบแล้ว
        // =================================================

        Debug.Log(
            "========== RADAR FINISHED =========="
        );


        // =================================================
        // แสดง ? เฉพาะ Object ที่ Radar เจอ
        // =================================================

        ShowDetectedMarkers();


        // =================================================
        // รอเวลาที่ ? จะแสดง
        // =================================================

        yield return new WaitForSecondsRealtime(
            scanDuration
        );


        // =================================================
        // ลบ ?
        // =================================================

        RemoveScanMarkers();


        isScanning =
            false;


        Debug.Log(
            "========== SCAN END =========="
        );


        // =================================================
        // Cooldown
        // =================================================

        StartCoroutine(
            CooldownRoutine()
        );
    }


    // =====================================================
    // CHECK TARGETS
    // =====================================================

    private void CheckTargetsInRadar(
        float currentRadius
    )
    {
        if (
            ScanTarget.AllTargets ==
            null
        )
        {
            return;
        }


        for (
            int i = 0;
            i < ScanTarget.AllTargets.Count;
            i++
        )
        {
            ScanTarget target =
                ScanTarget.AllTargets[i];


            if (target == null)
            {
                continue;
            }


            // ถ้าเจอไปแล้ว ไม่ต้องตรวจซ้ำ
            if (
                detectedTargets.Contains(
                    target
                )
            )
            {
                continue;
            }


            // Target ที่ Scan ไม่ได้
            if (!target.CanBeScanned())
            {
                continue;
            }


            // =================================================
            // หาระยะจาก Player ไป Target
            // =================================================

            float distance =
                Vector2.Distance(
                    transform.position,
                    target.transform.position
                );


            // =================================================
            // Radar ไปถึง Target แล้ว
            // =================================================

            if (
                distance <=
                currentRadius
            )
            {
                detectedTargets.Add(
                    target
                );


                Debug.Log(
                    "RADAR FOUND: "
                    + target.name
                    + " | Distance = "
                    + distance.ToString("F2")
                );
            }
        }
    }


    // =====================================================
    // SHOW DETECTED MARKERS
    // =====================================================

    private void ShowDetectedMarkers()
    {
        if (markerPrefab == null)
        {
            return;
        }


        if (markerParent == null)
        {
            return;
        }


        foreach (
            ScanTarget target
            in detectedTargets
        )
        {
            if (target == null)
            {
                continue;
            }


            target.ShowMarker(
                markerPrefab,
                markerParent,
                targetCamera
            );
        }
    }


    // =====================================================
    // REMOVE MARKERS
    // =====================================================

    private void RemoveScanMarkers()
    {
        foreach (
            ScanTarget target
            in detectedTargets
        )
        {
            if (target == null)
            {
                continue;
            }


            target.RemoveMarker();
        }


        detectedTargets.Clear();
    }


    // =====================================================
    // COOLDOWN
    // =====================================================

    private IEnumerator CooldownRoutine()
    {
        isCooldown =
            true;


        yield return new WaitForSecondsRealtime(
            cooldown
        );


        isCooldown =
            false;


        Debug.Log(
            "========== SCAN READY =========="
        );
    }


    // =====================================================
    // UPGRADE - SCAN RANGE
    // =====================================================

    public void IncreaseScanRange(
        float amount
    )
    {
        if (amount <= 0f)
        {
            return;
        }


        scanRange +=
            amount;


        Debug.Log(
            "Scan Range เพิ่มเป็น: " +
            scanRange
        );
    }


    // =====================================================
    // UPGRADE - COOLDOWN
    // =====================================================

    public void DecreaseCooldown(
        float amount
    )
    {
        if (amount <= 0f)
        {
            return;
        }


        cooldown -=
            amount;


        cooldown =
            Mathf.Max(
                cooldown,
                minimumCooldown
            );


        Debug.Log(
            "Scan Cooldown ลดเหลือ: " +
            cooldown +
            " วินาที"
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


    public float GetScanRange()
    {
        return scanRange;
    }


    public float GetCooldown()
    {
        return cooldown;
    }


    public float GetScanDuration()
    {
        return scanDuration;
    }
}