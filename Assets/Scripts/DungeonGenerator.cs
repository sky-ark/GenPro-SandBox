using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    public BSP bsp;
    public Delaunay delaunay;
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Tilemap doorTilemap;
    public Tile floorTile;
    public Tile wallTile;
    public Tile doorTile;
    public List<Tile> coloredFloorTiles;
    public int doorLength = 1; // Longueur de la porte
    public float doorOffset = 0.5f; // Position de la porte par rapport à la largeur/longueur (0.5 = au milieu)

    [ContextMenu("Generate Dungeon")]
    private void GenerateDungeon()
    {
        // Clear previous grid
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        doorTilemap.ClearAllTiles();
        // Call bsp to launch the rectangles calculation
        bsp.Launch();
        // Get the rectangles from the BSP script
        List<Rect> rects = bsp.GetRects();
        // Generate points from the centers of the rectangles
        List<Vector2> points = delaunay.GeneratePointsFromRects(rects);
        // Perform Delaunay triangulation
        List<Delaunay.Triangle> delaunayTriangles = delaunay.Triangulation(points);
        // Generate edges from points
        List<Delaunay.Edge> edges = delaunay.GenerateEdges(points);
        // Perform Kruskal's algorithm
        List<Delaunay.Edge> mst = delaunay.KruskalMST(edges);
        // Draw walls
        DrawWalls(rects);
        // Draw rooms
        DrawRooms(rects);
        // Draw doors
        DrawDoors(mst, points, rects);
        // Draw MST
        delaunay.DrawMST(mst, points);
    }

    private void DrawRooms(List<Rect> rects)
    {
        int colorIndex = 0;
        foreach (var rect in rects)
        {
            Tile currentTile = coloredFloorTiles[colorIndex % coloredFloorTiles.Count];
            for (int x = (int)rect.x; x < rect.x + rect.width; x++)
            {
                for (int y = (int)rect.y; y < rect.y + rect.height; y++)
                {
                    floorTilemap.SetTile(new Vector3Int(x, y, 0), currentTile);
                }
            }

            colorIndex++;
        }
    }

    private void DrawWalls(List<Rect> rects)
    {
        foreach (var rect in rects)
        {
            for (int x = (int)rect.x; x < rect.x + rect.width; x++)
            {
                // Top wall
                Vector3Int topPos = new Vector3Int(x, (int)rect.y, 0);
                if (floorTilemap.GetTile(topPos) == null)
                {
                    wallTilemap.SetTile(topPos, wallTile);
                }

                // Bottom wall
                Vector3Int bottomPos = new Vector3Int(x, (int)(rect.y + rect.height - 1), 0);
                if (floorTilemap.GetTile(bottomPos) == null)
                {
                    wallTilemap.SetTile(bottomPos, wallTile);
                }
            }

            for (int y = (int)rect.y; y < rect.y + rect.height; y++)
            {
                // Left wall
                Vector3Int leftPos = new Vector3Int((int)rect.x, y, 0);
                if (floorTilemap.GetTile(leftPos) == null)
                {
                    wallTilemap.SetTile(leftPos, wallTile);
                }

                // Right wall
                Vector3Int rightPos = new Vector3Int((int)(rect.x + rect.width - 1), y, 0);
                if (floorTilemap.GetTile(rightPos) == null)
                {
                    wallTilemap.SetTile(rightPos, wallTile);
                }
            }
        }
    }

    private void DrawDoors(List<Delaunay.Edge> mst, List<Vector2> points, List<Rect> rects)
    {
        foreach (var edge in mst)
        {
            Vector2 pointA = points[edge.src];
            Vector2 pointB = points[edge.dest];

            Rect rectA = rects[edge.src];
            Rect rectB = rects[edge.dest];

            if (IsVertical(rectA, rectB))
            {
                Debug.Log($"Placing vertical door between rectA: {rectA} and rectB: {rectB}");
                int x = (int)(rectA.x + rectA.width);
                int startY = (int)(rectA.y + rectA.height * doorOffset);
                int endY = startY + doorLength;
                for (int y = startY; y < endY; y++)
                {
                    PlaceDoor(x, y);
                    PlaceDoor(x - 2, y); // Place door on the adjacent wall
                }
            }
            else if (IsHorizontal(rectA, rectB))
            {
                Debug.Log($"Placing horizontal door between rectA: {rectA} and rectB: {rectB}");
                int y = (int)(rectA.y + rectA.height);
                int startX = (int)(rectA.x + rectA.width * doorOffset);
                int endX = startX + doorLength;
                for (int x = startX; x < endX; x++)
                {
                    PlaceDoor(x, y);
                    PlaceDoor(x, y - 2); // Place door on the adjacent wall
                }
            }
        }
    }

    public float tolerance = 0.1f; // Tolerance value

    private bool IsVertical(Rect rectA, Rect rectB)
    {
        bool result = Math.Abs(rectA.x - (rectB.x + rectB.width)) < tolerance || Math.Abs(rectA.x + rectA.width - rectB.x) < tolerance;
        Debug.Log($"IsVertical: {result} (rectA: {rectA}, rectB: {rectB})");
        return result;
    }

    private bool IsHorizontal(Rect rectA, Rect rectB)
    {
        bool result = Math.Abs(rectA.y - (rectB.y + rectB.height)) < tolerance || Math.Abs(rectA.y + rectA.height - rectB.y) < tolerance;
        Debug.Log($"IsHorizontal: {result} (rectA: {rectA}, rectB: {rectB})");
        return result;
    }

    private void PlaceDoor(int x, int y)
    {
        Debug.Log($"Placing door at {x}, {y}");
        Vector3Int doorPosition = new Vector3Int(x, y, 0);
        if (wallTilemap.GetTile(doorPosition) == wallTile)
        {
            doorTilemap.SetTile(doorPosition, doorTile);
        }
    }
}
