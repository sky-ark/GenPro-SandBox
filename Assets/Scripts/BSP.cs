using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BSP : MonoBehaviour
{
    private Rect room;
    public int seed;
    public int depth = 5;
    public float firstRoomWidth = 100, firstRoomHeight = 100;
    private Random random;
    public bool FirstSplitRandom = true;
    public float minClamp = .2f, maxClamp = .8f;

    public enum RoomSelectionStrategy
    {
        LargestArea,
        LargestDifference,
        Random
    }

    public RoomSelectionStrategy roomSelectionStrategy = RoomSelectionStrategy.LargestArea;

    public enum SplitStrategy
    {
        Alternate,
        Random,
        LongestSide
    }
    [Tooltip("Split's direction Strategy")]
    public SplitStrategy splitStrategy = SplitStrategy.Alternate;

    private bool splitVertically = true;

    private void Start()
    {
        random = new Random(seed);
    }

    [ContextMenu("Launch")]
    public void Launch()
    {
        Rect root = new Rect(0, 0, firstRoomWidth, firstRoomHeight);

        List<Rect> rects = new List<Rect>();
        rects.Add(root);

        // if Random is true, then we will randomly choose between vertical and horizontal split, otherwise we will always split vertically
        splitVertically = FirstSplitRandom ? random.Next(0, 2) == 1 : true;
        rects = _recursiveSplit(rects, depth);

        foreach (Rect rect in rects)
        {
            Debug.Log(rect);
            DrawRoom(rect);
            Vector2 center = GetRoomsCenter(rect);
            Debug.Log($"Center of {rect}: {center}");
            DrawCenter(center);
        }
    }

    public List<Rect> _recursiveSplit(List<Rect> rects, int depth)
    {
        if (depth == 0) return rects;

        // Chose the next room to cut based on the selected strategy
        Rect roomToCut = GetNextRoomToCut(rects);

        // Determine the splitting strategy
        bool currentSplitVertically = GetSplitDirection(roomToCut);

        // Cut the room
        (Rect, Rect) tuple = Split(roomToCut, currentSplitVertically, depth);

        // Remove the cutted room
        rects.Remove(roomToCut);

        // Add the new rooms
        rects.Add(tuple.Item1);
        rects.Add(tuple.Item2);

        // Go to next iteration
        return _recursiveSplit(rects, depth - 1);
    }

    private bool GetSplitDirection(Rect room)
    {
        switch (splitStrategy)
        {
            case SplitStrategy.Alternate:
                // Alternate between vertical and horizontal splits
                splitVertically = !splitVertically;
                return splitVertically;
            case SplitStrategy.Random:
                // Randomly choose between vertical and horizontal splits
                return random.Next(0, 2) == 1;
            case SplitStrategy.LongestSide:
                // Split along the longest side
                return room.width > room.height;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    Rect GetNextRoomToCut(List<Rect> rooms)
    {
        switch (roomSelectionStrategy)
        {
            case RoomSelectionStrategy.LargestArea:
                return GetLargestRoomByArea(rooms);
            case RoomSelectionStrategy.LargestDifference:
                return GetLargestRoomByDifference(rooms);
            case RoomSelectionStrategy.Random:
                return GetRandomRoom(rooms);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    Rect GetLargestRoomByArea(List<Rect> rooms)
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

    Rect GetLargestRoomByDifference(List<Rect> rooms)
    {
        Rect largestRoom = rooms[0];
        float largestDifference = Mathf.Abs(rooms[0].width - rooms[0].height);

        foreach (Rect room in rooms)
        {
            float difference = Mathf.Abs(room.width - room.height);
            if (difference > largestDifference)
            {
                largestRoom = room;
                largestDifference = difference;
            }
        }

        return largestRoom;
    }

    Rect GetRandomRoom(List<Rect> rooms)
    {
        return rooms[random.Next(rooms.Count)];
    }

    (Rect, Rect) Split(Rect room, bool splitVertically, int depth)
    {
        Random splitRandom = new Random(seed + depth);

        float min = splitVertically ? room.width * minClamp : room.height * minClamp;
        float max = splitVertically ? room.width * maxClamp : room.height * maxClamp;

        // Ensure the split point is within the range [min, max]
        float splitPoint = splitVertically
            ? room.x + (float)(splitRandom.NextDouble() * (max - min) + min)
            : room.y + (float)(splitRandom.NextDouble() * (max - min) + min);

        Rect firstPart, secondPart;

        if (splitVertically)
        {
            firstPart = new Rect(room.x, room.y, splitPoint - room.x, room.height);
            secondPart = new Rect(splitPoint, room.y, room.width - (splitPoint - room.x), room.height);
        }
        else
        {
            firstPart = new Rect(room.x, room.y, room.width, splitPoint - room.y);
            secondPart = new Rect(room.x, splitPoint, room.width, room.height - (splitPoint - room.y));
        }

        Debug.Log($"First: {firstPart}");
        Debug.Log($"Second: {secondPart}");

        return (firstPart, secondPart);
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
