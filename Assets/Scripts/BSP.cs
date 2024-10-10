using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BSP : MonoBehaviour
{
    private Rect room;
    public int seed;
    public int depth = 5;
    private Random random;
    public float minClamp = .2f, maxClamp = .8f;

    private void Start()
    {
        random = new Random(seed);
    }

    [ContextMenu("Launch")]
    public void Launch()
    {
        Rect root = new Rect(0, 0, 100, 100);

        List<Rect> rects = new List<Rect>();
        rects.Add(root);
        rects = _recursiveSplit(rects, depth, true);

        foreach (Rect rect in rects)
        {
            Debug.Log(rect);
            DrawRoom(rect);
            Vector2 center = GetRoomsCenter(rect);
            Debug.Log($"Center of {rect}: {center}");
            DrawCenter(center);
        }
    }

    public List<Rect> _recursiveSplit(List<Rect> rects, int depth, bool splitVertically)
    {
        if (depth == 0) return rects;

        // Chose the biggest room to cut
        Rect roomToCut = GetLargestRoom(rects);

        // Cut the room
        (Rect, Rect) tuple = Split(roomToCut, seed, splitVertically);

        // Remove the cutted room
        rects.Remove(roomToCut);

        // Add the new rooms
        rects.Add(tuple.Item1);
        rects.Add(tuple.Item2);

        // Go to next iteration
        return _recursiveSplit(rects, depth - 1, !splitVertically);
    }

    (Rect, Rect) Split(Rect room, int seed, bool splitVertically)
    {
        Random random = new Random(seed);

        float min = splitVertically ? room.width * minClamp : room.height * minClamp;
        float max = splitVertically ? room.width * maxClamp : room.height * maxClamp;
        int split = splitVertically ? (int)(room.x + random.NextDouble() * (max - min) + min)
                                    : (int)(room.y + random.NextDouble() * (max - min) + min);

        Rect firstPart = splitVertically ? new Rect(room.x, room.y, split - room.x, room.height)
                                         : new Rect(room.x, room.y, room.width, split - room.y);

        Rect secondPart = splitVertically ? new Rect(split, room.y, room.width - (split - room.x), room.height)
                                          : new Rect(room.x, split, room.width, room.height - (split - room.y));

        Debug.Log($"First: {firstPart}");
        Debug.Log($"Second: {secondPart}");

        return (firstPart, secondPart);
    }

    Rect GetLargestRoom(List<Rect> rooms)
    {
        Rect largestRoom = rooms[0];
        float largestArea = rooms[0].width * rooms[0].height;

        foreach (Rect room in rooms)
        {
            float area = room.width * room.height;
            if (area > largestArea)
            {
                largestRoom = room;
                largestArea = area;
            }
        }

        return largestRoom;
    }

    void DrawRoom(Rect room)
    {
        //draw room
        Vector3 topLeft = new Vector3(room.x, room.y, 0);
        Vector3 topRight = new Vector3(room.x + room.width, room.y, 0);
        Vector3 bottomLeft = new Vector3(room.x, room.y + room.height, 0);
        Vector3 bottomRight = new Vector3(room.x + room.width, room.y + room.height, 0);

        Debug.DrawLine(topLeft, topRight, Color.red, 10f);
        Debug.DrawLine(topRight, bottomRight, Color.red, 10f);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red, 10f);
        Debug.DrawLine(bottomLeft, topLeft, Color.red, 10f);
    }

    private Vector2 GetRoomsCenter(Rect rect)
    {
        return new Vector2(rect.x + rect.width / 2, rect.y + rect.height / 2);
    }

    private void DrawCenter(Vector2 center)
    {
        float crossSize = 5f;
        Vector3 horizontalStart = new Vector3(center.x - crossSize, center.y, 0);
        Vector3 horizontalEnd = new Vector3(center.x + crossSize, center.y, 0);
        Vector3 verticalStart = new Vector3(center.x, center.y - crossSize, 0);
        Vector3 verticalEnd = new Vector3(center.x, center.y + crossSize, 0);

        Debug.DrawLine(horizontalStart, horizontalEnd, Color.blue, 10f);
        Debug.DrawLine(verticalStart, verticalEnd, Color.blue, 10f);
    }
}
