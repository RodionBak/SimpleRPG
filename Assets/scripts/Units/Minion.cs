using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Миньон, базовый интеллект боевого юнита
public class Minion : GameUnit {
    

    public delegate void OnMovementComplete();
    public enum MinionState
    {
        Stop = 0,
        Command = 1,
        Move = 2
    }

    //указывать маркер для новой цели
    public override GameBody Target
    {
        get
        {
            return base.Target;
        }

        set
        {
            base.Target = value;
            PutMarker();
        }
    }


    float timeToNextTarget = 0f;
    [Header("Minion")]
    public MinionState _minionState = MinionState.Stop;
    public MinionState minionState
    {
        get { return _minionState; }
        set {
            _minionState = value;
            switch (value)
            {
                case MinionState.Move:
                    break;
                case MinionState.Command:
                    break;
                case MinionState.Stop:
                    ForwardMovement(0f, 0f);//остановиться
                    SetLookAt(transform.position);
                    break;
            }
        }
    }

    //маркер цели
    public UnitMarker marker;

    // Use this for initialization
    void Start () {
        InitMinion();
	}



    //инициализация
    public void InitMinion()
    {
        InitUnit();

        //создать маркер
        CreateMarkerObject();
    }


    //создать объект-маркера
    public void CreateMarkerObject()
    {
        //создать маркер
        if ((GameController.Instance.unitMarkerPrefub != null) && (marker == null) && (GameController.Instance.country == country))
        {
            GameObject markerObj = Instantiate(GameController.Instance.unitMarkerPrefub.gameObject, transform.position, Quaternion.identity) as GameObject;
            markerObj.SetActive(false);
            markerObj.transform.parent = transform;
            marker = markerObj.GetComponent<UnitMarker>();
        }
    }


    //указать марке на цель
    public void PutMarker()
    {
        if (GameController.Instance.country != country) return;
        if (marker == null)
        {
            CreateMarkerObject();
        }
        if (Target == null)
        {
            marker.gameObject.SetActive(false);
            return;
        }

        //враг
        if (Target.country != country)
        {
            marker.transform.position = Target.transform.position;
            marker.transform.parent = Target.transform;
        }


        //курсор
        if (Target.gameObject.GetComponent<TargetPlace>())
        {
            marker.transform.position = Target.transform.position;
            marker.transform.parent = null;
            Target = marker;
        }

        //видимость маркера
        marker.gameObject.SetActive(Target.country != country);
    }



    //передвижение к цели
    public void TargetMovement(Transform _target, OnMovementComplete callback)
    {
        //направление и угол
        Vector3 _direction = _target.position - transform.position;
        _direction = new Vector3(_direction.x, 0f, _direction.z);
        float _angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(_direction));

        //поворот к цели и движение
        SetLookAt(_target.position);
        ForwardMovement(0f, 1f - _angle / 180f);

        //завершение пути
        if (Vector3.Distance(transform.position, _target.position) <= (attackRange * ((minionState == MinionState.Command)?0.7f:1f)  ))
        {
            if (callback != null)
            {
                callback.Invoke();
            }
        }
    }


    //драться
    public virtual void FightingProcess()
    {
        if (Target == null) return;

        //вражеский юнит
        if (Target.country != country)
        {
            //бить
            base.HitTarget();
        }       
    }


    public virtual void MinionAIProcess()
    {
        if (bodyState == BodyState.Death) return;

        //новая цель
        bool newTarget = false;

        //следование к цели
        if (minionState != MinionState.Stop)
        {
            if (Target != null)
            {
                TargetMovement(Target.transform, delegate { minionState = MinionState.Stop; });
            }
            else
            {
                minionState = MinionState.Stop;

                if (minionState != MinionState.Command)
                {
                    newTarget = FindTarget();
                }
            }
        }

        //проверка других целей если нет приказа
        if (timeToNextTarget > 0f)
        {
            timeToNextTarget -= Time.deltaTime;
        }
        else
        {
            timeToNextTarget = Random.Range(1f, 2f);

            if (minionState != MinionState.Command)
            {
                newTarget = FindTarget();
            }
        }

        //переключился на новую цель
        if ((newTarget) && (minionState != MinionState.Command))
        {
            SetLookAt(Target.transform.position);
            minionState = MinionState.Move;
        }


        //драка
        FightingProcess();

        //перезарядки способностей
        AbilityReloading();

        //работа модификаторов
        ModificatorsCooldown();
    }




    // Update is called once per frame
    void Update () {
        MinionAIProcess();
    }


    //временно
    public override void OnDeath()
    {
        base.OnDeath();
    }


    //уничтожение
    public void OnDestroy()
    {
        if (marker != null)
        {
            Destroy(marker.gameObject);
        }
    }


}
