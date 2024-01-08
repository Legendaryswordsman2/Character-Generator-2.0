using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class NPCRandomPointWander : MonoBehaviour
{
    [SerializeField] Transform[] wanderPoints;

    AIPath aiPath;
    AIDestinationSetter destinationSetter;

    bool atDestination = false;

    Transform prevPoint = null;

    private void Awake()
    {

        if (!TryGetComponent(out aiPath))
        {
            Destroy(this);
            return;
        }

        if (!TryGetComponent(out destinationSetter))
        {
            Destroy(this);
            return;
        }

        if (wanderPoints.Length < 2)
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        int index = Random.Range(0, wanderPoints.Length - 1);

        transform.position = wanderPoints[index].position;
        prevPoint = wanderPoints[index];

        SelectNewWanderPoint();
    }

    void SelectNewWanderPoint()
    {
        //Debug.Log("Setting new wander point");
        do
        {
            destinationSetter.target = wanderPoints[Random.Range(0, wanderPoints.Length)];
        } while (destinationSetter.target == prevPoint);
        prevPoint = destinationSetter.target;

        //Debug.Log("New Wander Point Set: " + destinationSetter.target);
    }

    private void Update()
    {
        if (aiPath.reachedDestination && !atDestination)
        {
            SelectNewWanderPoint();
            atDestination = true;
        }
        else if (!aiPath.reachedDestination)
        {
            atDestination = false;
        }
    }
}