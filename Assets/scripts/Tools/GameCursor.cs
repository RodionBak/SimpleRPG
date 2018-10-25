using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//игровой курсор
public class GameCursor : MonoBehaviour {

    //виды курсора
    public enum CursorState
    {
        Default = 0,
        SetTarget = 1,
        Attack = 2,
        Zone = 3,
        TargetAttack
    }

    public List<GameObject> states = new List<GameObject>();

    //состояние курсора
    public CursorState _cursorState = CursorState.Default;
    public CursorState cursorState
    {
        get { return _cursorState; }
        set
        {
            _cursorState = value;
            SetCursorState(_cursorState);
        }
    }

    public FigureDrawer zoneFigure;
    public float zoneRadius;

    //установить режим курсора
    public void SetCursorState(CursorState _cursorState)
    {
        int index = (int)_cursorState;

        if (index >= states.Count) return;
        if (states[index] == null) return;

        foreach (GameObject stateObj in states)
        {
            stateObj.SetActive(stateObj == states[index]);
        }

        //zone
        if (_cursorState == CursorState.Zone)
        {
            if (zoneFigure != null)
            {
                zoneFigure.gameObject.SetActive(true);
                zoneFigure.DrawCircle(zoneRadius, 32);
            }
        }
    }


    //получить мировую координату
    public static Vector3 GetWorldPos(Vector3 _screenPos)
    {
        Vector3 result = new Vector3(0f, 0f, 0f);

        Ray ray = Camera.main.ScreenPointToRay(_screenPos);
        float distantion = ray.origin.y / ray.direction.y;

        result = new Vector3(
            ray.origin.x - (ray.direction.x * distantion),
            0f,
            ray.origin.z - (ray.direction.z * distantion));

        return result;
    }
}
