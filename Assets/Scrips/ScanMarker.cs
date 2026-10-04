using UnityEngine;
using UnityEngine.UI;

public class ScanMarker : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image markerImage;


    [Header("Screen Edge")]
    [SerializeField] private float screenEdgePadding = 50f;


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
        target = targetTransform;

        targetCamera = cameraToUse;

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


        // สำคัญ
        // บังคับให้ Marker ตั้งตรงตั้งแต่เริ่ม
        rectTransform.localRotation =
            Quaternion.identity;
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
            return;


        if (canvasRect == null)
            return;


        UpdateMarkerPosition();


        // =================================================
        // บังคับให้ ? ตั้งตรงตลอดเวลา
        // =================================================

        rectTransform.localRotation =
            Quaternion.identity;
    }


    // =====================================================
    // UPDATE MARKER POSITION
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
            screenPosition.x > Screen.width ||
            screenPosition.y < 0f ||
            screenPosition.y > Screen.height;


        // =================================================
        // TARGET อยู่ในจอ
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
        // TARGET อยู่นอกจอ
        // =================================================

        SetMarkerAtScreenEdge(
            screenPosition,
            isBehindCamera
        );
    }


    // =====================================================
    // TARGET อยู่ในจอ
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


        // ห้ามหมุน
        rectTransform.localRotation =
            Quaternion.identity;
    }


    // =====================================================
    // TARGET อยู่นอกจอ
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
        // TARGET อยู่ด้านหลัง Camera
        // =================================================

        if (behindCamera)
        {
            direction =
                screenCenter -
                new Vector2(
                    screenPosition.x,
                    screenPosition.y
                );


            if (direction.sqrMagnitude < 0.001f)
            {
                direction =
                    Vector2.up;
            }


            direction.Normalize();
        }


        // =================================================
        // TARGET อยู่ด้านหน้า
        // =================================================

        else
        {
            direction =
                new Vector2(
                    screenPosition.x,
                    screenPosition.y
                ) -
                screenCenter;


            if (direction.sqrMagnitude < 0.001f)
            {
                direction =
                    Vector2.up;
            }


            direction.Normalize();
        }


        // =================================================
        // ขนาดพื้นที่ด้านในของ Screen
        // =================================================

        float halfWidth =
            Screen.width * 0.5f -
            screenEdgePadding;


        float halfHeight =
            Screen.height * 0.5f -
            screenEdgePadding;


        // =================================================
        // ป้องกันหารด้วย 0
        // =================================================

        float scaleX;

        float scaleY;


        if (Mathf.Abs(direction.x) > 0.001f)
        {
            scaleX =
                Mathf.Abs(
                    halfWidth /
                    direction.x
                );
        }
        else
        {
            scaleX =
                float.PositiveInfinity;
        }


        if (Mathf.Abs(direction.y) > 0.001f)
        {
            scaleY =
                Mathf.Abs(
                    halfHeight /
                    direction.y
                );
        }
        else
        {
            scaleY =
                float.PositiveInfinity;
        }


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


        // =================================================
        // ตำแหน่งบนขอบจอ
        // =================================================

        Vector2 edgePosition =
            screenCenter +
            direction *
            scale;


        // =================================================
        // Screen → Canvas
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
        // สำคัญมาก
        //
        // ไม่หมุน Marker
        // ? จะตั้งตรงเสมอ
        // =================================================

        rectTransform.localRotation =
            Quaternion.identity;
    }
}