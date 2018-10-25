using UnityEngine;
using System.Collections;

//способность: метеоритный дождь
public class Shower : Ability {


    [Header("Shower")]
    public int meteoritCount = 10;
    public GameObject meteorite;
    public float height = 10f;
    public float interval = 0.2f;

    //начать дождь
    public override void StartAbility()
    {
        base.StartAbility();

        if (meteorite == null)
        {
            Destroy(gameObject);
            return;
        }
        StartCoroutine(CreateMeteorite());
    }

    public void Update()
    {
    }

    //
    private IEnumerator CreateMeteorite()
    {
        yield return new WaitForSeconds(interval);
        Vector3 point = transform.position + new Vector3(Random.Range(-maxRange, maxRange), 0f, Random.Range(-maxRange, maxRange));
        Instantiate(meteorite, point, Quaternion.identity);
        meteoritCount--;
        if (meteoritCount > 0)
        {
            StartCoroutine(CreateMeteorite());
        }else
        {
            Destroy(gameObject);
        }
    }
}
