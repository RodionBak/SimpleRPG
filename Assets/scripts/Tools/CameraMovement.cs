using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement instance;

    Vector3 currentCameraTarget = new Vector3(0f, 0f, 0f);
    public Vector3 cameraTarget = new Vector3(0f, 0f, 0f);
    public float cameraAngle = 45f;
    public float cameraDistance = 10f;
    public float cameraSpeed = 1f;
    public Transform followTarget;
    public bool followSelected = false;
    public bool arrowMovement = true;
    public bool mouseOutMovement = false;

    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }


    private void FixedUpdate()
    {
        UpdateMovement();
    }


    //обзор уровня
    public void UpdateMovement()
    {
        //следование цели
        if ((followSelected) && (followTarget != null))
        {
            cameraTarget = followTarget.position;
        }
        //свободное перемещение
        else
        {

            //навигация выходом курсора за рамку
            if (mouseOutMovement)
            {
                float ratio = ((float)Screen.height / (float)Screen.width );
                Vector3 screenCenter = new Vector3(Screen.width * 0.5f, 0f, Screen.height * 0.5f);
                Vector3 mousePosition = new Vector3(Input.mousePosition.x, 0f, Input.mousePosition.y) - screenCenter;
                mousePosition = new Vector3(mousePosition.x * ratio, 0f, mousePosition.z);
                float _distance = mousePosition.magnitude / screenCenter.z;

                //края экрана
                if ((_distance > 0.85f) && (_distance < 1.1f))
                {
                    cameraTarget += mousePosition.normalized * cameraSpeed * 0.15f * _distance;
                }
            }

            //навигация клавишами
            if (arrowMovement)
            {
                cameraTarget += new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")) * cameraSpeed * 0.2f;
            }
        }

        //регулировка дальности
        cameraDistance = Mathf.Clamp(cameraDistance - Input.GetAxis("Mouse ScrollWheel") * 5f, 5f, 35f);

        //цель камеры
        transform.rotation = Quaternion.Euler(cameraAngle, 0f, 0f);

        //плавное движение
        Vector3 cameraSpacing = transform.forward * -cameraDistance;
        currentCameraTarget = Vector3.Slerp(currentCameraTarget, cameraTarget + cameraSpacing, cameraSpeed * 0.04f);

        transform.position = currentCameraTarget + cameraSpacing;
    }
}