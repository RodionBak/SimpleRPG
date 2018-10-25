using UnityEngine;
using System.Collections;

//Босс врагов
public class Boss : Hero {

	// Use this for initialization
	void Start () {
        InitHero();
	}

    public GameBody mainEnemy;

    // Update is called once per frame
    void Update()
    {
        if (!active) return;

        MinionAIProcess();

        //периодически использовать способности
        if (Target != null)
        {
            if ((Target.country != country) && (abilities.Count > 1))
            {
                //сделать супер удар
                if ((abilities[0] != null) && (Vector3.Distance(Target.transform.position, transform.position) <= attackRange))
                {
                    AbilitySettings ability = abilities[0];

                    if (ability.ready)
                    {
                        ability.damage = damage;
                        ability.maxRange = 10f;
                        CastAbility(ability);
                    }
                }


                //послать метеоритный дождь
                if ((abilities[1] != null) && (mainEnemy.bodyState == BodyState.Alive))
                {
                    AbilitySettings ability = abilities[1];

                    if (ability.ready)
                    {
                        ability.damage = damage;
                        ability.maxRange = 5f;
                        ability.point = mainEnemy.transform.position;
                        CastAbility(ability);
                    }
                }
            }
        }
    }
}
