using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareCameraScaler : MonoBehaviour
{
    float originalSize = 5f;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        originalSize = cam.orthographicSize;
    }

    void Update()
    {
        float newSize = originalSize;
        if (cam.aspect < 1) newSize = originalSize / cam.aspect;
        cam.orthographicSize = newSize;
    }
}
