using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delaunay : MonoBehaviour
{
    public BSP bsp;
    private List<Rect> rects;
    private List<Vector2> points;
    private List<Triangle> allTriangles;
    private List<Triangle> delaunayTriangles;
    private List<Edge> edges;

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
        // Generate edges from points
        GenerateEdges();
        // Perform Kruskal's algorithm
        List<Edge> mst = KruskalMST(edges);
        // Draw all triangles and circles for 3 seconds
        // DrawTriangles(allTriangles, Color.yellow, 3f);
        // DrawCircumcircles(allTriangles, Color.red, 3f);
        // Draw only Delaunay triangles for 10 seconds
        DrawTriangles(delaunayTriangles, Color.green, 10f);
        // Draw the minimum spanning tree
        DrawMST(mst, Color.magenta, 10f);

        // DrawCircumcircles(delaunayTriangles, Color.magenta, 10f);
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

        }
    }

    private void GenerateEdges()
    {
        edges = new List<Edge>();

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                Vector2 p1 = points[i];
                Vector2 p2 = points[j];
                float weight = Vector2.Distance(p1, p2);
                edges.Add(new Edge(i, j, weight));
            }
        }
    }

    private List<Edge> KruskalMST(List<Edge> edges)
    {
        List<Edge> result = new List<Edge>();
        int i = 0;
        int e = 0;

        // Sort all the edges in non-decreasing order of their weight
        edges.Sort();

        // Create a parent array to keep track of the subsets
        int[] parent = new int[points.Count];
        for (int v = 0; v < points.Count; ++v)
            parent[v] = v;

        // Number of edges to be taken is equal to V-1
        while (e < points.Count - 1)
        {
            // Pick the smallest edge and increment the index for next iteration
            Edge nextEdge = edges[i++];

            int x = Find(parent, nextEdge.src);
            int y = Find(parent, nextEdge.dest);

            // If including this edge does not cause cycle, include it in result and increment the index of result for next edge
            if (x != y)
            {
                result.Add(nextEdge);
                e++;
                Union(parent, x, y);
            }
        }

        return result;
    }

    private int Find(int[] parent, int i)
    {
        if (parent[i] == i)
            return i;
        return Find(parent, parent[i]);
    }

    private void Union(int[] parent, int x, int y)
    {
        int xroot = Find(parent, x);
        int yroot = Find(parent, y);

        parent[xroot] = yroot;
    }

    private void DrawMST(List<Edge> mst, Color color, float duration)
    {
        foreach (var edge in mst)
        {
            Vector2 p1 = points[edge.src];
            Vector2 p2 = points[edge.dest];
            Debug.DrawLine(p1, p2, color, duration);
        }
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

    class Edge : IComparable<Edge>
    {
        public int src, dest;
        public float weight;

        public Edge(int src, int dest, float weight)
        {
            this.src = src;
            this.dest = dest;
            this.weight = weight;
        }

        public int CompareTo(Edge compareEdge)
        {
            return this.weight.CompareTo(compareEdge.weight);
        }
    }
}
