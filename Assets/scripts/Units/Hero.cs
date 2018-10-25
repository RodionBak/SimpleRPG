using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//перерождаемый герой со способностями
public class Hero : Minion {

    [Header ("Hero")]
    public Transform respawnPoint;
    public float respawnTime = 10f;

    // Use this for initialization
    void Start () {
        InitHero();
	}

    //создать героя
    public void InitHero()
    {
        InitMinion();
    }
	
	// Update is called once per frame
	void Update () {
        MinionAIProcess();
    }

    //воскрешение
    public override void Respawn()
    {
        base.Respawn();

        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }

        //переместить камеру
        if (country == GameController.Instance.country)
        {
            CameraMovement.instance.followSelected = false;
            CameraMovement.instance.cameraTarget = transform.position;
        }
    }

    //смерть
    bool timerAdded = false;
    public override void OnDeath()
    {
        base.OnDeath();

        if (timerAdded == false)
        {
            timerAdded = true;
            GameController.Instance.timers.AddTimer(className + " появится через %timer секунд", respawnTime * (Level + 1), 0, delegate
            {
                gameObject.SetActive(true);
                timerAdded = false;
                bodyState = BodyState.Alive;
            });
            gameObject.SetActive(false);
        }
    }

}
