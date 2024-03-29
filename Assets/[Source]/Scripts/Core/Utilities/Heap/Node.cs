﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Heap;
using Core.Pathfinding;

public class Node : IHeapable<Node>, INodeable<Node>
{
    public int HeapIndex { get; set; }
    public Node Parent { get; set; }
    public Vector2Int Position { get; set; }
    public float G { get; set; }
    public float H { get; set; }
    public float Cost { get; set; }
    public Move Move { get; set; }
    public bool Walkable { get; set; }

    public float Value
    {
        get
        {
            try
            {
                return G + H + Cost + Move.weight;
            }
            catch
            {
                return G + H + Cost;
            }
        }
    }

    public int CompareTo(Node other)
    {
        return Mathf.RoundToInt(Value - other.Value);
    }
}
