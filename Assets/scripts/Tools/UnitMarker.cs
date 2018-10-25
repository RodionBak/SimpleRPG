using UnityEngine;
using System.Collections;

//маркер - прицеь юнита
public class UnitMarker : GameBody {



	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //установить марке
    public void SetMarker(Transform _target)
    {
        if (_target.gameObject.GetComponent<GameUnit>())
        {
            //показать маркер
            transform.position = _target.position;
            transform.parent = _target;
        }else
        {
            transform.position = _target.position;
        }
    }
}
