using UnityEngine;
using System.Collections;


//решающая база
public class MainBase : GameUnit {


	// Use this for initialization
	void Start () {
        InitUnit();
	}
	
	// Update is called once per frame
	void Update () {
        //работа модификаторов
        ModificatorsCooldown();
    }

    //база уничтожена
    public override void OnDeath()
    {
        base.OnDeath();

        GameController.Instance.LoseMainBase(country);
    }
}
