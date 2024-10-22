using System;
using System.Collections.Generic;
using UnityEngine;

public class Delaunay : MonoBehaviour
{
    public BSP bsp;
    private List<Rect> rects;
    public List<Vector2> points;
    private List<Triangle> allTriangles;
    private List<Triangle> delaunayTriangles;
    private List<Edge> edges;

    public List<Vector2> GeneratePointsFromRects(List<Rect> rects)
    {
        points = new List<Vector2>();

        foreach (var rect in rects)
        {
            Vector2 center = new Vector2(rect.x + rect.width / 2, rect.y + rect.height / 2);
            points.Add(center);
        }

        return points;
    }

    public List<Triangle> Triangulation(List<Vector2> points)
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
                            if (IsDelaunay(triangle, points))
                            {
                                delaunayTriangles.Add(triangle);
                            }
                        }
                    }
                }
            }
        }

        return delaunayTriangles;
    }

    private bool IsDelaunay(Triangle triangle, List<Vector2> points)
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

    public List<Edge> GenerateEdges(List<Vector2> points)
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

        return edges;
    }

    public List<Edge> KruskalMST(List<Edge> edges)
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

    public class Triangle
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

    public class Edge : IComparable<Edge>
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
    
    public void DrawMST(List<Delaunay.Edge> mst, List<Vector2> points)
    {
        foreach (var edge in mst)
        {
            Vector2 pointA = points[edge.src];
            Vector2 pointB = points[edge.dest];

            Debug.DrawLine(new Vector3(pointA.x, pointA.y, 0), new Vector3(pointB.x, pointB.y, 0), Color.green, 10f);
        }
    }
}


