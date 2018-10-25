using UnityEngine;
using System.Collections;

//Главная база врагов
public class BossBase : MonoBehaviour {

    public MainBase bossBase;
    public Hero Boss;
    public Transform spawnPoint;

	// Use this for initialization
	void Start () {
        if (bossBase != null)
        {
            bossBase.onDamage += delegate (float _damage)
            {
                if (Boss != null) 
                {
                    if (Boss.active == false)
                    {
                        Boss.transform.position = spawnPoint.position;
                        Boss.active = true;
                    }
                }
            };
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
