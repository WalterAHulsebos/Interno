using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    List<DamageTypes> Immunities { get; }
    void OnDamageReceived(int damage, DamageTypes damageType);
}

public enum DamageTypes {Normal, Ball, Beam }
