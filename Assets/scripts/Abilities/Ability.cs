using UnityEngine;
using System.Collections;

//базовый класс способности (стрела, магия, взрыв)
public class Ability : MonoBehaviour
{

    public enum AbilityType
    {
        Target, Zone,
    }

    [Header ("Ability")]
    //тип
    public AbilityType abilityType = AbilityType.Target;

    //область
    public float maxRange = 1f;

    //цель
    public Transform target;

    //урон
    public float damage;

    //кастующий юнит
    public GameUnit ownerUnit;

    //установить область
    public void Setup(float _range)
    {
        abilityType = AbilityType.Zone;
        maxRange = _range;
        StartAbility();
    }

    //установить цель
    public void Setup(Transform _target)
    {
        abilityType = AbilityType.Target;
        target = _target;

        StartAbility();
    }

    //запуск
    public virtual void StartAbility()
    {
        
    }


    //нанести удар вокруг
    public static void BangDamage(Vector3 _bangPosition,float _damage, float _range,GameUnit _damagerUnit)
    {
        GameBody.Country damageCountry = GameBody.Country.Neytral;

        if (_damagerUnit != null)
        {
            damageCountry = _damagerUnit.country;
        }
        if (_range <= 0f) return;

        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i].GetComponent<GameUnit>() != null)
            {
                GameUnit _unit = units[i].GetComponent<GameUnit>();
                float _distance = Vector3.Distance(_bangPosition, _unit.transform.position);

                //юнит попадает под урон
                if ((_unit.country != damageCountry) && (_distance <= _range))
                {
                    float power = 1f - (_distance / _range);
                    _unit.SetDamage(power * _damage, _damagerUnit);
                }
            }
        }
    }
}
