using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;
using Sirenix.OdinInspector;
using Core.Jext;

public class Monster : Combatant {

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
            if (!CanSee(combatant, viewSet.views))
                continue;

            OnVisibleOpponent(combatant);
            return;
        }

        OnIdle();
    }

    public virtual bool CanSee<T>(Combatant other, List<T> set) where T : Move
    {
        Vector2Int convertedPosition;

        foreach(T t in set)
        {
            convertedPosition = t.ConvertGridToMovePosition(other.Node.Position, Node.Position);

            if (t.footPrint[convertedPosition.x, convertedPosition.y])
            {
                if (!t.directConnectionRequired)
                    return true;
                if (GameManager.instance.CanSee(Node.Position, other.Node.Position))
                    return true;
            }
        }
        return false;
    }

    public virtual bool CanAttack(Combatant other)
    {
        return CanSee(other, attackSet.attacks);
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