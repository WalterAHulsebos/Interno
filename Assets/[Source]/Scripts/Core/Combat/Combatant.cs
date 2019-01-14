using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Combat;
using Core;

public abstract class Combatant : Filler, ICombatable, IDamageable
{
    public int CombatOrder { get; protected set; }

    public List<DamageTypes> Immunities { get; set; } = new List<DamageTypes>();

    public int teamID, health;
    protected int currentHealth;

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

    public virtual void OnDamageReceived(int damage, DamageTypes damageType)
    {
        if (Immunities.Contains(damageType))
            return;
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