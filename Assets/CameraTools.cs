using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTools
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    static Vector3 CalcPlanePosition(Camera camera,Vector3 screenPos,float planeHeight)
    {
        Vector3 worldPos = camera.ScreenToWorldPoint(screenPos);
        Vector3 direction = (worldPos - camera.transform.position).normalized;
        Vector3 planeNormal = new Vector3(0, 1, 0);
        Vector3 planePos = new Vector3(0, planeHeight, 0);
        if (camera.orthographic)
        {
            direction = camera.transform.forward;
        }
        float hitTime = Vector3.Dot(planeNormal,(planePos-worldPos))/Vector3.Dot(planeNormal,direction);
        Vector3 targetPoint = worldPos + hitTime * direction;
        return targetPoint;
    }

    public static Vector3 CalcScreenToPlanePositon(Camera camera,float x,float y,float planeHeight)
    {
        float z = 0f;
        if (camera.orthographic)
        {
            float height = camera.transform.position.y - planeHeight;
            z = height / Mathf.Cos(Mathf.PI/180*camera.transform.rotation.x);
        }
        else
        {
            z = camera.nearClipPlane;
        }
        Vector3 screenPos = new Vector3(x, y, z);
        return CalcPlanePosition(camera, screenPos, planeHeight);
    }
}
