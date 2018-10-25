using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

//карта, объектов по тегу Unit
public class GUIMap : MonoBehaviour {


    public Image markerPrefub;
    public Transform markersParent;
    public Camera mainCamera;
    public float updatePositionTime = 0.2f;
    public float upfateMarkersTime = 3f;
    public GameBody.Country country = GameBody.Country.Friends;

    Vector3 minObject = new Vector3(999f,999f,999f);
    Vector3 maxObject = new Vector3(-999f, -999f, -999f);
    float mapSize = 1f;
    public bool active = true;
    //маркер
    public class MapMarker
    {
        public Transform trackObject;
        public Transform markerObject;
        public Color color;
        public float radius = 2f;
    }

    public List<MapMarker> markers = new List<MapMarker>();

	// Use this for initialization
	void Start () {
        if (active)
        {
            StartCoroutine(IUpdateMarkers());
            StartCoroutine(IUpdatePositions());
        }
    }
	
    public void UpdateMapSize()
    {
        minObject = new Vector3(999f, 999f, 999f);
        maxObject = new Vector3(-999f, -999f, -999f);

        //поиск ближайших врагов
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i].GetComponent<GameUnit>() != null)
            {
                GameUnit _unit = units[i].GetComponent<GameUnit>();
                if (_unit.transform.position.x < minObject.x)
                {
                    minObject = new Vector3(_unit.transform.position.x, 0f, minObject.z);
                }

                if (_unit.transform.position.x > maxObject.x)
                {
                    maxObject = new Vector3(_unit.transform.position.x, 0f, maxObject.z);
                }

                if (_unit.transform.position.z < minObject.z)
                {
                    minObject = new Vector3(minObject.x, 0f, _unit.transform.position.z);
                }

                if (_unit.transform.position.z > maxObject.z)
                {
                    maxObject = new Vector3(minObject.x, 0f, _unit.transform.position.z);
                }

                //добавить маркер нового юнита
                AddMarker(_unit.transform, (country == _unit.country)?Color.green:Color.red, _unit.radius);
            }
        }


        //80x80
        float width = maxObject.x - minObject.x;
        float height = maxObject.z - minObject.z;
        float maxSize = (width > height) ? width : height;
        if (maxSize > 0f)
        {
            mapSize = 80f / maxSize;
        }else
        {
            mapSize = 80f;
        }
    }


    //обновить положение маркеров
    public void UpdatePositions()
    {
        Vector3 center = minObject + ((maxObject - minObject) * 0.5f);
        Vector3 pos = new Vector3(0f, 0f, 0f);
        for (int i=0; i<markers.Count;i++)
        {
            //проверка на удаленные элементы
            if (i >= markers.Count) continue;
            if (markers[i] == null) continue;

            MapMarker _marker = markers[i];

            if (_marker.trackObject != null)
            {
                if (_marker.markerObject != null)
                {
                    _marker.markerObject.gameObject.SetActive(true);
                    pos = _marker.trackObject.position - center;
                    _marker.markerObject.localPosition = new Vector3(pos.x * mapSize,
                                                                     pos.z * mapSize, 0f);
                    float markerSize = _marker.radius * mapSize * 0.5f + 2f;
                    _marker.markerObject.GetComponent<RectTransform>().sizeDelta = new Vector2(markerSize, markerSize);
                }
                else
                {
                    ClearMarker(_marker);
                }
            }
            else
            {
                ClearMarker(_marker);
            }
        }
    }

    //добавить маркер на карту
    public void AddMarker(Transform _trackObject, Color _color, float _radius)
    {
        if (markerPrefub == null) return;
        if (_trackObject == null) return;

        //проверить юнита на наличие
        foreach(MapMarker _marker in markers)
        {
            if (_trackObject == _marker.trackObject)
            {
                return;
            }
        }

        //создать объект GUI маркера на карту
        GameObject markerObj = Instantiate(markerPrefub.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
        markerObj.transform.parent = markersParent;

        //создать объект списка маркеров для отслеживания
        MapMarker newMarker = new MapMarker();
        newMarker.trackObject = _trackObject;//отслеживаемый объект
        newMarker.markerObject = markerObj.transform;//GUI-маркер
        newMarker.color = _color;
        newMarker.radius = _radius;
        newMarker.markerObject.gameObject.GetComponent<Image>().color = _color;
        markers.Add(newMarker);
    }

    //удалить маркер
    public void ClearMarker(MapMarker _marker)
    {
        if (_marker == null) return;

        if (markers.Contains(_marker))
        {
            markers.Remove(_marker);
        }

        if (_marker.markerObject != null)
        {
            Destroy(_marker.markerObject.gameObject);
        }
    }


    private IEnumerator IUpdateMarkers()
    {
        yield return new WaitForSeconds(upfateMarkersTime);
        if (active)
        {
            UpdateMapSize();
            StartCoroutine(IUpdateMarkers());
        }
    }


    private IEnumerator IUpdatePositions()
    {
        yield return new WaitForSeconds(updatePositionTime);
        if (active)
        {
            UpdatePositions();
            StartCoroutine(IUpdatePositions());
        }
    }


    // Update is called once per frame
    void Update () {
	
	}


}
