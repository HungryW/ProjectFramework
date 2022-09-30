using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GameFramework;
using GameFrameworkPackage;

public class DragCamera2D : MonoBehaviour
{
    /*
     *TODO: 
     *  DONE: replace dolly with bezier dolly system
     *  DONE: add dolly track smoothing (Pro Feature)
     *  DONE: add dolly track straightening  (Pro Feature)
     *  DONE: Dolly track + gizmo colours
     *  DONE: add non tracked constant speed dolly system(continuous movement based on time)  (Pro Feature)
     *  WONTDO: [REPLACED BY FEATURE BELOW] add button to split dolly track evenly (between start and end) for time based dolly movement
     *  DONE: button to adjust times on all waypoints so camera moves at a constant speed  (Pro Feature)
     *  DONE: add per waypoint time (seconds on this segment)  (Pro Feature)
     *  DONE: add scaler for time to next waypoint in scene viewe gui  (Pro Feature)
     *  improve GUI elements (full custon editor inspector)
     *  DONE:    add waypoint gui  scene view button  (Pro Feature)
     *  DONE: better designed example scenes
     *  DONE: option to lock camera to track even if object escapes area
     *  add multiple dolly tracks to allow creating loops etc
     *  add track change triggers
     *  add bounds ids for multiple bounds
     *  add bounds triggers(e.g. small bounds until x event(obtain key etc) then larger bounds
    */

    public Camera cam;
    [Header("Camera Movement")]
    [Tooltip("Allow the Camera to be dragged.")]
    public bool dragEnabled = true;
    [Range(-5, 5)]
    [Tooltip("Speed the camera moves when dragged.")]
    public float dragSpeed = -0.06f;

    [Tooltip("Pixel Border to trigger edge scrolling")]
    public int edgeBoundary = 20;
    [Range(0, 100)]
    [Tooltip("Speed the camera moves Mouse enters screen edge.")]
    public float edgeSpeed = 1f;
    [Header("Keyboard Input")]
    [Tooltip("Enable or disable Keyboard input")]
    public bool keyboardInput = false;
    [Tooltip("Invert keyboard direction")]
    public bool inverseKeyboard = false;

    [Tooltip("Enable or disable touch input ")]
    public bool touchEnabled = false;
    [Tooltip("Drag Speed for touch controls")]
    [Range(-5, 5)]
    public float touchDragSpeed = 0.08f;
    [Tooltip("Enable or disable touch Inertia ")]
    public bool m_bTouchInertiaEnable = true;
    [Tooltip("Enable or disable touch Inertia ")]
    [Range(0.0001f, 1)]
    public float m_fnertiaASpeed = 0.1f;
    private Vector2 m_v2InertiaInitSpeed = Vector2.zero;

    [Header("Zoom")]
    [Tooltip("Enable or disable zooming")]
    public bool zoomEnabled = true;
    [Tooltip("Scale drag movement with zoom level")]
    public bool linkedZoomDrag = true;
    [Tooltip("Maximum Zoom Level")]
    public float maxZoom = 10;
    [Tooltip("Minimum Zoom Level")]
    [Range(0.01f, 10)]
    public float minZoom = 0.5f;
    [Tooltip("The Speed the zoom changes")]

    [Range(0.01f, 10f)]
    public float zoomStepSize = 0.5f;

    [Range(0.001f, 5f)]
    public float zoomTouchStepSize = 0.02f;

    [Header("Follow Object")]
    public GameObject followTarget;
    [Range(0.01f, 1f)]
    public float lerpSpeed = 0.5f;
    public Vector3 offset = new Vector3(0, 0, -10);


    [Header("Camera Bounds")]
    public bool clampCamera = true;
    public CameraBounds bounds;
    public Dc2dDolly dollyRail;

    private bool m_bLockCamera = false;

    public void SetCameraLock( bool a_bLock)
    {
        m_bLockCamera = a_bLock;
    }

    // private vars
    Vector3 bl;
    Vector3 tr;
    private Vector2 touchOrigin = -Vector2.one;

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void Update()
    {
        if (m_bLockCamera)
        {
            return;
        }

        if (_IsPointerOverUIObject())
        {
            return;
        }
#if  UNITY_EDITOR || UNITY_STANDALONE
        if (dragEnabled)
        {
            panControl();
        }

        if (edgeBoundary > 0)
        {
            edgeScroll();
        }


        if (zoomEnabled)
        {
            zoomControl();
        }
#endif

#if UNITY_EDITOR || (UNITY_IOS || UNITY_ANDROID)
        if (touchEnabled)
        {
            doTouchControls();
        }
        if (m_bTouchInertiaEnable)
        {
            _doTouchInertiaMove();
        }
#endif
        if (followTarget != null)
        {
            transform.position = Vector3.Lerp(transform.position + offset, followTarget.transform.position + offset, lerpSpeed);
        }

        if (clampCamera)
        {
            cameraClamp();
        }

        if (dollyRail != null)
        {
            stickToDollyRail();
        }

    }
#if UNITY_EDITOR || UNITY_STANDALONE
    private void edgeScroll()
    {
        float x = 0;
        float y = 0;
        if (Input.mousePosition.x >= Screen.width - edgeBoundary)
        {
            // Move the camera
            x = Time.deltaTime * edgeSpeed;
        }
        if (Input.mousePosition.x <= 0 + edgeBoundary)
        {
            // Move the camera
            x = Time.deltaTime * -edgeSpeed;
        }
        if (Input.mousePosition.y >= Screen.height - edgeBoundary)
        {
            // Move the camera
            y = Time.deltaTime * edgeSpeed
;
        }
        if (Input.mousePosition.y <= 0 + edgeBoundary)
        {
            // Move the camera
            y = Time.deltaTime * -edgeSpeed
;
        }
        transform.Translate(x, y, 0);
    }


    //click and drag
    public void panControl()
    {
        // if keyboard input is allowed
        if (keyboardInput)
        {
            float x = -Input.GetAxis("Horizontal") * dragSpeed;
            float y = -Input.GetAxis("Vertical") * dragSpeed;

            if (linkedZoomDrag)
            {
                x *= cam.orthographicSize;
                y *= cam.orthographicSize;
            }

            if (inverseKeyboard)
            {
                x = -x;
                y = -y;
            }
            transform.Translate(x, y, 0);
        }
        // if mouse is down
        if (Input.GetMouseButton(0))
        {
            float x = Input.GetAxis("Mouse X") * dragSpeed;
            float y = Input.GetAxis("Mouse Y") * dragSpeed;

            if (linkedZoomDrag)
            {
                x *= cam.orthographicSize;
                y *= cam.orthographicSize;
            }
            transform.Translate(x, y, 0);
        }
    }

    // managae zooming
    public void zoomControl()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            cam.orthographicSize = cam.orthographicSize - zoomStepSize;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back            
        {
            cam.orthographicSize = cam.orthographicSize + zoomStepSize;
        }
        clampZoom();
    }
#endif

#if  UNITY_EDITOR || (UNITY_IOS || UNITY_ANDROID)
    public void doTouchControls()
    {
        Touch[] touches = Input.touches;
        if (touches.Length > 0)
        {

            //get camera move on the basis of 1 touch
            if (touches.Length == 1)
            {
                if (touches[0].phase == TouchPhase.Began)
                {
                    m_v2InertiaInitSpeed = Vector2.zero;
                }
                else if (touches[0].phase == TouchPhase.Moved)
                {
                    Vector2 delta = touches[0].deltaPosition;
                    //get new position delta from touch delta and move sensitivity
                    //(also inverse that as camera moves in opposite direction of touch to give the effect of world movement)
                    Vector2 newPositionDelta = new Vector2(delta.x * touchDragSpeed * Time.deltaTime * -1, delta.y * touchDragSpeed * Time.deltaTime * -1);
                    newPositionDelta *= cam.orthographicSize;
                    //add delta to camera's local position(so as to ignore the  camera rotations)
                    transform.localPosition += new Vector3(newPositionDelta.x, newPositionDelta.y, 0);
                }
                else if (touches[0].phase == TouchPhase.Ended)
                {
                    m_v2InertiaInitSpeed = touches[0].deltaPosition;
                }
            }

            //get camera zoom on the basis of 2 touches
            if (touches.Length == 2)
            {
                Touch touchZero = touches[0];
                Touch touchOne = touches[1];

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
                //calculate how far/close touches have moved to update the zoom according to that
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                cam.orthographicSize += deltaMagnitudeDiff * zoomTouchStepSize;
            }
        }
        clampZoom();
        cameraClamp();
    }

    private void _doTouchInertiaMove()
    {
        if (m_v2InertiaInitSpeed == Vector2.zero)
        {
            return;
        }
        m_v2InertiaInitSpeed *= Mathf.Pow(m_fnertiaASpeed, Time.deltaTime);
        if (m_v2InertiaInitSpeed.sqrMagnitude < 0.000001)
            m_v2InertiaInitSpeed = Vector2.zero;

        Vector2 newPositionDelta = m_v2InertiaInitSpeed * touchDragSpeed * cam.orthographicSize * Time.deltaTime * -1;
        transform.localPosition += new Vector3(newPositionDelta.x, newPositionDelta.y, 0);
    }
#endif

    public void addCameraDolly()
    {
        if (dollyRail == null)
        {
            GameObject go = new GameObject("Dolly");
            Dc2dDolly dolly = go.AddComponent<Dc2dDolly>();

            Dc2dWaypoint wp1 = new Dc2dWaypoint();
            wp1.position = new Vector3(0, 0, 0);

            Dc2dWaypoint wp2 = new Dc2dWaypoint();
            wp2.position = new Vector3(1, 0, 0);

            Dc2dWaypoint[] dc2dwaypoints = new Dc2dWaypoint[2];
            dc2dwaypoints[0] = wp1;
            dc2dwaypoints[1] = wp2;
            wp1.endPosition = wp2.position;

            dolly.allWaypoints = dc2dwaypoints;

            this.dollyRail = dolly;

#if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = go;
            UnityEditor.SceneView.FrameLastActiveSceneView();
#endif
        }
    }

    public void addCameraBounds()
    {
        if (bounds == null)
        {
            GameObject go = new GameObject("CameraBounds");
            CameraBounds cb = go.AddComponent<CameraBounds>();
            cb.guiColour = new Color(0, 0, 1f, 0.1f);
            cb.pointa = new Vector3(20, 20, 0);
            this.bounds = cb;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    private void clampZoom()
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        Mathf.Max(cam.orthographicSize, 0.1f);
        //CGameEntryMgr.Event.Fire(null, ReferencePool.Acquire<CEventMainCameraSizeChangeArgs>().Fill(cam.orthographicSize));
    }

    private void ZoomOrthoCamera(Vector3 zoomTowards, float amount)
    {
        // Calculate how much we will have to move towards the zoomTowards position
        float multiplier = (1.0f / cam.orthographicSize * amount);
        // Move camera
        transform.position += (zoomTowards - transform.position) * multiplier;
        // Zoom camera
        cam.orthographicSize -= amount;
        // Limit zoom
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    // Clamp Camera to bounds
    public void cameraClamp()
    {
        tr = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, -transform.position.z));
        bl = cam.ScreenToWorldPoint(new Vector3(0, 0, -transform.position.z));

        if (bounds == null)
        {
            Debug.Log("Clamp Camera Enabled but no Bounds has been set.");
            return;
        }

        float boundsMaxX = bounds.pointa.x;
        float boundsMinX = bounds.transform.position.x;
        float boundsMaxY = bounds.pointa.y;
        float boundsMinY = bounds.transform.position.y;

        if (tr.x > boundsMaxX)
        {
            transform.position = new Vector3(transform.position.x - (tr.x - boundsMaxX), transform.position.y, transform.position.z);
        }

        if (tr.y > boundsMaxY)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - (tr.y - boundsMaxY), transform.position.z);
        }

        if (bl.x < boundsMinX)
        {
            transform.position = new Vector3(transform.position.x + (boundsMinX - bl.x), transform.position.y, transform.position.z);
        }

        if (bl.y < boundsMinY)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + (boundsMinY - bl.y), transform.position.z);
        }
    }

    public void stickToDollyRail()
    {
        if (dollyRail != null && followTarget != null)
        {
            Vector3 campos = dollyRail.getPositionOnTrack(followTarget.transform.position);
            transform.position = new Vector3(campos.x, campos.y, transform.position.z);
        }
    }

    private bool _IsInputOverUI()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
#else
        if (Input.touchCount >= 1 )
#endif
        {
#if  UNITY_EDITOR || UNITY_STANDALONE
            if (EventSystem.current.IsPointerOverGameObject())
#else
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#endif
            {
                return true;
            }
        }

        return false;
    }


    private bool _IsPointerOverUIObject()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (!Input.GetMouseButton(0))
        {
            return false;
        }
#else
         if (Input.touchCount < 1)
        {
            return false;
        }
#endif

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
#if UNITY_EDITOR || UNITY_STANDALONE
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#else
        eventDataCurrentPosition.position = Input.GetTouch(0).position;
#endif
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.layer == 5) //5 = UI layer
            {
                return true;
            }
        }

        return false;
    }



}

public class CEventMainCameraSizeChangeArgs : GameEventArgs
{
    public static readonly int EventId = typeof(CEventMainCameraSizeChangeArgs).GetHashCode();
    public override int Id
    {
        get
        {
            return EventId;
        }
    }

    public float fCameraSize
    {
        get;
        private set;
    }


    public CEventMainCameraSizeChangeArgs Fill(float a_fCameraSize)
    {
        fCameraSize = a_fCameraSize;
        return this;
    }

    public override void Clear()
    {
    }
}
