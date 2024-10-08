using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BSP : MonoBehaviour
{
    private Rect room;
    public int seed;
    public int minValue = 5;
    
    public void Start()
    {
        Rect root = new Rect(0, 0, 100, 100);
        Split(root, 0);
    }

    void Split(Rect room, int seed)
    {
        Random random = new Random(seed);
        bool splitVertically = random.Next(0, 2) == 0;
        if (splitVertically)
        {
            SplitVertically(room, random);
        }
        else
        {
            SplitHorizontally(room, random);
        }
    }
    
    void SplitVertically(Rect room, Random random)
    {
        int split = random.Next((int)room.x, (int)room.width);
        Rect left = new Rect(room.x, room.y, split, room.height);
        Rect right = new Rect(split, room.y, room.width - split, room.height);
        Debug.Log("Left: " + left);
        Debug.Log("Right: " + right);
        
        //Calculate areas
        
        float leftArea = left.width * left.height;
        float rightArea = right.width * right.height;
        
        //chose the greatest rect
        
        Rect greatestRect = leftArea > rightArea ? left : right;
        
        //check if it's not too small to split 
        if (greatestRect.width > minValue && greatestRect.height > minValue)
        {
            SplitHorizontally(greatestRect, random);
        }
        else return; //stop splitting
    }
    
    void SplitHorizontally(Rect room, Random random)
    {
        int split = random.Next((int)room.y, (int)room.height);
        Rect top = new Rect(room.x, room.y, room.width, split);
        Rect bottom = new Rect(room.x, split, room.width, room.height - split);
        Debug.Log("Top: " + top);
        Debug.Log("Bottom: " + bottom);
        
        float topArea = top.width * top.height;
        float bottomArea = bottom.width * bottom.height;
        
        Rect greatestRect = topArea > bottomArea ? top : bottom;
        
        if (greatestRect.width > minValue && greatestRect.height > minValue)
        {
            SplitVertically(greatestRect, random);
        }
        else return;
    }
}
