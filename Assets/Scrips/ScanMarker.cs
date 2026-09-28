using UnityEngine;
using UnityEngine.UI;

public class ScanMarker : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image markerImage;


    [Header("Screen Edge")]
    [SerializeField] private float screenEdgePadding = 50f;


    [Header("Marker")]
    [SerializeField] private bool rotateTowardsTarget = true;


    private Transform target;

    private Camera targetCamera;

    private RectTransform rectTransform;

    private RectTransform canvasRect;


    // =====================================================
    // INITIALIZE
    // =====================================================

    public void Initialize(
        Transform targetTransform,
        Camera cameraToUse
    )
    {
        target =
            targetTransform;


        targetCamera =
            cameraToUse;


        rectTransform =
            GetComponent<RectTransform>();


        Canvas canvas =
            GetComponentInParent<Canvas>();


        if (canvas != null)
        {
            canvasRect =
                canvas.GetComponent<RectTransform>();
        }


        if (markerImage == null)
        {
            markerImage =
                GetComponentInChildren<Image>();
        }
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);

            return;
        }


        if (targetCamera == null)
        {
            return;
        }


        if (canvasRect == null)
        {
            return;
        }


        UpdateMarkerPosition();
    }


    // =====================================================
    // UPDATE POSITION
    // =====================================================

    private void UpdateMarkerPosition()
    {
        Vector3 screenPosition =
            targetCamera.WorldToScreenPoint(
                target.position
            );


        bool isBehindCamera =
            screenPosition.z < 0f;


        bool isOutsideScreen =
            screenPosition.x < 0f ||
            screenPosition.x >
            Screen.width ||
            screenPosition.y < 0f ||
            screenPosition.y >
            Screen.height;


        // =================================================
        // OBJECT อยู่ในจอ
        // =================================================

        if (!isBehindCamera &&
            !isOutsideScreen)
        {
            SetMarkerInsideScreen(
                screenPosition
            );

            return;
        }


        // =================================================
        // OBJECT อยู่นอกจอ
        // =================================================

        SetMarkerAtScreenEdge(
            screenPosition,
            isBehindCamera
        );
    }


    // =====================================================
    // INSIDE SCREEN
    // =====================================================

    private void SetMarkerInsideScreen(
        Vector3 screenPosition
    )
    {
        Vector2 localPoint;


        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            null,
            out localPoint
        );


        rectTransform.anchoredPosition =
            localPoint;


        // อยู่ในจอ ไม่ต้องหมุน
        rectTransform.localRotation =
            Quaternion.identity;
    }


    // =====================================================
    // SCREEN EDGE
    // =====================================================

    private void SetMarkerAtScreenEdge(
        Vector3 screenPosition,
        bool behindCamera
    )
    {
        Vector2 screenCenter =
            new Vector2(
                Screen.width * 0.5f,
                Screen.height * 0.5f
            );


        Vector2 direction;


        // =================================================
        // OBJECT อยู่ด้านหลัง Camera
        // =================================================

        if (behindCamera)
        {
            direction =
                screenCenter -
                new Vector2(
                    screenPosition.x,
                    screenPosition.y
                );


            if (direction.sqrMagnitude <
                0.001f)
            {
                direction =
                    Vector2.up;
            }


            direction.Normalize();
        }


        // =================================================
        // OBJECT อยู่ด้านหน้าแต่หลุดจอ
        // =================================================

        else
        {
            direction =
                new Vector2(
                    screenPosition.x,
                    screenPosition.y
                ) -
                screenCenter;


            if (direction.sqrMagnitude <
                0.001f)
            {
                direction =
                    Vector2.up;
            }


            direction.Normalize();
        }


        // =================================================
        // หาจุดตัดกับขอบหน้าจอ
        // =================================================

        float halfWidth =
            Screen.width * 0.5f -
            screenEdgePadding;


        float halfHeight =
            Screen.height * 0.5f -
            screenEdgePadding;


        float scaleX =
            Mathf.Abs(
                halfWidth /
                direction.x
            );


        float scaleY =
            Mathf.Abs(
                halfHeight /
                direction.y
            );


        float scale =
            Mathf.Min(
                scaleX,
                scaleY
            );


        if (float.IsInfinity(scale) ||
            float.IsNaN(scale))
        {
            scale =
                Mathf.Min(
                    halfWidth,
                    halfHeight
                );
        }


        Vector2 edgePosition =
            screenCenter +
            direction *
            scale;


        // =================================================
        // Convert Screen → Canvas
        // =================================================

        Vector2 localPoint;


        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            edgePosition,
            null,
            out localPoint
        );


        rectTransform.anchoredPosition =
            localPoint;


        // =================================================
        // หมุน Marker ตามทิศทาง
        // =================================================

        if (rotateTowardsTarget)
        {
            float angle =
                Mathf.Atan2(
                    direction.y,
                    direction.x
                ) *
                Mathf.Rad2Deg;


            rectTransform.localRotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    angle - 90f
                );
        }
    }
}