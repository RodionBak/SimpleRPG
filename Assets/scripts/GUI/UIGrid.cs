using UnityEngine;
using System.Collections;

//список
public class UIGrid : MonoBehaviour {

    public enum GridOrientation
    {
        Vertical, Horizontal, Lines
    }

    public GridOrientation orientation = GridOrientation.Vertical;
    public float height = 10f;
    public float width = 10f;
    public int maxInLine = 4;
    public Vector3 spacing = new Vector3(0f, 0f,0f);

	// Use this for initialization
	void Start () {
        Reposition();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void Reposition()
    {
        int subCount = transform.childCount;
        int index = 0;

        //вертикальный список
        if (orientation == GridOrientation.Vertical)
        {
            for (int i = 0; i < subCount; i++)
            {
                Transform child = transform.GetChild(i);

                if (child.gameObject.activeSelf)
                {
                    child.localPosition = new Vector3(0f, index * -height, 0f);
                    index++;
                }
            }
        }else

        //горизонтальный список
        if (orientation == GridOrientation.Horizontal)
        {
            for (int i = 0; i < subCount; i++)
            {
                Transform child = transform.GetChild(i);

                if (child.gameObject.activeSelf)
                {
                    child.localPosition = spacing + new Vector3(index * width,0f, 0f);
                    index++;
                }
            }
        }else


        //рядами
        if (orientation == GridOrientation.Lines)
        {
            int lineIndex = 0;
            int rowIndex = 0;
            for (int i = 0; i < subCount; i++)
            {
                Transform child = transform.GetChild(i);

                if (child.gameObject.activeSelf)
                {
                    lineIndex = index % maxInLine;
                    rowIndex = index / maxInLine;
                    child.localPosition = spacing + new Vector3(lineIndex * width, rowIndex * -height, 0f);
                    index++;
                }
            }
        }
    }
}
