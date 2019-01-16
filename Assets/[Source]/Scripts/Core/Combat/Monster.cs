using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Combat;
using Sirenix.OdinInspector;
using Core.Jext;

public class Monster : Combatant
{
    [SerializeField, InlineEditor]
    protected ViewSet viewSet;
    [SerializeField]
    protected AttackSet attackSet;

    public override void OnActiveCombatant()
    {
        List<Combatant> combatants = GameManager.instance.CombatManager.Combatants;

        foreach (Combatant combatant in combatants)
        {
            if (combatant.teamID == teamID)
                continue;
            if (!CanSee(combatant.Node, viewSet.views))
                continue;

            OnVisibleOpponent(combatant);
            return;
        }

        OnIdle();
    }
    
    public virtual bool CanAttack(Combatant other)
    {
        return CanSee(other.Node, attackSet.attacks);
    }    


    public override int CompareTo(ICombatable other)
    {
        return CombatOrder - other.CombatOrder;
    }

    public virtual void OnIdle()
    {
        EndTurn();
    }

    public virtual void OnVisibleOpponent(Combatant combatant)
    {
        if(CanAttack(combatant))
        {
            OnAttack(combatant);
            return;
        }

        OnMoveTowards(combatant);
    }

    public virtual void OnMoveTowards(Combatant combatant)
    {
        List<Node> path = GameManager.instance.GetPath(combatant, combatant.Node.Position);
        // DO MOVE FROM PATH
    }

    public virtual void OnAttack(Combatant combatant)
    {
        // DO ATTACK THEN END TURN
    }
}