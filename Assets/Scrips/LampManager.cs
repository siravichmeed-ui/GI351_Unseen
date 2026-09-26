using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LampManager : MonoBehaviour
{
    [Header("Player Light")]
    [SerializeField] private Light2D playerLight;

    [Header("Global Light")]
    [SerializeField] private Light2D globalLight;

    [Header("Player Light Intensity")]
    [SerializeField] private float playerMaxIntensity = 10f;
    [SerializeField] private float playerMinIntensity = 0f;

    [Header("Global Light Intensity")]
    [SerializeField] private float globalStartIntensity = 0.2f;
    [SerializeField] private float globalMinIntensity = 0.05f;

    [Header("Lamp Time")]
    [SerializeField] private float lampDuration = 60f;

    [Header("Global Blackout Time")]
    [SerializeField] private float globalFadeDuration = 10f;

    [Header("Flicker")]
    [SerializeField] private float flickerStartPercent = 20f;
    [SerializeField] private float flickerMinIntensity = 0.5f;
    [SerializeField] private float flickerMaxIntensity = 2f;
    [SerializeField] private float flickerSpeed = 15f;

    private float currentTime;

    private bool isFlickering;
    private bool isLampOff;
    private bool isGlobalFading;

    private float globalFadeTimer;


    private void Start()
    {
        currentTime = lampDuration;

        // Player Light
        if (playerLight != null)
        {
            playerLight.intensity =
                playerMaxIntensity;
        }

        // Global Light
        if (globalLight != null)
        {
            globalLight.intensity =
                globalStartIntensity;
        }
    }


    private void Update()
    {
        // =====================================================
        // PLAYER LAMP
        // =====================================================

        if (!isLampOff)
        {
            UpdateLamp();
        }


        // =====================================================
        // GLOBAL BLACKOUT
        // =====================================================

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
        currentTime -= Time.deltaTime;


        // หมดเวลา
        if (currentTime <= 0f)
        {
            currentTime = 0f;

            TurnOffLamp();

            return;
        }


        // เปอร์เซ็นต์เวลาที่เหลือ
        float remainingPercent =
            (currentTime / lampDuration) * 100f;


        // เริ่มกระพริบ
        if (remainingPercent <= flickerStartPercent)
        {
            isFlickering = true;
        }


        // อัปเดตแสง
        if (isFlickering)
        {
            FlickerLight();
        }
        else
        {
            UpdatePlayerLight();
        }
    }


    // =====================================================
    // PLAYER LIGHT
    // =====================================================

    private void UpdatePlayerLight()
    {
        if (playerLight == null)
            return;


        float progress =
            currentTime / lampDuration;


        playerLight.intensity =
            Mathf.Lerp(
                playerMinIntensity,
                playerMaxIntensity,
                progress
            );
    }


    // =====================================================
    // FLICKER
    // =====================================================

    private void FlickerLight()
    {
        if (playerLight == null)
            return;


        float progress =
            currentTime / lampDuration;


        // ความสว่างพื้นฐานที่เหลืออยู่
        float baseIntensity =
            Mathf.Lerp(
                playerMinIntensity,
                playerMaxIntensity,
                progress
            );


        // สุ่มการกระพริบแบบ Smooth
        float flicker =
            Mathf.PerlinNoise(
                Time.time * flickerSpeed,
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
                baseIntensity,
                flickerValue
            );
    }


    // =====================================================
    // PLAYER LAMP OFF
    // =====================================================

    private void TurnOffLamp()
    {
        isLampOff = true;

        // ดับ Player Light
        if (playerLight != null)
        {
            playerLight.intensity =
                playerMinIntensity;
        }


        // เริ่มลด Global Light
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
            Mathf.Clamp01(progress);


        globalLight.intensity =
            Mathf.Lerp(
                globalStartIntensity,
                globalMinIntensity,
                progress
            );


        // ถึงค่าต่ำสุดแล้ว
        if (progress >= 1f)
        {
            globalLight.intensity =
                globalMinIntensity;

            isGlobalFading = false;
        }
    }
}