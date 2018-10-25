using UnityEngine;
using System.Collections;

//эффект получения уровня: луч спускается на голову юнита
public class LevelEffect : MonoBehaviour {

    public Transform target;
    public float timer = 1f;
	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {

        if (target != null)
        {
            //transform.LookAt(target);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), Time.deltaTime * 5f);

            if (Vector3.Distance(transform.position,target.position) < 0.5f)
            {
                Destroy(gameObject);
            }
        }

        timer += Time.deltaTime;
        transform.position += transform.forward * Time.deltaTime * 10f;

        if (timer < 0f)
        {
            Destroy(gameObject);
        }
    }
}
