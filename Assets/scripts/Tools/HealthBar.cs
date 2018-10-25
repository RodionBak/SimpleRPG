using UnityEngine;
using System.Collections;

//полоса жизни юнита
public class HealthBar : MonoBehaviour {

    Transform redLine;
    Transform greenLine;
    public GameBody body;
    public Vector3 spacing = new Vector3(0f, 1f, 0f);

	// Use this for initialization
	void Start () {
        InitHealthBar();
	}

    public void InitHealthBar()
    {
        if (transform.childCount > 1)
        {
            redLine = transform.GetChild(0);
            greenLine = transform.GetChild(1);
        }

        if (body != null)
        {
            body.onChangeHealth += delegate (float _health)
            {
                UpdateHealth(_health);
            };
            spacing = new Vector3(0f, body.height * 0.8f, 0f);

            Reposition();

            transform.parent = body.transform;
        }


    }
	
	// Update is called once per frame
	void Update () {
        Reposition();
	}


    public void Reposition()
    {
        if (body == null) return;
        transform.position = body.transform.position + spacing;

        if (Camera.main == null) return;
        transform.LookAt(Camera.main.transform);
    }


    //обновить показания жизней
    public void UpdateHealth(float _health)
    {
        if (greenLine == null) return;
        if (body == null) return;
        if (body.maxHealth == 0f) return;

        float _normHealth = Mathf.Clamp(_health / body.maxHealth, 0f, 1f);

        greenLine.localPosition = new Vector3((1f -_normHealth) * 0.5f,0f,0f);
        greenLine.localScale =    new Vector3(_normHealth, 0.3f, 1f);
    }


    //убрать
    public void HideBar()
    {
        Destroy(gameObject);
    }
}
