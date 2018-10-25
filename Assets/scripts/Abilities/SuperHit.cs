using UnityEngine;
using System.Collections;

//удар, поражающий всех юнитов в округе
public class SuperHit : Ability {

    //начало
    public override void StartAbility()
    {
        base.StartAbility();
        Ability.BangDamage(transform.position, damage, maxRange, ownerUnit);
        Destroy(gameObject);
    }
}
