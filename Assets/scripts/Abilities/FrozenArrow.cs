using UnityEngine;
using System.Collections;

public class FrozenArrow : Arrow {


    public UnitUpgrade.Modificator modificator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
            Fly();
	}


    //заморозить
    public override void OnHitTarget()
    {
        if (target.GetComponent<GameUnit>() != null)
        {
            GameUnit _unit = target.GetComponent<GameUnit>();

            _unit.SetDamage(damage,ownerUnit);
            _unit.modificators.Add(modificator);
        }

        base.OnHitTarget();
    }

    public override void StartAbility()
    {
        base.StartAbility();
    }
}
