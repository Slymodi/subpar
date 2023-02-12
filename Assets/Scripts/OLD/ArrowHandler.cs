using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHandler : MonoBehaviour
{

void Update()
{
    if(steeringPlaneForward == null || steeringPlaneRear == null) return;
     if (TouchInput.Instance.pointerDown) {
         steeringPlaneForward.gameObject.SetActive(true);
            steeringPlaneRear.gameObject.SetActive(true);
    } else if (TouchInput.Instance.pointerUp) {
        steeringPlaneForward.gameObject.SetActive(false);
        steeringPlaneRear.gameObject.SetActive(false);
    } else if (TouchInput.Instance.pointerHeld) {
        UpdateSteeringArrows();
    } 
}
    [SerializeField] PlayerInputConsumer inputConsumer;
    [SerializeField] public Transform steeringPlaneForward;
    [SerializeField] public Transform steeringPlaneRear;
    [SerializeField] float minSteeringPlaneScale = 0.1f;
    [SerializeField] float maxSteeringPlaneScale = 1f;
    [SerializeField] float arrowCenterOffset = 0.5f;


    private void UpdateSteeringArrows(bool instant = false)
    {


       /*  float scale = Mathf.Lerp(minSteeringPlaneScale, maxSteeringPlaneScale, inputConsumer.power);
        float rotation = inputConsumer.steeringAngle;

        {
            steeringPlaneForward?.eulerAngles = Vector3.up * rotation;
            steeringPlaneForward?.localScale = new Vector3(steeringPlaneForward.localScale.x, steeringPlaneForward.localScale.y, scale);
            Vector3 newPos = RotatePointAroundPivot(Vector3.back * (scale * 5 + arrowCenterOffset), Vector3.zero, Vector3.up * rotation);
            newPos.y = steeringPlaneForward?.localPosition.y;
            steeringPlaneForward?.localPosition = newPos;
        }

        {
            steeringPlaneRear.SeulerAngles = Vector3.up * rotation;
            steeringPlaneRear.localScale = new Vector3(steeringPlaneRear.localScale.x, steeringPlaneRear.localScale.y, scale);
            Vector3 newPos = RotatePointAroundPivot(Vector3.forward * (scale * 5 + arrowCenterOffset), Vector3.zero, Vector3.up * rotation);
            newPos.y = steeringPlaneForward.localPosition.y;
            steeringPlaneRear.localPosition = newPos;
        } */
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }
}
