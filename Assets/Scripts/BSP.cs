using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BSP : MonoBehaviour
{
    private Rect room;
    public int seed;
    public int depth = 5;
    
    public void Start()
    {
        Rect root = new Rect(0, 0, 100, 100);

        List<Rect> rects = new List<Rect>();
        rects.Add(root);
        rects = _recursiveSplit(rects, depth, true);
        Debug.Log(rects);
        foreach (Rect rect in rects)
        {
            DrawRoom(rect);
        }
        
        
        //DisplayRooms();
    }
    
    public List<Rect> _recursiveSplit(List<Rect> rects, int depth, bool splitVertically) {
        if (depth == 0) return rects;
        // Chose randomly a room to cut 
        // Rect roomToCut = rects[UnityEngine.Random.Range(0, rects.Count)];
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
        Random random = new Random(seed);
       // bool splitVertically = random.Next(0, 2) == 0; // random 0 or 1 to decide if we split vertically or horizontally
        (Rect, Rect) splitRooms;
        if (splitVertically)
        {
            splitRooms = SplitVertically(room, random);
        }
        else
        {
            splitRooms = SplitHorizontally(room, random);
        }

        return splitRooms;
    }
    
    (Rect, Rect) SplitVertically(Rect room, Random random)
    {
        List<Rect> rects = new List<Rect>();
        //this will cut randomly :
        //int split = random.Next((int)room.x, (int)room.width);
        //this will cut in the middle :
        //int split = (int)room.width / 2;
        //this will cut in the middle but with a random offset
        int split = (int)room.width / 2 + random.Next(-10, 10);
        Rect left = new Rect(room.x, room.y, split, room.height);
        Rect right = new Rect(split, room.y, room.width - split, room.height);
        Debug.Log("Left: " + left);
        Debug.Log("Right: " + right);
        
        /*//Calculate areas
        
        float leftArea = left.width * left.height;
        float rightArea = right.width * right.height;
        
        //chose the greatest rect
        
        Rect greatestRect = leftArea > rightArea ? left : right;
        
        //check if it's not too small to split 
        if (greatestRect.width > minValue && greatestRect.height > minValue)
        {
            SplitHorizontally(greatestRect, random);
        }*/
      

        return (left, right);
    }

    (Rect, Rect) SplitHorizontally(Rect room, Random random)
    {
        //this will cut randomly :
        //int split = random.Next((int)room.y, (int)room.height);
        //this will cut in the middle :
        //int split = (int)room.height / 2;
        //this will cut in the middle but with a random offset
        int split = (int)room.height / 2 + random.Next(-10, 10);
        Rect top = new Rect(room.x, room.y, room.width, split); 
        Rect bottom = new Rect(room.x, split, room.width, room.height - split);
        Debug.Log("Top: " + top);
        Debug.Log("Bottom: " + bottom);
        
        /*float topArea = top.width * top.height;
        float bottomArea = bottom.width * bottom.height;
        
        Rect greatestRect = topArea > bottomArea ? top : bottom;
        
        if (greatestRect.width > minValue && greatestRect.height > minValue)
        {
            SplitVertically(greatestRect, random);
        }*/
        
        return (top, bottom);
    }
    

   Rect GetLargestRoom(List<Rect> rooms) {
        Rect largestRoom = rooms[0];
        foreach (Rect room in rooms) {
            if (room.x * room.y > largestRoom.x * largestRoom.y) {
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

    /*private void OnDrawGizmos()
    {
        foreach (var VARIABLE in roo)
        {
            throw new NotImplementedException();
        }
    }*/
}
