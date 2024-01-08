using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum VisualizationType { Cube, Sphere, WireCube, WireSphere, Frustum, Line }
public class ChildVisualizer : MonoBehaviour
{
    [SerializeField] float visualizationSize = 0.1f;

    [SerializeField] VisualizationType visualizationType = VisualizationType.WireSphere;

    [SerializeField] bool onlyWhenSelected = true;
    [SerializeField] bool includeInactive = false;

    [Header("Draw Frustum Settings")]
    [SerializeField] float fov;
    [SerializeField] float maxRange, minRange, aspect;

    [Header("Draw Line Settings")]
    [SerializeField] Transform transformToDrawLineTo;
    private void OnDrawGizmos()
    {
        if (onlyWhenSelected) return;

        foreach (Transform child in transform)
        {
            if (includeInactive || child.gameObject.activeSelf)
                RenderChild(child);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!onlyWhenSelected) return;

        foreach (Transform child in transform)
        {
            if (includeInactive || child.gameObject.activeSelf)
                RenderChild(child);
        }
    }

    void RenderChild(Transform child)
    {
        switch (visualizationType)
        {
            case VisualizationType.Cube:
                Gizmos.DrawCube(child.position, new Vector3(visualizationSize, visualizationSize, visualizationSize));
                break;
            case VisualizationType.Sphere:
                Gizmos.DrawSphere(child.position, visualizationSize);
                break;
            case VisualizationType.WireCube:
                Gizmos.DrawWireCube(child.position, new Vector3(visualizationSize, visualizationSize, visualizationSize));
                break;
            case VisualizationType.WireSphere:
                Gizmos.DrawWireSphere(child.position, visualizationSize);
                break;
            case VisualizationType.Frustum:
                Gizmos.DrawFrustum(child.position, fov, maxRange, minRange, aspect);
                break;
            case VisualizationType.Line:
                if (transformToDrawLineTo != null)
                    Gizmos.DrawLine(child.position, transformToDrawLineTo.position);
                else
                    Gizmos.DrawLine(child.position, Vector3.zero);
                break;
        }
    }
}
