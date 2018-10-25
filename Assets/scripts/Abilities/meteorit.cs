using UnityEngine;
using System.Collections;

public class meteorit : GameBody {

    GameUnit owner;
    public float range = 4f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "meteorite")
        {
            Ability.BangDamage(transform.position, 100f ,range, owner);
            Destroy(gameObject);
        }
    }
}
