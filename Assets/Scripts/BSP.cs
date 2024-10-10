using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BSP : MonoBehaviour
{
    private Rect room;
    public int seed;
    public int depth = 5;
    private Random random;


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
        Debug.Log(rects);
        foreach (Rect rect in rects)
        {
            DrawRoom(rect);
            Vector2 center = GetRoomsCenter(rect);
            DrawCenter(center);
        }
    }
    
    public List<Rect> _recursiveSplit(List<Rect> rects, int depth, bool splitVertically) {
        if (depth == 0) return rects;
        // Chose the biggest room to cut
        Rect roomToCut = GetLargestRoom(rects);
        // Cut the room
        (Rect, Rect) tuple = Split(roomToCut,seed, splitVertically);
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
       // bool splitVertically = random.Next(0, 2) == 0; // random 0 or 1 to decide if we split vertically or horizontally
        if (splitVertically)
        {        
            //this will cut in the middle but with a random offset
            int split = (int)room.width / 2 + random.Next(-10, 10);
            Rect left = new Rect(room.x, room.y, split, room.height);
            Rect right = new Rect(split, room.y, room.width - split, room.height);
            Debug.Log("Left: " + left);
            Debug.Log("Right: " + right);
            return (left, right);
        }
        else
        {
            //this will cut in the middle but with a random offset
            int split = (int)room.height / 2 + random.Next(-10, 10);
            Rect top = new Rect(room.x, room.y, room.width, split); 
            Rect bottom = new Rect(room.x, split, room.width, room.height - split);
            Debug.Log("Top: " + top);
            Debug.Log("Bottom: " + bottom);
            return (top, bottom);
        }
    }
    
   Rect GetLargestRoom(List<Rect> rooms) {
        Rect largestRoom = rooms[0];
        foreach (Rect room in rooms) {
            if (room.width * room.height > largestRoom.width * largestRoom.height) {
                largestRoom = room;
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
