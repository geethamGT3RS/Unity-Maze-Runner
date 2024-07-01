using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlugCableConnector : MonoBehaviour
{
    public GameObject cable;
    public Material lineMaterial; 
    public int curveResolution = 20;
    public float bendFactor = 0.5f;
    public float startWidth = 0.1f;
    public float endWidth = 0.1f;
    public float yOffset = 0.2f;
    public float wireLength = 10f; // Default wire length

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = curveResolution;
        InitializeLineRenderer();
        UpdateLineRenderer();
    }

    void Update()
    {
        UpdateLineRenderer();
    }

    void InitializeLineRenderer()
    {
        if (lineMaterial != null)
        {
            lineRenderer.material = lineMaterial;
            lineRenderer.startColor = Color.black;
            lineRenderer.endColor = Color.black;
        }

        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;

        UpdateLineRenderer();
    }

    void UpdateLineRenderer()
    {
        if (lineRenderer != null && cable != null)
        {
            Vector3 start = transform.position;
            Vector3 end = cable.transform.position + Vector3.down * yOffset;
            Vector3 direction = (end - start).normalized;
            float distance = Mathf.Min(Vector3.Distance(start, end), wireLength);
            end = start + direction * distance;

            Vector3 controlPoint1 = start + (end - start) * bendFactor - Vector3.up * bendFactor;
            Vector3 controlPoint2 = start + (end - start) * (1 - bendFactor) - Vector3.up * bendFactor;
            
            for (int i = 0; i < curveResolution; i++)
            {
                float t = i / (float)(curveResolution - 1);
                Vector3 position = CalculateBezierPoint(t, start, controlPoint1, controlPoint2, end);
                lineRenderer.SetPosition(i, position);
            }
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; // (1 - t)^3 * p0
        p += 3 * uu * t * p1; // 3 * (1 - t)^2 * t * p1
        p += 3 * u * tt * p2; // 3 * (1 - t) * t^2 * p2
        p += ttt * p3; // t^3 * p3

        return p;
    }

    public void SetWireLength(float length)
    {
        wireLength = length;
    }
}
