using System.Collections;
using UnityEngine;

public class RadarScanEffect : MonoBehaviour
{
    [Header("Radar Range")]
    [SerializeField] private float startRadius = 0.1f;
    [SerializeField] private float maxRadius = 8f;

    [Header("Radar Animation")]
    [SerializeField] private float duration = 1.2f;

    [Header("Radar Appearance")]
    [SerializeField] private Color radarColor = Color.white;
    [SerializeField] private float lineWidth = 0.05f;

    [Header("Circle")]
    [SerializeField] private int segments = 100;

    [Header("Sorting")]
    [SerializeField] private int sortingOrder = 100;

    private LineRenderer lineRenderer;

    private Transform player;

    private Coroutine radarCoroutine;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        SetupLineRenderer();
    }


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        FindPlayer();

        HideRadar();
    }


    // =====================================================
    // FIND PLAYER
    // =====================================================

    private void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player =
                playerObject.transform;

            return;
        }


        GameObject playerByName =
            GameObject.Find("Player");

        if (playerByName != null)
        {
            player =
                playerByName.transform;

            return;
        }


        Debug.LogError(
            "RadarScanEffect: หา Player ไม่เจอ"
        );
    }


    // =====================================================
    // SETUP LINE RENDERER
    // =====================================================

    private void SetupLineRenderer()
    {
        lineRenderer =
            GetComponent<LineRenderer>();


        if (lineRenderer == null)
        {
            lineRenderer =
                gameObject.AddComponent<LineRenderer>();
        }


        lineRenderer.positionCount =
            segments + 1;


        lineRenderer.useWorldSpace =
            true;


        lineRenderer.loop =
            false;


        lineRenderer.startWidth =
            lineWidth;


        lineRenderer.endWidth =
            lineWidth;


        Shader shader =
            Shader.Find("Sprites/Default");


        if (shader != null)
        {
            Material material =
                new Material(shader);

            lineRenderer.material =
                material;
        }


        lineRenderer.startColor =
            radarColor;


        lineRenderer.endColor =
            radarColor;


        lineRenderer.sortingOrder =
            sortingOrder;


        lineRenderer.enabled =
            false;
    }


    // =====================================================
    // PLAY RADAR
    // =====================================================

    public void PlayRadar()
    {
        Debug.Log(
            "RADAR EFFECT PLAY"
        );


        if (player == null)
        {
            FindPlayer();
        }


        if (player == null)
        {
            Debug.LogError(
                "RadarScanEffect: Player ไม่มี"
            );

            return;
        }


        if (lineRenderer == null)
        {
            SetupLineRenderer();
        }


        if (radarCoroutine != null)
        {
            StopCoroutine(
                radarCoroutine
            );
        }


        radarCoroutine =
            StartCoroutine(
                RadarRoutine()
            );
    }


    // =====================================================
    // RADAR ROUTINE
    // =====================================================

    private IEnumerator RadarRoutine()
    {
        lineRenderer.enabled =
            true;


        float timer = 0f;


        while (timer < duration)
        {
            timer +=
                Time.deltaTime;


            float progress =
                Mathf.Clamp01(
                    timer / duration
                );


            float radius =
                Mathf.Lerp(
                    startRadius,
                    maxRadius,
                    progress
                );


            DrawCircle(
                radius
            );


            float alpha =
                Mathf.Lerp(
                    1f,
                    0f,
                    progress
                );


            Color color =
                radarColor;


            color.a =
                alpha;


            lineRenderer.startColor =
                color;


            lineRenderer.endColor =
                color;


            yield return null;
        }


        HideRadar();


        radarCoroutine =
            null;
    }


    // =====================================================
    // DRAW CIRCLE
    // =====================================================

    private void DrawCircle(
        float radius
    )
    {
        if (player == null)
            return;


        Vector3 center =
            player.position;


        for (
            int i = 0;
            i <= segments;
            i++
        )
        {
            float angle =
                (float)i /
                segments *
                Mathf.PI *
                2f;


            float x =
                Mathf.Cos(angle) *
                radius;


            float y =
                Mathf.Sin(angle) *
                radius;


            Vector3 position =
                center +
                new Vector3(
                    x,
                    y,
                    0f
                );


            position.z =
                center.z;


            lineRenderer.SetPosition(
                i,
                position
            );
        }
    }


    // =====================================================
    // HIDE RADAR
    // =====================================================

    public void HideRadar()
    {
        if (lineRenderer == null)
            return;


        lineRenderer.enabled =
            false;
    }


    // =====================================================
    // GET CURRENT RADIUS
    // =====================================================

    public float GetCurrentRadius(
        float elapsedTime
    )
    {
        float progress =
            Mathf.Clamp01(
                elapsedTime /
                duration
            );


        return Mathf.Lerp(
            startRadius,
            maxRadius,
            progress
        );
    }


    // =====================================================
    // GET DURATION
    // =====================================================

    public float GetDuration()
    {
        return duration;
    }


    // =====================================================
    // GET MAX RADIUS
    // =====================================================

    public float GetMaxRadius()
    {
        return maxRadius;
    }


    // =====================================================
    // SET MAX RADIUS
    // =====================================================

    public void SetMaxRadius(
        float radius
    )
    {
        maxRadius =
            radius;
    }
}