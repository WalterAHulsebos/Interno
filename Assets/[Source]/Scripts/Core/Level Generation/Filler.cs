using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;

public class Filler : IMoveable<Node>
{
    public Node Position { get; private set; }
    public bool HorizontalMovement { get; private set; }
    public bool VerticalMovement { get; private set; }

    public virtual bool Walkable(Node node)
    {
        return false;
    }
}
