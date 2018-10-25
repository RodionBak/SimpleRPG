using UnityEngine;
using System.Collections;

//лучник
public class MinionArcher : Minion {

	// Use this for initialization
	void Start () {
        InitMinion();
	}
	
	// Update is called once per frame
	void Update () {
        MinionAIProcess();
    }

    public override void OnHitTarget()
    {
        //стрелять стрелой
        if (abilities.Count > 0)
        {
            abilities[0].point = transform.position;
            abilities[0].damage = damage;
            abilities[0].target = Target.transform;
            CastAbility(abilities[0]);
        }else
        {
            SetDamage(damage, this);
        }
    }
}
