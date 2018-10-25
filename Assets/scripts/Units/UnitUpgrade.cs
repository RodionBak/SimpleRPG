using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//класс позволяет модифицировать характеристики юнита, 
//получать воздействие 
[System.Serializable]
public class UnitUpgrade //: MonoBehaviour
{

    //характеристики
    [System.Serializable]
    public class UnitLevel
    {
        public string name;
        public float maxHealth;
        public float armor;
        public float damage;
        public float speed;
        public float attackSpeed;
        public float attackRange;
        public float rotationSpeed;

        //прокачка
        public float goldToUpgrade;
        public float xpToUpgrade;

        //заполнение
        public void SetStats(GameUnit _unit)
        {
            if (_unit == null) return;

            maxHealth = _unit.maxHealth;
            armor = _unit.armor;
            damage = _unit.damage;
            speed = _unit.speed;
            attackSpeed = _unit.attackSpeed;
            attackRange = _unit.attackRange;
            rotationSpeed = _unit.rotationSpeed;
        }

        //получение
        public void GetStats(GameUnit _unit)
        {
            if (_unit == null) return;

            _unit.maxHealth = maxHealth;
            _unit.armor = armor;
            _unit.damage = damage;
            _unit.speed = speed;
            _unit.attackSpeed = attackSpeed;
            _unit.attackRange = attackRange;
            _unit.rotationSpeed = rotationSpeed;
        }
    }

    //модификаторы
    [System.Serializable]
    public class Modificator
    {
        public float addSpeed = 0f;
        public float addDamage = 0f;
        public float incSpeed = 0f;
        public float incDamage = 0f;
        public float incArmor = 0f;
        public float incAttackSpeed = 0f;
        public float incRotationSpeed = 1f;
        public float time = 1f;

        //применить
        public void Apply(GameUnit _unit)
        {
            if (_unit == null) return;

            _unit.speed = Mathf.Clamp(_unit.speed + addSpeed, 0f, 9999999f);
            _unit.damage = Mathf.Clamp(_unit.damage + addDamage, 0f, 9999999f);
            _unit.speed *= incSpeed;
            _unit.damage *= incDamage;
            _unit.armor *= incArmor;
            _unit.attackSpeed *= incAttackSpeed;
            _unit.rotationSpeed *= incRotationSpeed;
        }
    }


    public class Modificators : List<Modificator>
    {
        public GameUnit baseUnit;
        public UnitLevel baseStats;

        //создание модификатора
        public Modificators(GameUnit _unit)
        {
            baseUnit = _unit;
            baseStats = new UnitLevel();
            baseStats.SetStats(_unit);
        }

        //применить все модификаторы
        public void Apply()
        {
            baseStats.GetStats(baseUnit);

            foreach(Modificator _mod in this)
            {
                _mod.Apply(baseUnit);
            }
            
        }

        //отсчет времени модификаторов
        public void Cooldown(float _time)
        {
            for (int i=0;i<this.Count;i++)
            {
                if (i >= this.Count) continue;

                if (this[i].time > 0f)
                {
                    this[i].time -= _time;
                }
                else
                {
                    this.Remove(this[i]);
                }
            }
        }
    }







    //юнит
    [HideInInspector] public GameUnit unit;

    //уровни
    public List<UnitLevel> levels = new List<UnitLevel>();

    //модификаторы
    public Modificators modificators;

    
    //конструктор
    public UnitUpgrade(GameUnit _unit)
    {
        modificators = new Modificators(_unit);
        unit = _unit;
    }


    //установить уровень
    public void SetLevel(int _level)
    {
        if (_level < 0) return;
        if (_level > levels.Count) return;
        if (unit == null) return;

        levels[_level].SetStats(unit);
    }

}
