using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScanSkill : MonoBehaviour
{
    [Header("Scan")]
    [SerializeField] private KeyCode scanKey = KeyCode.Q;
    [SerializeField] private float scanRange = 8f;

    [Header("First Scan")]
    [SerializeField] private bool firstScanFullMap = true;

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


    // =====================================================
    // STATE
    // =====================================================

    private bool isScanning;
    private bool isCooldown;

    // เช็กว่าใช้ Scan ครั้งแรกไปแล้วหรือยัง
    private bool hasUsedFirstScan = false;

    // เวลาที่เหลือของ Cooldown
    private float cooldownRemaining = 0f;


    // Object ที่ Scan เจอ
    private HashSet<ScanTarget> detectedTargets =
        new HashSet<ScanTarget>();


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }


        if (markerParent == null)
        {
            GameObject scanMarkers =
                GameObject.Find("ScanMarkers");

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
        // =================================================
        // UPDATE COOLDOWN
        // =================================================

        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -=
                Time.unscaledDeltaTime;

            cooldownRemaining =
                Mathf.Max(
                    0f,
                    cooldownRemaining
                );
        }


        // =================================================
        // SCAN INPUT
        // =================================================

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
        // กำลัง Scan อยู่
        if (isScanning)
        {
            return;
        }


        // กำลัง Cooldown
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
        isScanning = true;


        Debug.Log(
            "========== SCAN START =========="
        );


        // ล้าง Target จาก Scan ก่อนหน้า
        detectedTargets.Clear();


        // =================================================
        // FIRST SCAN
        // =================================================

        if (
            firstScanFullMap &&
            !hasUsedFirstScan
        )
        {
            Debug.Log(
                "========== FIRST SCAN : FULL MAP =========="
            );
        }
        else
        {
            Debug.Log(
                "========== NORMAL SCAN : RANGE =========="
            );
        }


        // =================================================
        // RADAR
        // =================================================

        if (radarEffect != null)
        {
            radarEffect.SetMaxRadius(
                scanRange
            );

            radarEffect.PlayRadar();
        }


        // =================================================
        // FIRST SCAN = FULL MAP
        // =================================================

        if (
            firstScanFullMap &&
            !hasUsedFirstScan
        )
        {
            ScanEntireMap();
        }


        // =================================================
        // NORMAL SCAN = RANGE
        // =================================================

        else
        {
            float radarTime = 0f;

            float radarDuration =
                radarEffect != null
                    ? radarEffect.GetDuration()
                    : 0f;


            while (
                radarTime <
                radarDuration
            )
            {
                radarTime +=
                    Time.deltaTime;


                // หารัศมีปัจจุบันของ Radar
                float currentRadius =
                    radarEffect.GetCurrentRadius(
                        radarTime
                    );


                // ตรวจ Target ที่อยู่ในระยะ
                CheckTargetsInRadar(
                    currentRadius
                );


                yield return null;
            }
        }


        // =================================================
        // FIRST SCAN USED
        // =================================================

        hasUsedFirstScan = true;


        // =================================================
        // RADAR FINISHED
        // =================================================

        Debug.Log(
            "========== RADAR FINISHED =========="
        );


        // =================================================
        // SHOW MARKERS
        // =================================================

        ShowDetectedMarkers();


        // =================================================
        // MARKER DISPLAY TIME
        // =================================================

        yield return new WaitForSecondsRealtime(
            scanDuration
        );


        // =================================================
        // REMOVE MARKERS
        // =================================================

        RemoveScanMarkers();


        isScanning = false;


        Debug.Log(
            "========== SCAN END =========="
        );


        // =================================================
        // START COOLDOWN
        // =================================================

        StartCoroutine(
            CooldownRoutine()
        );
    }


    // =====================================================
    // FIRST SCAN - FULL MAP
    // =====================================================

    private void ScanEntireMap()
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


            // Target ที่ไม่อนุญาตให้ Scan
            if (!target.CanBeScanned())
            {
                continue;
            }


            // เพิ่ม Target เข้า List
            detectedTargets.Add(
                target
            );


            Debug.Log(
                "FIRST SCAN FOUND: " +
                target.name
            );
        }
    }


    // =====================================================
    // NORMAL SCAN - CHECK TARGETS
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


            // ถ้าเจอแล้ว ไม่ต้องตรวจซ้ำ
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
            // DISTANCE
            // =================================================

            float distance =
                Vector2.Distance(
                    transform.position,
                    target.transform.position
                );


            // =================================================
            // RADAR REACHED TARGET
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
        isCooldown = true;


        // ตั้งเวลาเริ่มต้น
        cooldownRemaining = cooldown;


        Debug.Log(
            "========== SCAN COOLDOWN =========="
        );


        // รอจนกว่าจะหมด Cooldown
        while (
            cooldownRemaining > 0f
        )
        {
            yield return null;
        }


        cooldownRemaining = 0f;

        isCooldown = false;


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


    public float GetCooldownRemaining()
    {
        return cooldownRemaining;
    }


    public float GetScanDuration()
    {
        return scanDuration;
    }
}