using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour {

    public Text nameText;
    public Text levelText;
    public Text damageText;
    public Text armorText;
    public Text speedText;
    public Text rangeText;
    public Text xpText;
    public Text goldText;
    public Text attackSpeedText;
    public GameUnit gameUnit;
    public RectTransform healthBar;
    public Text healthText;
    public Button upgradeButton;
    public Text buttonText;

    public UnitsList unitsList;

    //настройки размеров
    public float fullWidth = 194f;
    public float fullHeight = 155f;
    public float shortHeiht = 55f;

    // Use this for initialization
    void Start () {
        //клик кнопкой
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(BuyLevelClick);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    //кнопка покупки
    public void BuyLevelClick()
    {
        if (gameUnit != null)
        {
            gameUnit.BuyLevelForGold();
        }
    }

    //показывать параметры юнита
    public void ShowUnitUI(GameUnit _unit)
    {
        if (_unit == null) return;

        //обновлять с каждым изменением
        if (gameUnit != _unit)
        {
            if (gameUnit != null)
            {
                gameUnit.onUpdateUnitStats -= UpdateUnitStats;
                gameUnit.onDeath -= UnitDeath;
            }

            gameUnit = _unit;
            gameUnit.onUpdateUnitStats += UpdateUnitStats;
            gameUnit.onDeath += UnitDeath;
        }

        UpdateUnitStats();
    }


    public void UnitDeath()
    {
        gameObject.SetActive(false);
        unitsList.UpdateList();
    }

    //обновить параметры в панели
    public void UpdateUnitStats()
    {    

        //проверить все UI-элементы
        if ((damageText == null) || (armorText == null) || (speedText == null) || (rangeText == null) || (nameText == null) || (levelText == null) ||
             (attackSpeedText == null) || (xpText == null) ||(goldText == null) || (healthBar == null)  || (healthText == null)) return;

        //цвет
        Color color = (GameController.Instance.country == gameUnit.country) ? Color.green : Color.red;

        //обновить значения
        nameText.text = gameUnit.className;
        nameText.color = color;
        levelText.text = gameUnit.Level.ToString();
        levelText.color = color;

        damageText.text = gameUnit.damage.ToString();
        armorText.text = gameUnit.armor.ToString();
        speedText.text = gameUnit.speed.ToString();
        rangeText.text = gameUnit.attackRange.ToString();
        attackSpeedText.text = gameUnit.attackSpeed.ToString();
        xpText.text = gameUnit.XP.ToString();
        goldText.text = gameUnit.Gold.ToString();

        healthBar.localScale = new Vector3(gameUnit.Health / gameUnit.maxHealth, 1f, 1f);
        healthText.text = ((int)gameUnit.Health).ToString() + "/" + gameUnit.maxHealth.ToString();

        //кнопка улучшение юнита
        bool activeButton = false;

        if (gameUnit.country == GameController.Instance.country)
        {
            //цена
            float goldToUpgrade = gameUnit.GoldToUpgrade();
            
            //можно улучшить за золото
            if (goldToUpgrade > 0f)
            {
                //текст цены на кнопке
                buttonText.text = "Улучшить за " + goldToUpgrade.ToString();
                activeButton = true;
            }
        }
        upgradeButton.gameObject.SetActive(activeButton);
    }


    //сделать панель сокращенной
    public void SetShort(bool _short)
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = _short?new Vector2(fullWidth, shortHeiht): new Vector2(fullWidth, fullHeight);
        damageText.transform.parent.gameObject.SetActive(!_short);
        speedText.transform.parent.gameObject.SetActive(!_short);
        armorText.transform.parent.gameObject.SetActive(!_short);
        attackSpeedText.transform.parent.gameObject.SetActive(!_short);
        rangeText.transform.parent.gameObject.SetActive(!_short);
        goldText.transform.parent.gameObject.SetActive(!_short);
        xpText.transform.parent.gameObject.SetActive(!_short);
    }
    



    //улучшить уровень за золото
    public void UpgradeUnit()
    {

    }
}
