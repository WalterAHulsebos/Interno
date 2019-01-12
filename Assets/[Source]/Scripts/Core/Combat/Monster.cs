using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combat;

public abstract class Monster : Combatant {

    public abstract bool CanSee(Combatant other);
    public abstract bool CanAttack(Combatant other);
}
