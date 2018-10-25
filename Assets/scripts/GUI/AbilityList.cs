using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//UI список способностей 
public class AbilityList : MonoBehaviour {

    public UIGrid uiGrid;

    public List<AbilityUI> abilitySlots = new List<AbilityUI>();

    public int nextSlot = 0;

	// Use this for initialization
	void Start () {

        //события для обновления списка
        GameController.Instance.onSelectionChange += UpdateSlots;

        ClearSlots();
	}
	
	// Update is called once per frame
	void Update () {
    }


    //очистить слоты
    public void ClearSlots()
    {
        foreach(AbilityUI abilityUI in abilitySlots)
        {
            abilityUI.gameObject.SetActive(false);
        }
        nextSlot = 0;
    }
    

    //обновить список
    public void UpdateSlots()
    {
        //отключить слоты
        ClearSlots();

        //активировать слоты способностей
        foreach (SelectableObject selObj in GameController.Instance.selectList)
        {
            //юниты
            if (selObj.gameObject.GetComponent<GameUnit>() != null)
            {
                GameUnit _unit = selObj.gameObject.GetComponent<GameUnit>();
                //все способности юнита
                if (_unit.country == GameController.Instance.country)
                {
                    for (int i = 0; i < _unit.abilities.Count; i++)
                    {
                        if (_unit.abilities[i] != null)
                        {
                            if (_unit.abilities[i].hotKey != KeyCode.None)
                            {
                                AddAbility(_unit, i);
                            }
                        }
                    }
                }
            }
        }
    }

    
    //добавить способность
    public void AddAbility(GameUnit _unit, int _abilityIndex)
    {
        if (nextSlot >= abilitySlots.Count) return;
        if (abilitySlots[nextSlot] == null) return;

        if (_unit == null) return;
        if (_unit.abilities.Count <= _abilityIndex) return;
        if (_unit.abilities[_abilityIndex] == null) return;

        abilitySlots[nextSlot].gameObject.SetActive(true);
        abilitySlots[nextSlot].ShowAbility(_unit,_abilityIndex);

        nextSlot++;
    }
    
}
