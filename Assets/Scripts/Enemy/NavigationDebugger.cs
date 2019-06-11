using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationDebugger : MonoBehaviour
{
    public NavMeshAgent agentToDebug;

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agentToDebug != null)
        {
            if (agentToDebug.hasPath)
            {
                line.positionCount = agentToDebug.path.corners.Length;
                line.SetPositions(agentToDebug.path.corners);
                line.enabled = true;
            }
            else
            {
                line.enabled = true;
            }
        }
    }
}
