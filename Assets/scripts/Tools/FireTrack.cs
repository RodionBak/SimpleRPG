using UnityEngine;
using System.Collections;


//оставляет за собой след
public class FireTrack : MonoBehaviour {

	Vector3 lastPosition;
	public LineRenderer lineRenderer;
	Vector3[] positions;
    public int maxLength = 14;
    int length = 0;

	//цель
	public Transform targetTransform;
	public int trackCount = 15;

	void Start () {

        if (gameObject.GetComponent<LineRenderer> () != null) {
			lineRenderer = gameObject.GetComponent<LineRenderer> ();
		} else 
		{
			gameObject.AddComponent<LineRenderer> ();
			lineRenderer = gameObject.GetComponent<LineRenderer> ();
		}

        //очистить все точки
        positions = new Vector3[trackCount];
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = gameObject.transform.position;
        }

        
        lineRenderer.SetVertexCount(trackCount);
        lineRenderer.SetPositions(positions);
        lineRenderer.useWorldSpace = false;

        StartCoroutine(AddPosition());
    }


    //добавить позицию
    private IEnumerator AddPosition()
	{
        yield return new WaitForSeconds(0.07f);

        float trackLenght = 0f;
		
		for (int i = (positions.Length - 1); i >=2; i--) 
		{
			//сместить
			positions [i] = positions [i - 1];
		}

        positions[1] = -(transform.position - lastPosition);
        positions[0] = new Vector3(0f, 0f, 0f);

        if ((transform.position - lastPosition).magnitude > 0f)
        {
            if (length < maxLength)
            {
                length++;
            }
        }else
        {
            if (length > 1)
            {
                length--;
            }
        }

        //временный массив
        Vector3[] newPos = new Vector3[length];
        for (int i = 0; i < length;i++)
        {
            newPos[i] = positions[i];
        }

        lineRenderer.SetVertexCount(trackCount);
        lineRenderer.SetPositions(newPos);

        lastPosition = transform.position;

        StartCoroutine(AddPosition());
	}



}
