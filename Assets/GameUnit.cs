using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameUnit : GameBody
{

    [Header("Unit")]
    //состояние и цели
    public string className = "unit";//название класса   
    public GameUnit commander;//командир (если есть)
    public Animator animator;
    public GameBody _target;
    public virtual GameBody Target
    {
        get { return _target; }
        set { _target = value; }
    }

    public List<GameBody> defaultTargets = new List<GameBody>();//цели по умолчанию

    //характеристики:
    [Header("Characteristics")]
    public float attackRange = 2f;
    public float attackSpeed = 1f;
    public float rotationSpeed = 2f;

    //активность
    public bool active = true;

    //Уровень
    public int _level = 0;
    public int Level
    {
        get { return _level; }
        set
        {
            if (value > _level)
            {
                OnLevelUp(value);
            }

            //применить новые параметры
            if ((value < levels.Count) && (value >= 0))
            {
                levels[value].GetStats(this);
            }


            _level = value;
            onUpdateUnitStats();
        }
    }


    //золото
    public float _gold = 10f;
    public float Gold
    {
        get { return _gold; }
        set
        {
            if (value > _gold)
            {
                OnGoldAdd(value - _gold);
            }
            _gold = value;
            onUpdateUnitStats();
        }
    }

    //опыт
    public float _xp = 0f;
    public float XP
    {
        get { return _xp; }
        set
        {
            if (value > _xp)
            {
                OnXpAdd(value - _xp);
            }
            _xp = value;
            onUpdateUnitStats();
        }
    }


    //Уровни
    public List<UnitUpgrade.UnitLevel> levels = new List<UnitUpgrade.UnitLevel>();

    //модификаторы
    public UnitUpgrade.Modificators modificators;




    //настройки способности
    [System.Serializable]
    public class AbilitySettings
    {
        public string name;
        public Ability.AbilityType abilityType;
        public GameObject abilityObj;
        public float maxRange;
        public float damage;
        public KeyCode hotKey;
        [HideInInspector] public Vector3 point;
        [HideInInspector] public Transform target;

        public float cooldownTime = 1f;
        [HideInInspector] public float readyTime = 0f;
        [HideInInspector] public bool ready;
    }


    //способности   
    public List<AbilitySettings> abilities = new List<AbilitySettings>();

    public delegate void UpdateStatesDelegate();

    //События
    public event ChangeValueDelegate onGoldAdd = delegate { };
    public event ChangeValueDelegate onXpAdd = delegate { };
    public event ChangeValueDelegate onLevelUp = delegate { };
    public event UpdateStatesDelegate onUpdateUnitStats = delegate { };



    // Use this for initialization
    void Start()
    {
        InitUnit();
    }



    public void InitUnit()
    {
        InitBody();

        if (gameObject.GetComponent<Animator>())
        {
            animator = gameObject.GetComponent<Animator>();
        }
        FindTarget();

        //обновить характеристики
        Level = Level;
        OnLevelUp(Level);

        //запустить работу модификаторов
        modificators = new UnitUpgrade.Modificators(this);
    }



    // Update is called once per frame
    void Update()
    {
        //работа модификаторов
        //ModificatorsCooldown();
    }


    //модификаторы
    public void ModificatorsCooldown()
    {
        //работа модификаторов
        if (modificators != null)
        {
            if (modificators.Count > 0)
            {
                modificators.Cooldown(Time.deltaTime);
                modificators.Apply();
            }
        }
    }


    //найти врага по близости
    public bool FindTarget()
    {
        float minDistance = 10f;
        float currentDistance = 0f;

        //цели юнита по умолчанию
        if (defaultTargets.Count > 0)
        {
            minDistance = 200f;
            foreach (GameBody _body in defaultTargets)
            {
                if (_body != null)
                {
                    currentDistance = Vector3.Distance(transform.position, _body.transform.position);
                    float addDistance = 0f;

                    //у союзников низкий приоритет
                    if (_body.country == country)
                    { 
                        addDistance = 50f;                       
                    }

                    if ((currentDistance + addDistance) < minDistance)
                    {
                        minDistance = currentDistance + addDistance;
                        Target = _body;
                    }
                }
            }
        }

        //поиск ближайших врагов
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i].GetComponent<GameUnit>() != null)
            {
                GameUnit _unit = units[i].GetComponent<GameUnit>();
                //враг
                if (_unit.country != country)
                {
                    currentDistance = Vector3.Distance(transform.position, _unit.transform.position);
                    if (currentDistance < minDistance)
                    {
                        minDistance = currentDistance;
                        Target = _unit;
                    }
                }
            }
        }

        return Target != null;
    }




    //ударить врага
    public virtual void HitTarget()
    {
        if (CanHit())
        {
            PlayAnimation("Hit");
            OnHitTarget();
        }
    }

    //нанесение урона врагу
    public virtual void OnHitTarget()
    {
        Target.SetDamage(damage, this);
    }

    



    //перерождение
    public override void Respawn()
    {
        base.Respawn();
        onUpdateUnitStats();
    }





    //получение урона и анимация
    public override void SetDamage(float _damage, GameBody _damager)
    {
        base.SetDamage(_damage,_damager);

        if (bodyState == BodyState.Alive)
        {
            PlayAnimation("Damage");
        }
        onUpdateUnitStats();
    }



    //смерть
    public override void OnDeath()
    {
        base.OnDeath();
        
        //последний атаковавший - убийца, увеличить ему опыт и золото
        if (lastDamager != null)
        {
            if (lastDamager.gameObject.GetComponent<GameUnit>() != null)
            {
                lastDamager.gameObject.GetComponent<GameUnit>().GiveStats(this);
            }
        }
    }

    //отдать свои опыт и деньги
    public virtual void GiveStats(GameUnit _unit)
    {
        if (_unit == null) return;

        if (commander != null)//если есть командир - отдать командиру
        {
            commander.XP += _unit.XP;
            commander.Gold += _unit.Gold;
        }
        else
        {
            XP += _unit.XP;
            Gold += _unit.Gold;
        }

        onUpdateUnitStats();
    }


    //изменение состояние "тела"
    public override void ChangeState(BodyState _bodyState)
    {
        PlayAnimation(_bodyState.ToString());
    }

    //перемещаться к цели
    public void MoveAt(Vector3 _moveTarget, float _speed)
    {
        Vector3 _direction = _moveTarget - transform.position;

        if (gameObject.GetComponent<Rigidbody>() != null)
        {
            gameObject.GetComponent<Rigidbody>().velocity = _direction.normalized * speed * _speed;
        }
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
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_direction),Time.deltaTime * rotationSpeed);
    }

    //движение смотря вперед
    public void ForwardMovement(float _axisX, float _axisY)
    {
        if (gameObject.GetComponent<Rigidbody>() != null)
        {
            gameObject.GetComponent<Rigidbody>().velocity = (transform.forward * _axisY + transform.right * _axisX) * speed;
        }
    }

    //воспроизведение анимаций
    public void PlayAnimation(string _animationName)
    {
        if (animator == null) return;

        switch (_animationName)
        {
            case "Alive":
                animator.SetBool("Alive", true);
                break;
            case "Death":
                animator.SetBool("Alive", false);
                break;
            case "Hit":
                animator.SetBool("Hit", true);
                StartCoroutine( OutAnimation(0.05f) );
                break;
            case "Damage":
                animator.SetBool("Damage", true);
                StartCoroutine( OutAnimation(0.05f) );
                break;
        }
    }


    private IEnumerator OutAnimation(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("Damage", false);
        animator.SetBool("Hit", false);
    }



    //возможность ударить
    private bool _canHit = true;
    public bool CanHit()
    {
        //врага
        if (Target == null) return false;

        //растояния
        if ((Vector3.Distance(transform.position, Target.transform.position) - Target.radius) > attackRange) return false;

        //от скорости атаки
        if (_canHit)
        {
            _canHit = false;
            StartCoroutine(CanHitAgainCorutine());
            return true;
        }

        return false;
    }

    private IEnumerator CanHitAgainCorutine()
    {
        float _attackSpeed = Mathf.Clamp(attackSpeed, 0.05f, 900f);
        yield return new WaitForSeconds(1f / _attackSpeed);
        _canHit = true;
    }

    //каст способности
    public void CastAbilityCommand(int index)
    {
        if (index >= abilities.Count) return;
        AbilitySettings _ability = abilities[index];
        if (!_ability.ready) return;

        //способность с указанием зоны
        if (_ability.abilityType == Ability.AbilityType.Zone)
        {
            GameController.Instance.ZoneAction(delegate (Vector3 _point)
            {
                _ability.point = _point + new Vector3(0f, 5f, 0f);
                CastAbility(_ability);
            });
        }

        //способность с указанием цели
        if (_ability.abilityType == Ability.AbilityType.Target)
        {
            GameController.Instance.TargetAction(delegate (GameBody _target)
            {
                _ability.point = transform.position;
                _ability.target = _target.transform;
                CastAbility(_ability);
            });
        }
    }

    //каст способности
    public virtual void CastAbility(AbilitySettings ability)
    {
        if (ability == null) return;
        if (ability.abilityObj == null) return;
        if (!ability.ready) return;

        //создать объект способности
        GameObject abilityClone = Instantiate(ability.abilityObj, ability.point, Quaternion.identity) as GameObject;
        if (abilityClone.GetComponent<Ability>() == null) return;
        Ability abilityObj = abilityClone.GetComponent<Ability>();
        abilityObj.damage = ability.damage;
        abilityObj.maxRange = ability.maxRange;
        abilityObj.ownerUnit = this;

        //указать тип прицела, запустить способоность
        if (ability.abilityType == Ability.AbilityType.Target)
        {
            abilityObj.Setup(ability.target);
        }
        else
        if (ability.abilityType == Ability.AbilityType.Zone)
        {
            abilityObj.Setup(ability.maxRange);
        }

        //ждать перезарядки
        ability.ready = false;
        ability.readyTime = 0f;

        //воспроизвести анимацию юнита
        PlayAnimation(ability.name);
    }


    //перезарядки способностей
    public void AbilityReloading()
    {
        foreach (AbilitySettings ability in abilities)
        {
            if (!ability.ready)
            {
                if (ability.readyTime < ability.cooldownTime)
                {
                    ability.readyTime += Time.deltaTime;
                }
                else
                {
                    ability.ready = true;
                    ability.readyTime = ability.cooldownTime;
                }
            }
        }
    }



    //получение золота
    public void OnGoldAdd(float _gold)
    {
        onGoldAdd(_gold);
    }

    //получение опыта
    public void OnXpAdd(float _xp)
    {
        onXpAdd(_xp);

        //условие получение нового уровня
        int setLevel = -1;
        for (int i=0; i<levels.Count; i++)
        {
            if (XP >= levels[i].xpToUpgrade)
            {
                setLevel = i;
            }
        }


        //достигнут новый уровень
        if (setLevel > Level)
        {
            Level = setLevel;
        }
    }


    //получение уровня
    public virtual void OnLevelUp(int _newLevel)
    {
        onLevelUp(_newLevel);

        //эффект получения уровня
        if (Level > 0)
        {
            GameController.Instance.StartLevelUpEffect(this);
        }
    }


    public virtual void SetActiveUnit(bool _active)
    {
        active = _active;
    }


    //можно ли улучшить уровень за золото
    public float GoldToUpgrade()
    {
        //есть ли цена
        float coast = 0f;

        if ((Level < levels.Count) && (Level >= 0) && (levels.Count > 0))
        {
            coast = levels[Level].goldToUpgrade;
        }

        return coast;
    }

    //покупка уровня за золото
    public virtual void BuyLevelForGold()
    {
        float goldToUpgrade = GoldToUpgrade();

        if (goldToUpgrade > 0f)
        { 
            //покупатель, юнит или его командир
            GameUnit byerUnit = (commander != null) ? commander : this;

            //хватает золота
            if (byerUnit.Gold >= goldToUpgrade)
            {
                Level++;
                byerUnit.Gold -= goldToUpgrade;
                GameController.Instance.timers.AddTimer(className + " улучшено до " + Level.ToString() + " уровня", 5f, 0, null);
            }
            else
            {
                GameController.Instance.timers.AddTimer("Недостаточно золота", 3f, 0, null);
            }
        }
    }
}
