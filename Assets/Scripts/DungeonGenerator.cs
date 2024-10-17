using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    public BSP bsp;
    public Delaunay delaunay;
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Tile floorTile;
    public Tile wallTile;
    public Tile corridorTile;
    public Tile doorTile;
    public List<Tile> coloredFloorTiles; // Liste de tuiles colorées

    [ContextMenu("Generate Dungeon")]
    private void GenerateDungeon()
    {
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

        // Draw rooms
        DrawRooms(rects);
        // Draw corridors and doors
        DrawCorridorsAndDoors(mst, points);
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

    private void DrawCorridorsAndDoors(List<Delaunay.Edge> mst, List<Vector2> points)
    {
        // Implementation of drawing corridors and doors
    }

    private void DrawCorridor(Vector2 p1, Vector2 p2)
    {
        // Implementation of drawing a corridor
    }

    private void DrawDoor(Vector2 p1, Vector2 p2)
    {
        // Implementation of drawing a door
    }
}
