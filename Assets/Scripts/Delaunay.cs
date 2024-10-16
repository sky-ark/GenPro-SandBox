using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delaunay : MonoBehaviour
{
    public BSP bsp;
    private List<Rect> rects;
    private List<Vector2> points;
    private List<Triangle> triangles;
    
    [ContextMenu("Launch")]
    private void Launch()
    {
        //Call bsp to launch the rectangles calculation
        bsp.Launch();
        //Get the rectangles from the BSP script
        rects = bsp.GetRects();
        //Generate points from the centers of the rectangles
        GeneratePointsFromRects();
        //Perform Delaunay triangulation
        Triangulation();
        //Draw triangles (for visualization purposes)
        DrawTriangles();
    }
    //Get the rectangles from the BSP script
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
        triangles = new List<Triangle>();

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = 0; j < points.Count; j++)
            {
                if ( i!= j ) 
                {
                    for (int k = 0; k < points.Count; k++)
                    {
                        if (j != k && i != k)
                        {
                            Vector2 p1 = points[i];
                            Vector2 p2 = points[j];
                            Vector2 p3 = points[k];
                            Triangle triangle = new Triangle(p1, p2, p3);
                            triangles.Add(triangle);
                        }
                    }
                }
            }
        }
    }
    
    private void DrawTriangles()
    {
        if (triangles == null) return;
        foreach (var triangle in triangles)
        {
            Debug.DrawLine(triangle.p1, triangle.p2, Color.yellow);
            Debug.DrawLine(triangle.p2, triangle.p3, Color.yellow);
            Debug.DrawLine(triangle.p3, triangle.p1,Color.yellow);
        }
    }

    class Triangle
    {
        //Triangle is made up of 3 points
        public Vector2 p1, p2, p3;
        public Triangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
    }
    
}
