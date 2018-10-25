using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//базовый класс ауры
public class Aura : GameBody {

    public List<GameBody> bodies = new List<GameBody>();

	// Use this for initialization
	void Start () {
        InitBody();
        StartCoroutine(CheckBodies());
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    //вхождение в ауру
     void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GameBody>() != null)
        {
            EnterAura(other.GetComponent<GameBody>());
        }
    }

    public virtual void EnterAura(GameBody _body)
    {
        if (_body == null) return;

        bodies.Add(_body);
    }

    //выход из ауры
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<GameBody>() != null)
        {
            ExitAura(other.GetComponent<GameBody>());
        }
    }

    public virtual void ExitAura(GameBody _body)
    {
        if (_body == null) return;

        if (bodies.Contains(_body))
        {
            bodies.Remove(_body);
        }
    }


    //удалять тела
    private IEnumerator CheckBodies()
    {
        yield return new WaitForSeconds(1f);

        foreach (GameBody _body in bodies)
        {
            if (_body == null)
            {
                bodies.Remove(_body);
            }
            else
            {
                if (_body.height <= 0f)
                {
                    bodies.Remove(_body);
                }
            }
        }
    }
}
