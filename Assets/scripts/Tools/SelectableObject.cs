using UnityEngine;
using System.Collections;

//выделяемый объект
public class SelectableObject : FigureDrawer {

    public bool selected = false;
    [HideInInspector] public float radius = 1f;
    float height = 0f;
    bool enabled = false;
    bool mouseOver = false;
    GameUnit _unit;


    private void OnEnable()
    {
    }

    // Use this for initialization
    void Start () {
        SetupLineRenderer();
    }
	
	// Update is called once per frame
	void Update () {
	
	}


    public void InitObject()
    {
        if (gameObject.GetComponent<GameBody>() != null)
        {
            radius = gameObject.GetComponent<GameBody>().radius;

            if (gameObject.GetComponent<GameUnit>() != null)
            {
                _unit = gameObject.GetComponent<GameUnit>();
            }
        }else
        {
            GameBody.GetObjectSize(gameObject, ref radius, ref height);
        }

        enabled = true;
    }


    //выделить объект
    public void SelectObject(bool _select)
    {
        ShowSelection(_select);
        GameController.Instance.SelectObject(this, _select);
    }


    //отобразить выделение
    public void ShowSelection(bool _show)
    {

        if (!enabled)
        {
            InitObject();
        }

        selected = _show;

        if (!_show)
        {
            Clear();
            return;
        }

        DrawCircle(new Vector3(0f,transform.position.y * -0.5f,0f),radius * 1.2f, 32);

        //цвет союзника или врага
        if (_unit != null)
        {
            SetColor((_unit.country == GameController.Instance.country) ?
                                  ((_unit.gameObject.GetComponent<Hero>() != null)? 
                                    new Color(1f, 1f, 0f, 0.8f): //главный герой
                                    new Color(0f, 1f, 0f, 0.7f))://союзник
                                  new Color(1f, 0f, 0f, 0.7f));//враг
        }
        
    }


    //выделение прямоуголником
    private void OnTriggerEnter(Collider other)
    {
        if (selected) return;

        if (other.gameObject.name == "SelectRect")
        {
            SelectObject(true);
        }
    }

    //снятие выделения
    private void OnTriggerExit(Collider other)
    {
        if (!selected) return;
        if (!GameController.Instance.selecting) return;

        if (other.gameObject.name == "SelectRect")
        {
            SelectObject(false);
        }
    }



    //уничтожение
    private void OnDestroy()
    {
        GameController.Instance.SelectObject(this, false);
    }

    //наведение курсора
    public void OnMouseOver()
    {
        if (GameController.Instance.SelectMode != GameController.SelectingMode.FindZone)
        {
            GameController.Instance.cursor.cursorState = GameCursor.CursorState.Attack;
        }
    }


    public void OnMouseExit()
    {
        GameController.Instance.SelectMode = GameController.Instance.SelectMode;
    }
}
