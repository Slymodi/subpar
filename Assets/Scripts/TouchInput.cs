using UnityEngine;
using System.Linq;

/// <summary>
/// Abstraction on simple touch/mouse input
/// </summary>
public class TouchInput: Singleton<TouchInput>
{
    // positions are between (0, 0) = bottom left and (1, 1) = top right
    public Vector2 pointerStartPosition;
    public Vector2 pointerPosition;

    public bool pointerDown;
    public bool pointerUp;
    public bool pointerHeld;

    public bool secondPointerDown;
    public bool secondPointerUp;
    public bool secondPointerHeld;

    public Vector3 nStartTouch;
    public Vector3 nEndTouch;

    [SerializeField] Collider inputRaycastPlane
    {
        get
        {
            if (_inputRaycastPlane == null) _inputRaycastPlane = Resources.FindObjectsOfTypeAll<Collider>().Where(obj => obj.name == "InputRaycastPlane").FirstOrDefault();
            return _inputRaycastPlane;
        }
    }
    [SerializeField]private Collider _inputRaycastPlane;

    void Update()
    {
        bool lastPointerHeld = pointerHeld;
        bool lastSecondPointerHeld = secondPointerHeld;
        pointerHeld = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            pointerPosition = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.height);
            pointerHeld = true;
        }  
        if (Input.GetMouseButton(0)) {
            pointerPosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            pointerHeld = true;
            nEndTouch = GetPlanePosition(pointerPosition).GetValueOrDefault();
            Vector3 goalEndTouch = nStartTouch + (nEndTouch - nStartTouch).normalized;
            nEndTouch = Vector3.Lerp(nEndTouch, goalEndTouch, 1);
        }
        if (Input.GetMouseButton(1)) {
            secondPointerHeld = true;
        }

        

        pointerDown = !lastPointerHeld && pointerHeld;
        pointerUp = lastPointerHeld && !pointerHeld;

        secondPointerDown = !lastSecondPointerHeld && secondPointerHeld;
        secondPointerUp = lastSecondPointerHeld && !secondPointerHeld;


        if (pointerDown) {
            pointerStartPosition = pointerPosition;
            nStartTouch = GetPlanePosition(pointerPosition).GetValueOrDefault();
        }
    }

    Vector3? GetPlanePosition(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(screenPosition.x * Screen.width, screenPosition.y * Screen.height));
        RaycastHit hit;
        if (inputRaycastPlane.Raycast(ray, out hit, 100.0f))
        {
            return hit.point;
        }
        return null;
        
    }
}