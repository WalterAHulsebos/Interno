using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public abstract class Combatant : Filler, ICombatable, IDamagable
{
    public int CombatOrder { get; protected set; }
    public int teamID, health;
    private int currentHealth;

    public abstract int CompareTo(ICombatable other);
    public abstract void OnActiveCombatant();

    protected override void Awake()
    {
        base.Awake();
        currentHealth = health;
    }

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

    public void OnDamageReceived(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, health);

        if (currentHealth > 0)
            return;

        OnDeath();
    }

    protected virtual void OnDeath()
    {
        NotifyDestruction();
    }
}