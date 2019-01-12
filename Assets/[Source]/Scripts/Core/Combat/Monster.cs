using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;
using Sirenix.OdinInspector;
using Jext;

public class Monster : Combatant {

    [InlineEditor]
    public ViewSet viewSet, attackSet;

    public override void OnActiveCombatant()
    {
        List<Combatant> combatants = GameManager.instance.CombatManager.Combatants;

        foreach (Combatant combatant in combatants)
        {
            if (combatant.teamID == teamID)
                continue;
            if (!CanSee(combatant, viewSet))
                continue;

            OnVisibleOpponent(combatant);
            return;
        }

        OnIdle();
    }

    public virtual bool CanSee(Combatant other, ViewSet set)
    {
        Vector2Int convertedPosition;

        foreach(View view in set.views)
        {
            convertedPosition = view.ConvertGridToMovePosition(other.Node.Position, Node.Position);
            if (view.footPrint[convertedPosition.x, convertedPosition.y])
            {
                if (!view.directViewRequired)
                    return true;
                if (GameManager.instance.CanSee(Node.Position, other.Node.Position))
                    return true;
            }
        }
        return false;
    }

    public virtual bool CanAttack(Combatant other)
    {
        return CanSee(other, attackSet);
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