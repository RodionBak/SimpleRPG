using UnityEngine;
using System.Collections;

//Летящая стрела
public class Arrow : Ability {
    public float speed = 4f;
    public float rotationSpeed = 5f;
    public float liveTime = 10f;

	// Use this for initialization
	void Start () {
	
	}
	



	// Update is called once per frame
	void Update () {
        Fly();           
    }

    //полет
    public void Fly()
    {
        transform.position += transform.forward * speed * 0.05f;
        if (liveTime > 0f)
        {
            liveTime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }



        //навигация
        if (target == null) return;
        transform.LookAt(target);

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            OnHitTarget();
        }
    }

    //достигла цели
    public virtual void OnHitTarget()
    {
        if (target.GetComponent<GameBody>() != null)
        {
            target.GetComponent<GameBody>().SetDamage(damage, ownerUnit);
        }
        Destroy(gameObject);
    }


    //смотреть на точку
    public void SetLookAt(Vector3 _lookPoint)
    {
        Vector3 _direction = _lookPoint - transform.position;
        _direction = new Vector3(_direction.x, 0f, _direction.z);
        if (_direction.magnitude == 0f)
        {
            _direction = transform.forward;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_direction), Time.deltaTime * rotationSpeed);
    }


    //начало полета
    public override void StartAbility()
    {
        base.StartAbility();
    }
}
