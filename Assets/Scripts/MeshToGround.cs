using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshToGround : MonoBehaviour
{
    [SerializeField] float maxRayDistance = 100f;
    [SerializeField] float upwardsOffset = 50f;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Vector3 translatedPointOffset = new Vector3();
    MeshFilter meshFilter;
    Vector3[] vertices;
    Vector3[] originalVertices;

    void Awake() {
        meshFilter = GetComponent<MeshFilter>();
        vertices = meshFilter.mesh.vertices;
        originalVertices = meshFilter.mesh.vertices;
    }

    void LateUpdate()
    {
        UpdateMesh();
    }

    void UpdateMesh() {
        for (int i = 0; i < originalVertices.Length; i ++) {
            Vector3? hit = GroundRaycastHit(transform.TransformPoint(originalVertices[i]));
            if (hit != null) {
                vertices[i] = transform.InverseTransformPoint((Vector3) hit);
            } else {
                vertices[i] = originalVertices[i];
            }
        }
        meshFilter.mesh.vertices = vertices;
    }

    Vector3? GroundRaycastHit(Vector3 point) {
        RaycastHit hit;
        Vector3 origin = point + Vector3.up * upwardsOffset;
        if (Physics.Raycast(origin, Vector3.down, out hit, maxRayDistance, layerMask)) {
            return hit.point + translatedPointOffset;
        } else {
            return null;
        }
    }
}
