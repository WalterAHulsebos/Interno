using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Pathfinding;
using Sirenix.OdinInspector;

public class Filler : MonoBehaviour, IMoveable<Node>
{
    [InlineEditor]
    public MoveSet moveSet;
    public bool walkable;

    public Node Node { get; private set; }
    public bool HorizontalMovement { get; private set; }
    public bool VerticalMovement { get; private set; }

    protected virtual void Awake()
    {
        NotifyExistence();
    }

    public virtual bool Walkable(Node node)
    {
        List<Combatant> combatants = GameManager.instance.CombatManager.Combatants;
        foreach (Combatant combatant in combatants)
            if (combatant.Node == node)
                return false;
        return true;
    }

    protected virtual void NotifyExistence()
    {
        GameManager.instance.filler.Add(this);
    }

    protected virtual void NotifyDestruction()
    {
        GameManager.instance.filler.Remove(this);
    }
}
