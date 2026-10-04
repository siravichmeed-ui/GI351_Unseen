using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LampManager : MonoBehaviour
{
    [Header("Player Light")]
    [SerializeField] private Light2D playerLight;

    [Header("Global Light")]
    [SerializeField] private Light2D globalLight;


    // =====================================================
    // PLAYER LIGHT
    // =====================================================

    [Header("Player Light Intensity")]
    [SerializeField] private float playerMaxIntensity = 10f;
    [SerializeField] private float playerMinIntensity = 0f;


    [Header("Player Light Radius")]
    [SerializeField] private float playerMaxRadius = 5f;
    [SerializeField] private float playerMinRadius = 0f;


    // =====================================================
    // GLOBAL LIGHT
    // =====================================================

    [Header("Global Light Intensity")]
    [SerializeField] private float globalStartIntensity = 0.2f;
    [SerializeField] private float globalMinIntensity = 0.05f;


    // =====================================================
    // LAMP TIME
    // =====================================================

    [Header("Lamp Time")]
    [SerializeField] private float lampDuration = 60f;


    // =====================================================
    // GLOBAL BLACKOUT
    // =====================================================

    [Header("Global Blackout Time")]
    [SerializeField] private float globalFadeDuration = 10f;


    // =====================================================
    // FLICKER
    // =====================================================

    [Header("Flicker")]
    [SerializeField] private float flickerStartPercent = 20f;

    [SerializeField] private float flickerMinIntensity = 0.5f;

    [SerializeField] private float flickerMaxIntensity = 2f;

    [SerializeField] private float flickerSpeed = 15f;


    // =====================================================
    // PRIVATE VARIABLES
    // =====================================================

    private float currentTime;

    private bool isFlickering;

    private bool isLampOff;

    private bool isGlobalFading;

    private float globalFadeTimer;


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        currentTime = lampDuration;


        // =================================================
        // PLAYER LIGHT
        // =================================================

        if (playerLight != null)
        {
            playerLight.intensity =
                playerMaxIntensity;

            playerLight.pointLightOuterRadius =
                playerMaxRadius;
        }


        // =================================================
        // GLOBAL LIGHT
        // =================================================

        if (globalLight != null)
        {
            globalLight.intensity =
                globalStartIntensity;
        }
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        // =================================================
        // PLAYER LAMP
        // =================================================

        if (!isLampOff)
        {
            UpdateLamp();
        }


        // =================================================
        // GLOBAL BLACKOUT
        // =================================================

        if (isGlobalFading)
        {
            UpdateGlobalLight();
        }
    }


    // =====================================================
    // LAMP
    // =====================================================

    private void UpdateLamp()
    {
        currentTime -=
            Time.deltaTime;


        // =================================================
        // หมดเวลา
        // =================================================

        if (currentTime <= 0f)
        {
            currentTime = 0f;

            TurnOffLamp();

            return;
        }


        // =================================================
        // เปอร์เซ็นต์เวลาที่เหลือ
        // =================================================

        float remainingPercent =
            (currentTime / lampDuration) * 100f;


        // =================================================
        // เริ่มกระพริบ
        // =================================================

        if (remainingPercent <=
            flickerStartPercent)
        {
            isFlickering = true;
        }


        // =================================================
        // UPDATE PLAYER LIGHT
        // =================================================

        UpdatePlayerLight();


        // =================================================
        // FLICKER
        // =================================================

        if (isFlickering)
        {
            FlickerLight();
        }
    }


    // =====================================================
    // PLAYER LIGHT
    // =====================================================

    private void UpdatePlayerLight()
    {
        if (playerLight == null)
            return;


        // =================================================
        // คำนวณสัดส่วนเวลาที่เหลือ
        // =================================================

        float progress =
            currentTime /
            lampDuration;


        progress =
            Mathf.Clamp01(
                progress
            );


        // =================================================
        // ลดขนาดวงแสงตามเวลาที่เหลือ
        // =================================================

        playerLight.pointLightOuterRadius =
            Mathf.Lerp(
                playerMinRadius,
                playerMaxRadius,
                progress
            );


        // =================================================
        // ความสว่าง
        // =================================================

        playerLight.intensity =
            playerMaxIntensity;
    }


    // =====================================================
    // FLICKER
    // =====================================================

    private void FlickerLight()
    {
        if (playerLight == null)
            return;


        float flicker =
            Mathf.PerlinNoise(
                Time.time *
                flickerSpeed,
                0f
            );


        float flickerValue =
            Mathf.Lerp(
                flickerMinIntensity,
                flickerMaxIntensity,
                flicker
            );


        playerLight.intensity =
            Mathf.Min(
                playerMaxIntensity,
                flickerValue
            );
    }


    // =====================================================
    // PLAYER LAMP OFF
    // =====================================================

    private void TurnOffLamp()
    {
        isLampOff = true;


        // =================================================
        // PLAYER LIGHT
        // =================================================

        if (playerLight != null)
        {
            playerLight.pointLightOuterRadius =
                playerMinRadius;

            playerLight.intensity =
                playerMinIntensity;
        }


        // =================================================
        // START GLOBAL LIGHT FADE
        // =================================================

        isGlobalFading = true;

        globalFadeTimer = 0f;
    }


    // =====================================================
    // GLOBAL LIGHT FADE
    // =====================================================

    private void UpdateGlobalLight()
    {
        if (globalLight == null)
            return;


        globalFadeTimer +=
            Time.deltaTime;


        float progress =
            globalFadeTimer /
            globalFadeDuration;


        progress =
            Mathf.Clamp01(
                progress
            );


        globalLight.intensity =
            Mathf.Lerp(
                globalStartIntensity,
                globalMinIntensity,
                progress
            );


        // =================================================
        // ถึงค่าต่ำสุดแล้ว
        // =================================================

        if (progress >= 1f)
        {
            globalLight.intensity =
                globalMinIntensity;

            isGlobalFading = false;
        }
    }


    // =====================================================
    // ADD LIGHT TIME
    // =====================================================

    public void AddLightTime(
        float amount
    )
    {
        if (amount <= 0f)
        {
            return;
        }


        // =================================================
        // เพิ่มเวลา
        // =================================================

        currentTime +=
            amount;


        // =================================================
        // ห้ามเกินเวลาสูงสุด
        // =================================================

        currentTime =
            Mathf.Clamp(
                currentTime,
                0f,
                lampDuration
            );


        // =================================================
        // ถ้าไฟเคยดับ
        // ให้เปิดกลับมา
        // =================================================

        if (currentTime > 0f)
        {
            isLampOff = false;

            isFlickering = false;
        }


        // =================================================
        // อัปเดตไฟทันที
        // =================================================

        UpdatePlayerLight();


        // =================================================
        // ถ้า Global Light กำลังดับ
        // ไม่ต้องหยุดระบบเดิม
        // =================================================


        Debug.Log(
            "เพิ่มเวลาไฟ: +"
            + amount
            + " วินาที"
            + " | เหลือ: "
            + currentTime
            + " วินาที"
        );
    }


    // =====================================================
    // GET REMAINING TIME
    // =====================================================

    public float GetRemainingTime()
    {
        return currentTime;
    }


    // =====================================================
    // GET REMAINING PERCENT
    // =====================================================

    public float GetRemainingPercent()
    {
        if (lampDuration <= 0f)
            return 0f;


        return
            (currentTime /
            lampDuration) *
            100f;
    }


    // =====================================================
    // GET LIGHT RADIUS
    // =====================================================

    public float GetCurrentLightRadius()
    {
        if (playerLight == null)
            return 0f;


        return playerLight.pointLightOuterRadius;
    }


    // =====================================================
    // RESET LAMP
    // =====================================================

    public void ResetLamp()
    {
        currentTime =
            lampDuration;


        isLampOff =
            false;


        isFlickering =
            false;


        isGlobalFading =
            false;


        globalFadeTimer =
            0f;


        if (playerLight != null)
        {
            playerLight.intensity =
                playerMaxIntensity;

            playerLight.pointLightOuterRadius =
                playerMaxRadius;
        }


        if (globalLight != null)
        {
            globalLight.intensity =
                globalStartIntensity;
        }
    }
}