using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//отображение UI способности 
public class AbilityUI : MonoBehaviour {

    public Image cooldownLine;
    public Text abilityText;

    //для обновления
    GameUnit unit;
    int abilityIndex = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (gameObject.activeSelf)
        {
            if (unit != null)
            {
                ShowAbility(unit, abilityIndex);
            }
        }
    }



    //обновить
    public void ShowAbility(GameUnit _unit, int _abilityIndex)
    {
        if (_unit == null) return;
        if (_unit.abilities.Count <= _abilityIndex) return;
        if (_unit.abilities[_abilityIndex] == null) return;

        GameUnit.AbilitySettings abilitySettings = _unit.abilities[_abilityIndex];
        unit = _unit;
        abilityIndex = _abilityIndex;

        float cooldownWidth = 1f;
        if (abilitySettings.cooldownTime > 0f)
        {
            cooldownWidth = (abilitySettings.readyTime / abilitySettings.cooldownTime);
            cooldownLine.transform.localScale = new Vector3(cooldownWidth, 1f, 1f);
        }

        abilityText.text = abilitySettings.name + " " + abilitySettings.hotKey.ToString();

        //цвет активный или нет
        abilityText.color = (cooldownWidth == 1f) ? Color.green : Color.black;

        //запуск способности
        if (Input.GetKeyDown(abilitySettings.hotKey))
        {
            _unit.CastAbilityCommand(_abilityIndex);
        }
    }
}
