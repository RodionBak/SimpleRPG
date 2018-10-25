using UnityEngine;
using System.Collections;


//объект с отрисованной фигурой
public class FigureDrawer : MonoBehaviour {

    LineRenderer lineRenderer;

    private void OnEnable()
    {
        SetupLineRenderer();
    }


    //насроить
    public void SetupLineRenderer()
    {
        if (gameObject.GetComponent<LineRenderer>() != null)
        {
            lineRenderer = gameObject.GetComponent<LineRenderer>();
        }
        else
        {
            gameObject.AddComponent<LineRenderer>();
            lineRenderer = gameObject.GetComponent<LineRenderer>();
        }


        //материал выделения
        if (GameController.Instance != null)
        {
            if (GameController.Instance.selectMaterial != null)
            {
                lineRenderer.material = GameController.Instance.selectMaterial;
            }
        }

       // lineRenderer.
        lineRenderer.useWorldSpace = false;
        lineRenderer.SetWidth(0.2f, 0.2f);

        Clear();
    }



    //цвет
    public void SetColor(Color _color)
    {
        if (lineRenderer == null) return;

        lineRenderer.SetColors(_color, _color);
    }


    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //отобразить окружность
    public void DrawCircle(float _radius, int _segments)
    {
        if (lineRenderer == null) return;

        Vector3[] pos = new Vector3[_segments + 1];
        float _angle = 0f;
        float _step = (360f /_segments) * Mathf.Deg2Rad;
        for (int i=0;i<(_segments + 1);i++)
        {
            _angle = _step * i;
            pos[i] = new Vector3(
                Mathf.Cos(_angle) * _radius, 0.2f, 
                Mathf.Sin(_angle) * _radius);
        }

        lineRenderer.SetVertexCount(_segments + 1);
        lineRenderer.SetPositions(pos);
        //lineRenderer.SetPosition()
    }


    public void DrawCircle(Vector3 spacing, float _radius, int _segments)
    {
        if (lineRenderer == null) return;

        Vector3[] pos = new Vector3[_segments + 1];
        float _angle = 0f;
        float _step = (360f / _segments) * Mathf.Deg2Rad;
        for (int i = 0; i < (_segments + 1); i++)
        {
            _angle = _step * i;
            pos[i] = new Vector3(
                Mathf.Cos(_angle) * _radius, 0.2f,
                Mathf.Sin(_angle) * _radius) + spacing;
        }

        lineRenderer.SetVertexCount(_segments + 1);
        lineRenderer.SetPositions(pos);
        //lineRenderer.SetPosition()
    }


    //отобразить квадрат
    public void DrawRect(Rect _rect)
    {
        if (lineRenderer == null) return;

        Vector3[] pos = new Vector3[5];
        pos[0] = new Vector3(5, 0.2f, 5);
        pos[1] = new Vector3(-5, 0.2f, 5);
        pos[2] = new Vector3(-5, 0.2f, -5);
        pos[3] = new Vector3(5, 0.2f, -5);
        pos[4] = new Vector3(5, 0.2f, 5);

        lineRenderer.SetVertexCount(5);
        lineRenderer.SetPositions(pos);
    }

    public void Clear()
    {
        lineRenderer.SetVertexCount(0);
    }
}
