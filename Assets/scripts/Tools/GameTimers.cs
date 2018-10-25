using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//менеджер игровых таймеров
public class GameTimers : MonoBehaviour {
    public delegate void TimerActionDelegate();



    //GUI префаб таймера
    public TimerUI uiTimerPrefub;
    UIGrid uiGrid;

    //список запущенных таймеров
    [System.Serializable]
    public class ActionTimer
    {
        public string name;
        public float time;
        public float timeToAction;
        public TimerActionDelegate action = delegate { };
        public int repeatCount = 0;
        public TimerUI uiPanel;
    }

    public List<ActionTimer> timers = new List<ActionTimer>();

    private void OnEnable()
    {
        if (gameObject.GetComponent<UIGrid>() != null)
        {
            uiGrid = gameObject.GetComponent<UIGrid>();
        }else
        {
            gameObject.AddComponent<UIGrid>();
            uiGrid = gameObject.GetComponent<UIGrid>();
        }
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	   if (timers.Count > 0)
        {
            for (int i=0; i<timers.Count;i++)
            {
                if (i >= timers.Count) continue;
                if (timers[i] == null) continue;

                ActionTimer _timer = timers[i];
                //отсчет
                if (_timer.timeToAction > 0f)
                {
                    _timer.timeToAction -= Time.deltaTime;

                    //обновить GUI панель
                    if (_timer.uiPanel != null)
                    {
                        _timer.uiPanel.ShowTimer(_timer.timeToAction);
                    }
                //закончить
                }else
                {
                    OnTimerAction(_timer);
                }
            }
        }
	}


    //конец таймера
    public void OnTimerAction(ActionTimer _timer)
    {
        //действие по таймеру
        if (_timer.action != null)
        {
            _timer.action.Invoke();
        }


        //повтор таймера
        if (_timer.repeatCount > 0)
        {
            _timer.repeatCount--;
            ResetTimer(_timer);
        }
        else
        {
            if ((_timer.uiPanel != null) && (uiGrid != null))
            {
                Destroy(_timer.uiPanel.gameObject);
                uiGrid.Reposition();
            }
            timers.Remove(_timer);
        }
    }

    public void ResetTimer(ActionTimer _timer)
    {
        _timer.timeToAction = _timer.time;
    }


    //добавить новый таймер
    public void AddTimer(string text, float _setTime, int _repeatCount, TimerActionDelegate _timerAction)
    {
        ActionTimer newTimer = new ActionTimer();
        newTimer.time = _setTime;
        newTimer.repeatCount = _repeatCount;
        newTimer.action = _timerAction;

        //добавить GUI-элемент
        if ((uiTimerPrefub != null) && (uiGrid != null))
        {
            TimerUI timerUI = Instantiate(uiTimerPrefub, uiTimerPrefub.transform.position, uiTimerPrefub.transform.rotation) as TimerUI;
            timerUI.transform.parent = uiGrid.transform;
            timerUI.gameObject.SetActive(true);
            newTimer.uiPanel = timerUI;
            newTimer.uiPanel.SetText(text);

            uiGrid.Reposition();
        }

        ResetTimer(newTimer);
        timers.Add(newTimer);
    }
}
