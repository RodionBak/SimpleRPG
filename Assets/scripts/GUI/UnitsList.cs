using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//UI-список выделенных юнитов
public class UnitsList : MonoBehaviour {

    public UIGrid uiGrid;
    public List<UnitUI> unitsSlots = new List<UnitUI>();
    public int nextSlot = 0;
    public int slotsCount = 20;

    private void OnEnable()
    {
        if ((gameObject.GetComponent<UIGrid>() != null) && (uiGrid == null))
        {
            uiGrid = gameObject.GetComponent<UIGrid>();
        }
    }

    // Use this for initialization
    void Start () {
        //создать слоты
        if (unitsSlots.Count == 0) return;
        if (unitsSlots[0] == null) return;

        for (int i = 0; i < slotsCount; i++)
        {
            GameObject newSlot = Instantiate(unitsSlots[0].gameObject, transform.position, Quaternion.identity) as GameObject;
            newSlot.transform.parent = transform;
            newSlot.SetActive(false);
            unitsSlots.Add(newSlot.GetComponent<UnitUI>());
        }

        //обновлять список выделенных
        GameController.Instance.onSelectionChange += UpdateList;

        ClearList();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //добавить панель юнита в список
    public void AddUnitPanel(GameUnit _unit)
    {
        if (nextSlot >= unitsSlots.Count) return;
        if (unitsSlots[nextSlot] == null) return;
        //if (_unit == unitsSlots[nextSlot].gameUnit) return;

        unitsSlots[nextSlot].gameObject.SetActive(true);
        unitsSlots[nextSlot].ShowUnitUI(_unit);
        nextSlot++;

        uiGrid.Reposition();
    }


    //обновить список
    public void UpdateList()
    {
        ClearList();

        //активировать панели юнитов
        foreach (SelectableObject selObj in GameController.Instance.selectList)
        {
            //юниты
            if (selObj.gameObject.GetComponent<GameUnit>() != null)
            {
                GameUnit _unit = selObj.gameObject.GetComponent<GameUnit>();
                AddUnitPanel(_unit);
            }
        }

        //установить размер панелей
        int panelSpace = (int)(Screen.width / 200f) - 1;
        uiGrid.maxInLine = panelSpace;

        //свернуть/развернуть панели
        foreach (UnitUI _unit in unitsSlots)
        {
            _unit.SetShort(nextSlot > panelSpace);
        }
   
        uiGrid.Reposition();
    }

    //очистить список
    public void ClearList()
    {
        foreach (UnitUI _unit in unitsSlots)
        {
            _unit.gameObject.SetActive(false);
        }
        nextSlot = 0;
    }
}
