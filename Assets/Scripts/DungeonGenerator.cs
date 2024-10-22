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
    public int doorWidth = 1; // New parameter to control door width

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
        DrawCorridors(mst, points);
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

   private void DrawCorridors(List<Delaunay.Edge> mst, List<Vector2> points)
{
    foreach (var edge in mst)
    {
        Vector2 pointA = points[edge.src];
        Vector2 pointB = points[edge.dest];

        int x0 = (int)pointA.x;
        int y0 = (int)pointA.y;
        int x1 = (int)pointB.x;
        int y1 = (int)pointB.y;

        // Calculer les distances sur les axes X et Y
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        // Calculer les longueurs des portes pour les deux alignements
        int doorLengthX = CalculateDoorLength(x0, y0, x1, y0, true);
        int doorLengthY = CalculateDoorLength(x0, y0, x0, y1, false);

        // Déterminer s'il vaut mieux aligner sur X ou Y
        bool alignOnX = doorLengthX <= doorLengthY;

        if (alignOnX)
        {
            // Aligner sur X
            int x = x0;
            for (int y = y0; y != y1; y += (y1 > y0 ? 1 : -1))
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                Debug.DrawLine(new Vector3(x, y, 0), new Vector3(x + 1, y, 0), Color.yellow, 10f);
                PlaceDoorIfWall(position);
            }
        }
        else
        {
            // Aligner sur Y
            int y = y0;
            for (int x = x0; x != x1; x += (x1 > x0 ? 1 : -1))
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                Debug.DrawLine(new Vector3(x, y, 0), new Vector3(x, y + 1, 0), Color.yellow, 10f);
                PlaceDoorIfWall(position);
            }
        }
    }
}

private int CalculateDoorLength(int x0, int y0, int x1, int y1, bool alignOnX)
{
    int doorLength = 0;

    if (alignOnX)
    {
        // Aligner sur X
        int x = x0;
        for (int y = y0; y != y1; y += (y1 > y0 ? 1 : -1))
        {
            Vector3Int position = new Vector3Int(x, y, 0);
            if (wallTilemap.GetTile(position) == wallTile)
            {
                doorLength++;
            }
        }
    }
    else
    {
        // Aligner sur Y
        int y = y0;
        for (int x = x0; x != x1; x += (x1 > x0 ? 1 : -1))
        {
            Vector3Int position = new Vector3Int(x, y, 0);
            if (wallTilemap.GetTile(position) == wallTile)
            {
                doorLength++;
            }
        }
    }

    return doorLength;
}

private void PlaceDoorIfWall(Vector3Int position)
{
    if (wallTilemap.GetTile(position) == wallTile)
    {
        doorTilemap.SetTile(position, doorTile);
    }
}



    /*private void DrawDoors(List<Delaunay.Edge> mst, List<Vector2> points)
    {
        foreach (var edge in mst)
        {
            Vector2 pointA = points[edge.src];
            Vector2 pointB = points[edge.dest];

            int x0 = (int)pointA.x;
            int y0 = (int)pointA.y;
            int x1 = (int)pointB.x;
            int y1 = (int)pointB.y;

            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                for (int i = -doorWidth / 2; i <= doorWidth / 2; i++)
                {
                    Vector3Int doorPosition = new Vector3Int(x0 + (dy == 0 ? 0 : i), y0 + (dx == 0 ? 0 : i), 0);
                    if (wallTilemap.GetTile(doorPosition) == wallTile)
                    {
                        doorTilemap.SetTile(doorPosition, doorTile);
                    }
                }

                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }
    }*/
}