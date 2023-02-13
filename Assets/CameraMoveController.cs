using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveStatus
{
    eDefault=1,
    eJumpPosition=2,
    eMove=3,
    eZoom=4,
}

public class CameraMoveController {
    private MoveStatus status = MoveStatus.eDefault;

    public Camera camera;
    public GameObject cameraObject;

    private Vector3 originalPosition;
    private Vector3 targetPosition;

    private bool moveEnd = false;
    private float moveSmooth = 1f;

    private float beginTime;
    private Vector2 touchPosition;
    private Vector3 beginWorldPosition;

    private float cameraMinX=-120;
    private float cameraMaxX = 120;
    private float cameraMinZ = -120;
    private float cameraMaxZ=120;

    private float targetSize=0;

    private bool zoomEnd = false;
    private float zoomSmooth = 1f;

    private float minZoomDistance=5;
    private float maxZoomDistance=20;

    private float zoomMoveScale = 2f;
    private float springMinZoomDistance = 4;
    private float springMaxZoomDistance = 21;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void OnUpdate()
    {
        switch (status)
        {
            case MoveStatus.eDefault:
                break;
            case MoveStatus.eMove:
                MoveUpdate();
                break;
            case MoveStatus.eJumpPosition:
                JumpMoveUpdate();
                break;
            case MoveStatus.eZoom:
                ZoomUpdate();
                break;
            default:
                break;
        }
    }

    void MoveUpdate()
    {
        originalPosition = Vector3.Lerp(originalPosition, targetPosition, moveSmooth * Time.deltaTime);
        SetCameraPosition(originalPosition);
        if (moveEnd)
        {
            float absX = Mathf.Abs(originalPosition.x - targetPosition.x);
            float absY = Mathf.Abs(originalPosition.y - targetPosition.y);
            float absZ = Mathf.Abs(originalPosition.z - targetPosition.z);

            if (absX < 0.01 && absY < 0.01 && absZ < 0.01)
            {
                SetCameraPosition(targetPosition);
                SetStatus(MoveStatus.eDefault);
            }
        }
    }

    void JumpMoveUpdate()
    {

    }

    void ZoomUpdate()
    {
        float orthographicSize = targetSize;
        float currentSize = camera.orthographicSize;
        if (zoomEnd)
        {
            if (currentSize > maxZoomDistance)
            {
                orthographicSize = maxZoomDistance;
            }
            if (currentSize < minZoomDistance)
            {
                orthographicSize = minZoomDistance;
            }
        }
        float value = Mathf.Lerp(currentSize, orthographicSize, zoomSmooth * Time.deltaTime);
        SetCameraSize(value);
    }

    Vector3 VerifyCameraPositionLimit(Vector3 position)
    {
        if (position.x < cameraMinX)
        {
            position.x = cameraMinX;
        }
        if (position.x > cameraMaxX)
        {
            position.x = cameraMaxX;
        }
        if (position.z < cameraMinZ)
        {
            position.z = cameraMinZ;
        }
        if (position.z > cameraMaxZ)
        {
            position.z = cameraMaxZ;
        }
        return position;
    }

    void SetCameraPosition(Vector3 position)
    {
        position = VerifyCameraPositionLimit(position);
        cameraObject.transform.position = position;
    }

    Vector3 GetCameraPosition()
    {
        return cameraObject.transform.position;
    }

    void SetStatus(MoveStatus move_status)
    {
        if (status != move_status)
        {
            ExitStatus(status);
        }
        status = move_status;
        EnterStatus(status);
    }

    void ExitStatus(MoveStatus move_status)
    {

    }

    void EnterStatus(MoveStatus move_status)
    {

    }

    void SetCameraSize(float value)
    {
        camera.orthographicSize = value;
    }

    public void MoveTouchBegan(Vector3 position)
    {
        beginTime = Time.time;
        touchPosition = position;
        beginWorldPosition = CameraTools.CalcScreenToPlanePositon(camera,position.x,position.y,0);
        SetStatus(MoveStatus.eMove);
        originalPosition = GetCameraPosition();
        targetPosition = originalPosition;
        moveSmooth = 10f;
    }

    public void MoveTouchMoved(Vector3 position)
    {
        SetStatus(MoveStatus.eMove);
        originalPosition = GetCameraPosition();
        Vector3 currentWorldPosition= CameraTools.CalcScreenToPlanePositon(camera, position.x, position.y, 0);
        Vector3 moveDiff = currentWorldPosition - beginWorldPosition;
        targetPosition = originalPosition - moveDiff;
    }

    public void MoveTouchEnded(Vector3 position)
    {
        Vector3 currentWorldPosition = CameraTools.CalcScreenToPlanePositon(camera, position.x, position.y, 0);
        Vector3 moveDiff = currentWorldPosition - beginWorldPosition;
        float duration = Time.time - beginTime;
        Vector2 touchOffset = new Vector2((position.x-touchPosition.x)/Screen.width,(position.y-touchPosition.y)/Screen.height);
        float speed = touchOffset.magnitude / duration;
        originalPosition = GetCameraPosition();
        if (speed>1.5)
        {
            moveSmooth = 2f;
            targetPosition = originalPosition - moveDiff * 6;
        }
        else
        {
            targetPosition = originalPosition - moveDiff;
            moveSmooth = 10f;
        }
        moveEnd = true;
    }

    public void MoveTouchCancel()
    {
        SetStatus(MoveStatus.eDefault);
    }

    public void ZoomBegan()
    {
        SetStatus(MoveStatus.eZoom);
        //originalPosition = GetCameraPosition();
        //targetPosition = originalPosition;
        zoomEnd = false;
    }

    public void ZoomMoved(float distance)
    {
        if (Mathf.Abs(distance)<0.1f)
        {
            return;
        }
        float currentSize = camera.orthographicSize;
        targetSize = currentSize - distance * zoomMoveScale;
        if (targetSize>springMaxZoomDistance)
        {
            targetSize = springMaxZoomDistance;
        }
        if (targetSize < springMinZoomDistance)
        {
            targetSize = springMinZoomDistance;
        }
    }

    public void ZoomEnded()
    {
        zoomSmooth = 2;
        zoomEnd = true;
    }
}
