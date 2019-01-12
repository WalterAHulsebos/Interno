using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public abstract class Combatant : Filler, ICombatable
{
    public int CombatOrder { get; protected set; }
    public int teamID, health;

    public abstract int CompareTo(ICombatable other);
    public abstract void OnActiveCombatant();

    protected override void NotifyExistence()
    {
        base.NotifyExistence();
        GameManager.instance.CombatManager.Add(this);
    }

    protected override void NotifyDestruction()
    {
        base.NotifyDestruction();
        GameManager.instance.CombatManager.Remove(this);
    }

    protected virtual void EndTurn()
    {
        GameManager.instance.CombatManager.Next();
    }
}