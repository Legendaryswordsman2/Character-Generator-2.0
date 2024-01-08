using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] AIPath aiPath;
    [SerializeField] AIDestinationSetter destinationSetter;
    public void SetEndDestination(Transform endTarget)
    {
        destinationSetter.target = endTarget;
    }

    private void Update()
    {
        if(aiPath.reachedDestination && destinationSetter.target != null)
            Destroy(gameObject);
    }
}