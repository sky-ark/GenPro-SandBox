using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delaunay : MonoBehaviour
{
    public Shader gradiantShader;
    public BSP bsp;
    private List<Rect> rects;
    private List<Vector2> points;
    private List<Triangle> allTriangles;
    private List<Triangle> delaunayTriangles;

    [ContextMenu("Launch")]
    private void Launch()
    {
        // Call bsp to launch the rectangles calculation
        bsp.Launch();
        // Get the rectangles from the BSP script
        rects = bsp.GetRects();
        // Generate points from the centers of the rectangles
        GeneratePointsFromRects();
        // Perform Delaunay triangulation
        Triangulation();
        // Draw all triangles and circles for 3 seconds
        // DrawTriangles(allTriangles, Color.yellow, 3f);
        // DrawCircumcircles(allTriangles, Color.red, 3f);
        // Draw only Delaunay triangles for 10 seconds
        DrawTriangles(delaunayTriangles, Color.green, 10f);
        // DrawCircumcircles(delaunayTriangles, Color.magenta, 10f);
        DrawFilledTriangles(delaunayTriangles);
    }

    // Get the rectangles from the BSP script
    private void GeneratePointsFromRects()
    {
        points = new List<Vector2>();

        foreach (var rect in rects)
        {
            Vector2 center = new Vector2(rect.x + rect.width / 2, rect.y + rect.height / 2);
            points.Add(center);
        }
    }

    private void Triangulation()
    {
        allTriangles = new List<Triangle>();
        delaunayTriangles = new List<Triangle>();

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = 0; j < points.Count; j++)
            {
                if (i != j)
                {
                    for (int k = 0; k < points.Count; k++)
                    {
                        if (j != k && i != k)
                        {
                            Vector2 p1 = points[i];
                            Vector2 p2 = points[j];
                            Vector2 p3 = points[k];
                            Triangle triangle = new Triangle(p1, p2, p3);
                            allTriangles.Add(triangle);
                            if (IsDelaunay(triangle))
                            {
                                delaunayTriangles.Add(triangle);
                            }
                        }
                    }
                }
            }
        }
    }

    private bool IsDelaunay(Triangle triangle)
    {
        Vector2 center;
        float radius;
        GetCircumcircle(triangle, out center, out radius);

        foreach (var point in points)
        {
            if (point != triangle.p1 && point != triangle.p2 && point != triangle.p3)
            {
                if (Vector2.Distance(center, point) < radius)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void GetCircumcircle(Triangle triangle, out Vector2 center, out float radius)
    {
        Vector2 p1 = triangle.p1;
        Vector2 p2 = triangle.p2;
        Vector2 p3 = triangle.p3;

        float ax = p1.x;
        float ay = p1.y;
        float bx = p2.x;
        float by = p2.y;
        float cx = p3.x;
        float cy = p3.y;

        float d = 2 * (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by));
        float ux = ((ax * ax + ay * ay) * (by - cy) + (bx * bx + by * by) * (cy - ay) + (cx * cx + cy * cy) * (ay - by)) / d;
        float uy = ((ax * ax + ay * ay) * (cx - bx) + (bx * bx + by * by) * (ax - cx) + (cx * cx + cy * cy) * (bx - ax)) / d;

        center = new Vector2(ux, uy);
        radius = Vector2.Distance(center, p1);
    }

    private void DrawTriangles(List<Triangle> triangles, Color color, float duration)
    {
        if (triangles == null) return;
        foreach (var triangle in triangles)
        {
            Debug.DrawLine(triangle.p1, triangle.p2, color, duration);
            Debug.DrawLine(triangle.p2, triangle.p3, color, duration);
            Debug.DrawLine(triangle.p3, triangle.p1, color, duration);
        }
    }

    private void DrawCircumcircles(List<Triangle> triangles, Color color, float duration)
    {
        foreach (var triangle in triangles)
        {
            Vector2 center;
            float radius;
            GetCircumcircle(triangle, out center, out radius);

            // Draw the circumcircle
            Vector2 start = new Vector2(center.x + radius, center.y);
            for (int i = 0; i < 360; i++)
            {
                float angle = i * Mathf.Deg2Rad;
                Vector2 end = new Vector2(center.x + radius * Mathf.Cos(angle), center.y + radius * Mathf.Sin(angle));
                Debug.DrawLine(start, end, color, duration);
                start = end;
            }
        }
    }

    private void DrawFilledTriangles(List<Triangle> triangles)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> trianglesList = new List<int>();
        List<Color> colors = new List<Color>();

        for (int i = 0; i < triangles.Count; i++)
        {
            Triangle triangle = triangles[i];
            vertices.Add(triangle.p1);
            vertices.Add(triangle.p2);
            vertices.Add(triangle.p3);
            trianglesList.Add(i * 3);
            trianglesList.Add(i * 3 + 1);
            trianglesList.Add(i * 3 + 2);

            Color color = GetGradientColor(i, triangles.Count);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = trianglesList.ToArray();
        mesh.colors = colors.ToArray();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        meshRenderer.material = new Material(shader:gradiantShader);
    }

    private Color GetGradientColor(int index, int total)
    {
        float t = (float)index / (total - 1);
        return Color.HSVToRGB(t,1,1);
    }

    class Triangle
    {
        // Triangle is made up of 3 points
        public Vector2 p1, p2, p3;
        public Triangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
    }
}
