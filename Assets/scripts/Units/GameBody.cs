using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//базовое тело юнита
public class GameBody : MonoBehaviour {

    public enum BodyState : byte
    {
        Alive = 0,
        Death = 1
    }

    //сторона
    public enum Country
    {
        Friends = 0,
        Enemies = 1,
        Neytral = 2
    }


    //состояние
    private BodyState _bodyState = BodyState.Alive;
    public BodyState bodyState
    {
        get { return _bodyState; }
        set
        {
            switch (value)
            {
                case BodyState.Alive:
                    if (_bodyState != BodyState.Alive)
                    {
                        Respawn();
                    }
                    break;
                case BodyState.Death:
                    if (_bodyState != BodyState.Death)
                    {
                        OnDeath();
                    }
                    break;
            }

            _bodyState = value;
            ChangeState(_bodyState);
        }
    }



    [Header("Body")]
    public Country country = Country.Friends;//сторона
    public float maxHealth = 100f;
    public float _health = 100f;
    public float Health
    {
        get { return _health; }
        set
        {
            _health = Mathf.Clamp(value, 0f, maxHealth);

            onChangeHealth(_health);
        }
    }

    public float armor = 0.0f;
	public float damage = 10f;
    public float speed = 1f;
    public float radius = 0.5f;
    public float height = 0.5f;

    //последний атаковавший
    [HideInInspector] public GameBody lastDamager;

    //делегаты и события
    public delegate void ChangeValueDelegate(float _value);
    public delegate void GameBodyDelegate();
    public delegate void AfterDeathDelegate();


    public event ChangeValueDelegate onChangeHealth = delegate { };
    public event GameBodyDelegate onDeath = delegate { };
    public event GameBodyDelegate onDestroy = delegate { };
    public event ChangeValueDelegate onDamage = delegate { };
    public AfterDeathDelegate afterDeathDelegate;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.T))
        {
            Health -= 10f;
            Debug.Log("Test health");
        }
	}


    public void InitBody()
    {
        Health = maxHealth;

        //обновить размеры
        UpdateSize();

        //добавить полоску жизней
        GameController.Instance.AddHealthBar(this);
    }

    //найти размеры объекта по коллайдерам
    public static bool GetObjectSize(GameObject _testedObject, ref float _radius, ref float _height)
    {
        if ((_radius > 0) && (_height > 0f)) return false;

        if (_testedObject.GetComponent<CapsuleCollider>() != null)
        {
            _height = _testedObject.GetComponent<CapsuleCollider>().height;
            _radius = _testedObject.GetComponent<CapsuleCollider>().radius;
        }
        else
       if (_testedObject.GetComponent<BoxCollider>() != null)
        {
            _height = _testedObject.GetComponent<BoxCollider>().size.y;
            _radius = (_testedObject.GetComponent<BoxCollider>().size.x > _testedObject.GetComponent<BoxCollider>().size.z) ?
                      _testedObject.GetComponent<BoxCollider>().size.x : _testedObject.GetComponent<BoxCollider>().size.z;
        }
        else
       if (_testedObject.GetComponent<SphereCollider>() != null)
        {
            _radius = _testedObject.GetComponent<SphereCollider>().radius;
            _height = _radius * 2f;
        }

        _radius *= (_testedObject.transform.localScale.x > _testedObject.transform.localScale.z) ?
                   _testedObject.transform.localScale.x : _testedObject.transform.localScale.z;
        _height *= _testedObject.transform.localScale.y;

        return true;
    }

    public void UpdateSize()
    {
          GameBody.GetObjectSize(gameObject, ref radius, ref height);
    }

    //получение урона
	public virtual void SetDamage(float _damage, GameBody _damager)
	{
        Health -= _damage - _damage * armor;

        if (_damager != null)
        {
            lastDamager = _damager;
        }


		if ((Health <= 0f) && (bodyState == BodyState.Alive))
		{
            Health = 0f;
            bodyState = BodyState.Death;
		}

        onDamage(_damage);
	}

    //смерть
	public virtual void OnDeath()
	{
        onDeath();
        StartCoroutine(DestroyDeath());
	}
    
    //воскрешение
	public virtual void Respawn()
	{
        Health = maxHealth;
	}

    //изменение состояния
	public virtual void ChangeState(BodyState _bodyState)
	{
	}

    private void OnDestroy()
    {
        onDestroy();
    }


    private IEnumerator DestroyDeath()
    {
        yield return new WaitForSeconds(1f);
        if (afterDeathDelegate == null)
        {
            Destroy(gameObject);
        }else
        {
            afterDeathDelegate.Invoke();
        }
    }
}
