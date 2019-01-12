using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;
using Core.Pathfinding;
using Sirenix.OdinInspector;

public abstract class Combatant : MonoBehaviour, ICombatable, IMoveable<Node>
{
    [InlineEditor]
    public MoveSet moveSet;

    public Node Node { get; private set; }
    public bool HorizontalMovement { get; private set; }
    public bool VerticalMovement { get; private set; }

    public abstract int CompareTo(ICombatable other);
    public abstract void OnActiveCombatant();
    public abstract bool Walkable(Node node);
}