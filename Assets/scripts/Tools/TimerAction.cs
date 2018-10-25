using UnityEngine;
using System.Collections;
using UnityEngine.Events;

//таймер
public class TimerAction : MonoBehaviour {

    
    public float timeToAction;
    public float time = 30f;
    public bool repeat = false;

    int seconds = 0;
    public bool _active = true;
    public bool Active
    {
        get { return _active; }
        set
        {
            _active = value;

            //запуск таймера
            timeToAction = time;

            //показать текст
            if ((timerText != null) && (hideTimerText))
            {
                timerText.gameObject.SetActive(_active);
            }
        }
    }


    [Header("UI Timer text")]
    public TimerUI timerText;//для отображения
    public bool hideTimerText = true;


    //делегат
    public delegate void ActionDelegate();

    //действие
    public UnityEvent timerAction;
    public ActionDelegate onTimerAction = delegate { };



    // Запуск таймера
    void Start () {
        if (Active)
        {
            Active = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Active)
        {
            if (timeToAction > 0f)
            {
                timeToAction -= Time.deltaTime;
            }else
            {
                OnAction();
            }

            //каждую секунду
            if (seconds != (int)timeToAction)
            {
                OnSecond();
                seconds = (int)timeToAction;
            }
        }
	}

    //секунда таймера
    public virtual void OnSecond()
    {
        if (timerText != null)
        {
            timerText.ShowTimer(timeToAction);
        }
    }


    //таймер истек
    public virtual void OnAction()
    {
        //реакции
        timerAction.Invoke();
        onTimerAction();

        //повторить таймер или вернуть в исх. состояние
        Active = repeat;
    }


    //доп. метод
    public void SetActive(bool _setActive)
    {
        Active = _setActive;
    }
}
