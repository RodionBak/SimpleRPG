using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    //Singletone
    private static GameController _instance;
    public static GameController Instance
    {
        get
        {
            return _instance;
        }
    }

    public enum CameraMode
    {
        Free, Follow
    }

    //режим выделения
    public enum SelectingMode
    {
        Default = 0,//свободный
        FindTarget = 1,//указание цели
        FindZone = 2,//указание области с радиусом
        SetTargetUnit = 3//указание цели для движения
    }

    [Header("UI Managers")]
    public GameTimers timers;

    [Header ("State")]
    public CameraMode cameraMode = CameraMode.Free;
    public GameBody.Country country = GameBody.Country.Friends;

    //режим выделения
    public SelectingMode _selectingMode;
    public SelectingMode SelectMode
    {
        get { return _selectingMode; }
        set
        {
            _selectingMode = value;
            if (cursor != null)
            {
                switch (_selectingMode)
                {
                    case SelectingMode.Default: cursor.cursorState = GameCursor.CursorState.Default; break;
                    case SelectingMode.FindTarget: cursor.cursorState = GameCursor.CursorState.TargetAttack; break;
                    case SelectingMode.FindZone: cursor.cursorState = GameCursor.CursorState.Zone; break;
                    case SelectingMode.SetTargetUnit: cursor.cursorState = GameCursor.CursorState.SetTarget; break;
                }
            }
        }
    }

    [Header("Cursor")]
    public Vector3 cursorWorldPos;
    public GameCursor cursor;
    public TargetPlace targetPlace;

    Vector3 startSelecting = new Vector3(0f, 0f, 0f);
    Vector3 endSelecting   = new Vector3(0f, 0f, 0f);

    [Header("Prefubs")]
    public GameObject clickEffect;
    public GameObject levelEffect;
    public FigureDrawer figureDrawer;
    public GameObject healthBar;
    public Transform selectRect;
    public Material selectMaterial;
    public UnitMarker unitMarkerPrefub;

    [Header("Selected objects")]
    public bool selecting = false;
    bool frameSelecting = false;
    public List<SelectableObject> selectList = new List<SelectableObject>();


    //Здания уровня
    public List<GameUnit> builds = new List<GameUnit>();

    //делегаты
    public delegate void SelectDelegate();
    public delegate void OnFrameSelectDelegate(Vector3 _startSelect, Vector3 _endSelect);
    public delegate void OnCommandPositionDelegate(Vector3 _point);
    public delegate void TargetActionDelegate(GameBody _target);
    public delegate void ZoneActionDelegate(Vector3 _point);

    //события
    public event OnFrameSelectDelegate onFrameSelect = delegate { };
    public event OnCommandPositionDelegate onCommandPosition = delegate { };
    public event SelectDelegate onSelectionChange = delegate { };


    //действия
    public TargetActionDelegate targetAction = delegate  { };
    public ZoneActionDelegate zoneAction = delegate { };

    void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        timers.AddTimer("Simple RPG Rodion Bakanov rodionbak@gmail.com", 5f, 0, null);
        

        timers.AddTimer("Старт через %timer секунд", 5f, 0, delegate
        {
            foreach (GameUnit _build in builds)
            {
                if (_build != null)
                {
                    _build.SetActiveUnit(true);
                }
            }

            timers.AddTimer("Навигация: курсор мыши или стрелки", 8f, 0, null);
            timers.AddTimer("Левая кнопка мыши: выделение юнита", 8f, 0, null);
            timers.AddTimer("Правая кнопка мыши: команда", 8f, 0, null);
            timers.AddTimer("Двойной щелчек: наблюдение за целью", 8f, 0, null);
        });



    }




    //Управление
    void Update()
    {
        cursorWorldPos = GameCursor.GetWorldPos(Input.mousePosition);

        //курсор
        if (cursor == null) return;

        //позиция курсора
        cursor.transform.position = cursorWorldPos;


        //нажатия DOWN
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnCursorDown(KeyCode.Mouse0);

            //двойной клик
            if (doubleClick == false)
            {
                doubleClick = true;
                StartCoroutine(DoubleClickTime(0.5f));
            }
        }

        //правой кнопкой
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            OnCursorDown(KeyCode.Mouse1);
        }

        //выделение
        if (selecting)
        {
            OnSelecting();
        }


        //Нажатия UP
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            OnCursorUp(KeyCode.Mouse0);
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            OnCursorUp(KeyCode.Mouse1);
        }
    }





    //проверить наличие объекта под курсором
    public SelectableObject GetObjectInCursor()
    {
        //проверить клик по объекту
        SelectableObject result = null;
        RaycastHit hit;
        Ray MyRay;
        MyRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(MyRay.origin, MyRay.direction * 10, Color.yellow);
        if (Physics.Raycast(MyRay, out hit, 100))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<SelectableObject>() != null)
                {
                    result = hit.collider.gameObject.GetComponent<SelectableObject>();
                }
            }
        }

        return result;
    }

    //Double Click
    public bool doubleClick;
    private IEnumerator DoubleClickTime(float clickTime)
    {
        yield return new WaitForSeconds(clickTime);
        doubleClick = false;
    }


    //Cursor Down
    public void OnCursorDown(KeyCode keyCode)
    {

        //проверить клик по объекту
        SelectableObject clickObject = GetObjectInCursor();
        GameBody _clickBody = null;
        if (clickObject != null)
        {
            //выделено тело
            if (clickObject.gameObject.GetComponent<GameBody>() != null)
            {
                _clickBody = clickObject.gameObject.GetComponent<GameBody>();
            }
        }

        //приказы правой кнопкой мыши
        if ((selectList.Count > 0) && (keyCode == KeyCode.Mouse1))
        {
            //к объекту или цели
            CommandGo((_clickBody != null) ? _clickBody : targetPlace);
            SelectMode = SelectingMode.Default;
            return;
        }

        //режим указа цели
        if ((SelectMode == SelectingMode.FindTarget) && (keyCode == KeyCode.Mouse0) && (_clickBody != null))
        {
            SelectMode = SelectingMode.Default;

            if (targetAction != null)
            {
                targetAction.Invoke(_clickBody);
                return;
            }
        }

        //режим указания зоны
        if (SelectMode == SelectingMode.FindZone)
        {
            SelectMode = SelectingMode.Default;

            //действие по указанию зоны
            if (zoneAction != null)
            {
                zoneAction.Invoke(cursorWorldPos);
                return;
            }
        }


        //обычный клик по объекту
        if ((SelectMode != SelectingMode.FindTarget) && (keyCode == KeyCode.Mouse0) && (clickObject != null))
        {
            DeselectAll();

            //выделить объект
            SelectObject(clickObject, true);

            //двойной клик - следовать за объектом
            if (doubleClick)
            {
                Debug.Log("Double click");
                CameraMovement.instance.followTarget = clickObject.transform;
            }

            return;
        }


        //левой кнопкой по пустому полю
        if (keyCode == KeyCode.Mouse0)
        {
            //обычный режим выделения
            selecting = true;
            startSelecting = cursorWorldPos;
            ShowClick(cursorWorldPos);

            //подготовить рамку для выделения
            if (selectRect != null)
            {
                selectRect.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                selectRect.transform.position = startSelecting + new Vector3(0f, 0.2f, 0f);
                selectRect.gameObject.SetActive(true);
            }
        }

    }


    //Cursor UP
    public void OnCursorUp(KeyCode keyCode)
    {
        selecting = false;
        frameSelecting = false;
        endSelecting = cursorWorldPos;
        ShowClick(cursorWorldPos);

        SelectFrame(startSelecting, endSelecting);
        if (selectRect != null)
        {
            selectRect.gameObject.SetActive(false);
        }
    }


    //выделение
    public void OnSelecting()
    {
        //начать выделение рамкой
        if ((Vector3.Distance(startSelecting, cursorWorldPos) > 0.5f) && (frameSelecting == false))
        {
            frameSelecting = true;
            DeselectAll();
        }

        //выделение рамкой
        if (frameSelecting)
        {
            SelectFrame(startSelecting, cursorWorldPos);
        }
    }


    //добавить выделенный объект
    public void SelectObject(SelectableObject _selectableObject, bool _select)
    {
        if (_selectableObject == null) return;

        if (_select)
        {
            if (selectList.Contains(_selectableObject)) return;
            CameraMovement.instance.followTarget = null;

            //выделить
            _selectableObject.ShowSelection(_select);
            selectList.Add(_selectableObject);
            onSelectionChange();

            //переключить режим указания цели
            if (SelectMode != SelectingMode.SetTargetUnit)
            {
                SelectMode = SelectingMode.SetTargetUnit;
            }
        }
        else
        {
            if (selectList.Contains(_selectableObject))
            {
                selectList.Remove(_selectableObject);
            }
        }
    }



    //навести на объект
    public void OnCursorOver(SelectableObject _selectableObject)
    {
        if (SelectMode == SelectingMode.FindTarget)
        {
            cursor.cursorState = GameCursor.CursorState.Attack;
        }
    }


    //навести на объект
    public void OnCursorExit(SelectableObject _selectableObject)
    {
        if ((SelectMode == SelectingMode.FindTarget) && (cursor.cursorState == GameCursor.CursorState.Attack))
        {
            cursor.cursorState = GameCursor.CursorState.SetTarget;
        }
    }



    //снять выделения
    public void DeselectAll()
    {
        foreach(SelectableObject selected in selectList)
        {
            selected.ShowSelection(false);
        }
        selectList.Clear();

        //вернуть исходный режим выделения
        SelectMode = SelectingMode.Default;

        //событие обновления списка
        onSelectionChange();
    }



    //выделение рамкой
    public void SelectFrame(Vector3 _startSelect, Vector3 _endSelect)
    {
        //обновить рамку выделения
        if (selectRect != null)
        {
            float _width = cursorWorldPos.x - startSelecting.x;
            float _height = cursorWorldPos.z - startSelecting.z;
            selectRect.transform.localScale = new Vector3(_width, _height, 1f);
            selectRect.transform.position = startSelecting + new Vector3(_width * 0.5f, 0.1f, _height * 0.5f);
        }
    }


    //показать эффект клика
    public void ShowClick(Vector3 clickWorldPos)
    {
        if (clickEffect != null)
        {
            Instantiate(clickEffect, clickWorldPos, Quaternion.identity);
        }
    }



    //приказать юнитам точку передвижения
    public void CommandGo(Vector3 _goPoint)
    {
        //курсор
        if (targetPlace == null) return;
        targetPlace.transform.position = cursorWorldPos;

        //задать каждому выделенному эту цель
        foreach (SelectableObject selected in selectList)
        {
            if (selected.gameObject.GetComponent<Minion>())
            {
                Minion minion = selected.gameObject.GetComponent<Minion>();

                if (minion.country == country)
                {
                    minion.Target = targetPlace;
                    minion.minionState = Minion.MinionState.Command;
                }
            }
        }
    }



    //указать юнитам цель
    public void CommandGo(GameBody _targetBody)
    {
        //курсор
        if (targetPlace == null) return;
        targetPlace.transform.position = cursorWorldPos;

        //задать каждому выделенному эту цель
        foreach (SelectableObject selected in selectList)
        {
            if (selected.gameObject.GetComponent<Minion>())
            {
                Minion minion = selected.gameObject.GetComponent<Minion>();

                if (minion.country == country)
                {
                    minion.Target = _targetBody;
                    minion.minionState = Minion.MinionState.Command;
                }
            }
        }
    }



    //добавить строку жизни для игрового тела
    public void AddHealthBar(GameBody _body)
    {
        GameObject newBar = Instantiate(healthBar, transform.position, Quaternion.identity) as GameObject;

        if (newBar.GetComponent<HealthBar>() != null)
        {
            newBar.GetComponent<HealthBar>().body = _body;
        }
    }


    //запросы на указание цели и действие
    public void TargetAction(TargetActionDelegate _action)
    {
        targetAction = _action;
        SelectMode = SelectingMode.FindTarget;
    }


    //запросы на указание зоны и действие
    public void ZoneAction(ZoneActionDelegate _action)
    {
        zoneAction = _action;
        SelectMode = SelectingMode.FindZone;
    }


    //запустить эффект получения уровня
    public void StartLevelUpEffect(GameUnit _unit)
    {
        if (levelEffect == null) return;
        GameObject newEffect = Instantiate(levelEffect, _unit.transform.position + new Vector3(0f,5f,0f), Quaternion.identity) as GameObject;
        newEffect.GetComponent<LevelEffect>().target = _unit.transform;
        newEffect.GetComponent<LevelEffect>().timer = 2f;
    }




    //уничтожена решающая база
    public void LoseMainBase(GameBody.Country _country)
    {
        GameController.friendsWin += (_country == GameBody.Country.Enemies) ? 1 : 0;
        GameController.enemiesWin += (_country == GameBody.Country.Enemies) ? 0 : 1;

        string winCountry = (_country == GameBody.Country.Enemies) ? "Разработчики" : "Враги";
        string winMessage = "Победили " + winCountry +  "! счет: " + friendsWin.ToString() + ":" + enemiesWin.ToString();

        //сообщение о победе
        timers.AddTimer(winMessage, 10f, 0, delegate
        {
        });

        //рестарт раунда
        timers.AddTimer("рестарт раунда через %timer сек", 30f, 0, delegate
        {         
            SceneManager.LoadScene("Game");
        });
        
    }
    //счет (временно)
    public static int friendsWin = 0;
    public static int enemiesWin = 0;

}
