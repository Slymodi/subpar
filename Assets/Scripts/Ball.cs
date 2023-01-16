using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Transform steeringPlaneForward;
    [SerializeField] private Transform steeringPlaneRear;
    [SerializeField] Transform aimingLine;
    [SerializeField] Transform ring;
    [SerializeField] private float minSteeringPlaneScale = 0.5f;
    [SerializeField] private float maxSteeringPlaneScale = 1f;
    [SerializeField] float ringSpinSpeed = 90f;
    [SerializeField] float arrowCenterOffset = 1f;
    [SerializeField] float aimSpeed = 0.5f;
    [Header("Shoot Settings")]
    [SerializeField] GameStateController gameStateController;
    [SerializeField] public float powerMultiplier = 500f;
    [SerializeField] Collider inputRaycastPlane;
    [SerializeField] float minAimDistance = 0.05f;
    [SerializeField] float powerSensitivity = 1f;
    [SerializeField] Projection projection;
    [SerializeField] Ball ballPrefab;

    bool pointerMovedEnough = false;
    Vector3 lastEndTouch = Vector3.zero;

    public enum ShootState
    {
        PUTT,DRIVE,POP
    }

    public ShootState shootState;

    Rigidbody ThisRigidBody
    {
        get
        {
            if (thisRigidBody == null) thisRigidBody = GetComponent<Rigidbody>();
            return thisRigidBody;
        }
    }
    Rigidbody thisRigidBody;

    private bool showingSteering = true;
    public float steeringAngle = 0;    // relative to -z
    public float power = 0;    // between 0 and 1
    public bool isGhost = false;
    bool moveable = true;

    void Start()
    {
        ShowSteering(false);
        shootState = ShootState.DRIVE;
    }

    void Update()
    {
        UpdateSteeringArrows();

        ring.gameObject.SetActive(!moveable);
        if (!moveable) ring.transform.localEulerAngles += Vector3.up * ringSpinSpeed * Time.deltaTime;

        bool aiming = false;

        if (TouchInput.Instance.secondPointerDown) ChangeShootState();

        if (TouchInput.Instance.pointerHeld && gameStateController.State == GameStateController.GameState.aiming)
        {
            if (!pointerMovedEnough)
            {
                float touchChange = (TouchInput.Instance.pointerStartPosition - TouchInput.Instance.pointerPosition).magnitude;
                if (touchChange > minAimDistance)
                {
                    pointerMovedEnough = true;
                }
            }
            else
            {
                projection.SimulateTrajectory(ballPrefab,transform.position, steeringAngle, shootState,power);

                bool justStarted = !aiming;
                aiming = true;

                Vector3? nStartTouch = GetPlanePosition(TouchInput.Instance.pointerStartPosition);
                Vector3? nEndTouch = GetPlanePosition(TouchInput.Instance.pointerPosition);
                if (nStartTouch == null || nEndTouch == null) return;

                Vector3 startTouch = (Vector3)nStartTouch;
                Vector3 rawEndTouch = (Vector3)nEndTouch;
                power = Mathf.Clamp((startTouch - rawEndTouch).magnitude * powerSensitivity, 0, 1);
                Vector3 goalEndTouch = startTouch + (rawEndTouch - startTouch).normalized * power;
                Vector3 endTouch = Vector3.Lerp(lastEndTouch, goalEndTouch, justStarted? 1: Mathf.Min(1, Time.deltaTime * aimSpeed));
                lastEndTouch = endTouch;

                float aimAngle = Vector3.SignedAngle(Vector3.forward, (endTouch - startTouch), Vector3.up);
                SetSteering(aimAngle);
                power = Mathf.Clamp((startTouch - endTouch).magnitude, 0, 1);

                // aim line
                float aimLineLength = (startTouch - endTouch).magnitude / powerSensitivity;
                aimingLine.transform.localScale = new Vector3(
                    aimingLine.transform.localScale.x,
                    aimingLine.transform.localScale.y,
                    aimLineLength / 10f
                );


            }
        }
        else
        {
            pointerMovedEnough = false;
        }

        if (TouchInput.Instance.pointerUp && gameStateController.State == GameStateController.GameState.aiming)
        {
            gameStateController.State = GameStateController.GameState.rolling;
            Shoot();
        }

        ShowSteering(aiming);

    }


    // ============================================================================================
    // Visual Effects 
    // ============================================================================================

    private void UpdateSteeringArrows(bool instant = false)
    {


        float scale = Mathf.Lerp(minSteeringPlaneScale, maxSteeringPlaneScale, power);
        float rotation = steeringAngle;

        {
            steeringPlaneForward.eulerAngles = Vector3.up * rotation;
            steeringPlaneForward.localScale = new Vector3(steeringPlaneForward.localScale.x, steeringPlaneForward.localScale.y, scale);
            Vector3 newPos = RotatePointAroundPivot(Vector3.back * (scale * 5 + arrowCenterOffset), Vector3.zero, Vector3.up * rotation);
            newPos.y = steeringPlaneForward.localPosition.y;
            steeringPlaneForward.localPosition = newPos;
        }

        {
            steeringPlaneRear.eulerAngles = Vector3.up * rotation;
            steeringPlaneRear.localScale = new Vector3(steeringPlaneRear.localScale.x, steeringPlaneRear.localScale.y, scale);
            Vector3 newPos = RotatePointAroundPivot(Vector3.forward * (scale * 5 + arrowCenterOffset), Vector3.zero, Vector3.up * rotation);
            newPos.y = steeringPlaneForward.localPosition.y;
            steeringPlaneRear.localPosition = newPos;
        }
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    public void ShowSteering(bool show)
    {
        if (show != showingSteering)
        {
            showingSteering = show;
            steeringPlaneForward.gameObject.SetActive(showingSteering);
            steeringPlaneRear.gameObject.SetActive(showingSteering);
            aimingLine.gameObject.SetActive(showingSteering);
            UpdateSteeringArrows(true);
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
        else
        {
            return null;
        }
    }

    // ============================================================================================
    // Mechanics 
    // ============================================================================================

    private void ChangeShootState()
    {
        switch(shootState)
        {
            case ShootState.PUTT:
                shootState = ShootState.DRIVE;
                break;
            case ShootState.DRIVE:
                shootState = ShootState.POP;
                break;
            case ShootState.POP:
                shootState = ShootState.PUTT;
                break;
        }
    }

    public void SetSteering(float angle)
    {
        steeringAngle = angle;
    }

    public void Shoot()
    {
        ThisRigidBody.AddForce(GetShootDirection() * power * powerMultiplier);
    }

    private Vector3 GetShootDirection()
    {
        //quaternion angle axis where the angle is multiplied by right and up vector

        switch(shootState)
        {
            case ShootState.PUTT:
                return Quaternion.AngleAxis(steeringAngle + 90, Vector3.up) * new Vector3(0.5f, 0, 0);
            case ShootState.DRIVE:
                return Quaternion.AngleAxis(steeringAngle + 90, Vector3.up) * new Vector3(1, 0, 0);
            case ShootState.POP:
                return Quaternion.AngleAxis(steeringAngle + 90, Vector3.up) * new Vector3(1, 1, 0);
        }
        return Quaternion.AngleAxis(steeringAngle + 90, Vector3.up) * new Vector3(1, 1, 0);
        //return Quaternion.AngleAxis(steeringAngle + 90, Vector3.up);
    }

    public void SetMoveable(bool moveable)
    {
        this.moveable = moveable;
        ThisRigidBody.isKinematic = !moveable;
    }

    public float GetSpeed()
    {
        return ThisRigidBody.velocity.magnitude;
    }
}
