using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour {
    
	public Transform target;
    public Vector3 offset = new Vector3(10, 10, 10);
    public float speed = 1f;

	void LateUpdate () {
		// Early out if we don't have a target
		if (!target) return;

        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * speed);
	}
}