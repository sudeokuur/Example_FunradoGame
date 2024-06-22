using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineDrawer : MonoBehaviour
{
    [SerializeField]private Material toungeMaterial;
    public float lineWidth = 0.1f;

    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private Queue<LineData> lineQueue = new Queue<LineData>();
    private bool isDrawing = false;

    //defines and sets the parameters that will be used in a struct
    private struct LineData
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
        public float duration;

        public LineData(Vector3 startPoint, Vector3 endPoint, float duration)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.duration = duration;
        }
    }

    //adding the lines to a queue
    public void DrawLine(Vector3 startPoint, Vector3 endPoint, float duration)
    {
        lineQueue.Enqueue(new LineData(startPoint, endPoint, duration));
        if (!isDrawing)
        {
            StartCoroutine(ProcessLineQueue());
        }
    }

    //drawing all the lines until none left
    private IEnumerator ProcessLineQueue()
    {
        isDrawing = true;

        while (lineQueue.Count > 0)
        {
            LineData lineData = lineQueue.Dequeue();
            yield return StartCoroutine(DrawLineCoroutine(lineData.startPoint, lineData.endPoint, lineData.duration));
        }

        isDrawing = false;
    }

    //all the operations needed to draw the lines
    private IEnumerator DrawLineCoroutine(Vector3 startPoint, Vector3 endPoint, float duration)
    {
        GameObject lineObject = new GameObject("Line");
        lineObject.tag = "Line";
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Set the material and colors
        lineRenderer.material = new Material(toungeMaterial);
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2;

        lineRenderers.Add(lineRenderer);

        float elapsedTime = 0f;

        Vector3 fixedStartPoint = new Vector3(startPoint.x, 5f, startPoint.z);
        Vector3 fixedEndPoint = new Vector3(endPoint.x, 5f, endPoint.z);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            Vector3 currentPos = Vector3.Lerp(fixedStartPoint, fixedEndPoint, t);

            lineRenderer.SetPosition(0, fixedStartPoint);
            lineRenderer.SetPosition(1, currentPos);

            yield return null;
        }

        //Ensure the final position is set
        lineRenderer.SetPosition(0, fixedStartPoint);
        lineRenderer.SetPosition(1, fixedEndPoint);
    }

    public void DrawMultipleLines(List<Vector3> startPoints, List<Vector3> endPoints, float duration)
    {
        if (startPoints.Count != endPoints.Count)
        {
            Debug.LogError("Start points and end points lists must have the same number of elements.");
            return;
        }

        for (int i = 0; i < startPoints.Count; i++)
        {
            DrawLine(startPoints[i], endPoints[i], duration);
        }
    }
}
