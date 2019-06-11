using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public Vector2 gridWorldSize;
    public static Vector2 gridSize;

    void Awake()
    {
        gridSize = gridWorldSize;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
    }
}