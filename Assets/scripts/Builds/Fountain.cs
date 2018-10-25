using UnityEngine;
using System.Collections;

//фонтан
public class Fountain : Aura {

    //скорости восстановления
    public float regenerationSpeed = 10f;
    public float regenerationCommander = 15f;
    public GameUnit commander;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if (bodies.Count > 0)
        {
            foreach (GameBody _body in bodies)
            {
                if (_body == null) continue;
                if (_body.GetComponent<GameUnit>() == null) continue;
                if (_body.bodyState == BodyState.Death) continue;

                GameUnit _unit = _body.GetComponent<GameUnit>();
                //регенерация для командира
                if (_unit == commander)
                {
                    _unit.Health += regenerationCommander * Time.deltaTime;
                }
                //регенерация для обычного юнита
                else
                {
                    _unit.Health += regenerationSpeed * Time.deltaTime;
                }


            }
        }
	}


    //вхождение в ауру только для юнитов-союзников
    public override void EnterAura(GameBody _body)
    {
        if (_body.country == country)
        {
            base.EnterAura(_body);
        }
    }
}
