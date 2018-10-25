using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour {

    public Text text;
    public string sourceText;

    private void OnEnable()
    {
        if (gameObject.GetComponent<Text>() != null)
        {
            text = gameObject.GetComponent<Text>();
        }
        else
        {
            int childCount = transform.childCount;
            for (int i=0; i<childCount;i++)
            {
                if (transform.GetChild(i).gameObject.GetComponent<Text>() != null)
                {
                    text = transform.GetChild(i).gameObject.GetComponent<Text>();
                }
            }
        }

        if (text != null)
        {
            sourceText = text.text;
        }
    }
    
    //отобразить время в тексте
    public void ShowTimer(float timer)
    {
        if (text == null) return;

        text.text = sourceText.Replace("%timer", ((int)timer).ToString());
    }


    //задать текст
    public void SetText(string _text)
    {
        if (text != null)
        {
            sourceText = _text;
        }
    }
}
