using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMove : MonoBehaviour
{
    private CameraMoveController controller = new CameraMoveController();

    private float scrollValue;

    private int zoomEndFrame;

    private int touchCount;

    private Vector2 beginPosition;

    private float touchDistance;

    private float zoomScale=0.5f;

    private bool isMoveAction = false;

    // Start is called before the first frame update
    void Start()
    {
        controller.cameraObject = gameObject;
        controller.camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor)
        {
            EditorUpdate();
        }
        else
        {
            MobileUpdate();
        }
        controller.OnUpdate();
    }

    void OnTouchBegan(Vector2 position)
    {
        beginPosition = position;
        controller.MoveTouchBegan(position);
    }

    void OnTouchMoved(Vector2 position)
    {
        if(isMoveAction==false && Vector2.Distance(position, beginPosition) > 10)
        {
            beginPosition = position;
            isMoveAction = true;
        }
        if(isMoveAction && Vector2.Distance(position, beginPosition) > 0)
        {
            beginPosition = position;
            controller.MoveTouchMoved(position);
        }
    }

    void OnTouchEnded(Vector2 position)
    {
        if (isMoveAction==false&&Vector2.Distance(position,beginPosition)>10)
        {
            isMoveAction = true;
        }
        if (isMoveAction)
        {
            controller.MoveTouchEnded(position);
        }
        else
        {
            controller.MoveTouchCancel();
        }
        beginPosition = Vector2.zero;
        isMoveAction = false;
    }

    void EditorUpdate()
    {
        float value = Input.GetAxis("Mouse ScrollWheel");

        if (value!=0)//scrollValue==0 && 
        {
            controller.ZoomBegan();
            //scrollValue = value;

            float distance = value * 100;
            controller.ZoomMoved(distance);
            scrollValue = value;
        }
        else if(scrollValue != 0 && value != 0)
        {
            
        }
        else if (scrollValue != 0 && value == 0)
        {
            controller.ZoomEnded();
            scrollValue = value;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            OnTouchBegan(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            OnTouchMoved(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnTouchEnded(Input.mousePosition);
        }
    }

    void MobileUpdate()
    {
        int frameCount = Time.frameCount;
        int count = Input.touchCount;
        if (count==0)
        {
            zoomEndFrame = -1;
            touchCount = count;
        }
        else if (count == 1)
        {
            Touch touch = Input.GetTouch(0);
            TouchPhase phase = touch.phase;
            switch (phase)
            {
                case TouchPhase.Began:
                    OnTouchBegan(touch.position);
                    break;
                case TouchPhase.Moved:
                    if (zoomEndFrame>=0 && frameCount - zoomEndFrame > 2)
                    {
                        OnTouchBegan(touch.position);
                        zoomEndFrame = -1;
                    }
                    OnTouchMoved(touch.position);
                    break;
                case TouchPhase.Stationary:
                    if (zoomEndFrame >= 0 && frameCount - zoomEndFrame > 2)
                    {
                        OnTouchBegan(touch.position);
                        zoomEndFrame = -1;
                    }
                    break;
                case TouchPhase.Ended:
                    OnTouchEnded(touch.position);
                    break;
                case TouchPhase.Canceled:
                    break;
                default:
                    break;
            }
            touchCount = 1;
        }
        else if (count==2)
        {
            beginPosition = Vector2.zero;

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            TouchPhase phase0 = touch0.phase;
            TouchPhase phase1 = touch1.phase;

            if (phase0==TouchPhase.Began&& phase1 == TouchPhase.Began)
            {
                if (touchCount==1)
                {
                    OnTouchEnded(touch0.position);
                }
                touchDistance = Vector2.Distance(touch0.position, touch1.position);
                controller.ZoomBegan();
            }
            else if(phase0 == TouchPhase.Moved || phase1 == TouchPhase.Moved)
            {
                float nowDistance = Vector2.Distance(touch0.position, touch1.position);
                float distance = nowDistance - touchDistance;
                controller.ZoomMoved(distance * zoomScale);
                touchDistance = nowDistance;
            }
            else if (phase0 == TouchPhase.Ended || phase1 == TouchPhase.Ended)
            {
                controller.ZoomEnded();
                zoomEndFrame = Time.frameCount;
                touchDistance = 0;
            }
            touchCount = 2;
        }
    }
}
