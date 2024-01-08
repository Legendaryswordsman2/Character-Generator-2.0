using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] AIPath aiPath;
    [SerializeField] AIDestinationSetter destinationSetter;
    [SerializeField] Animator animator;

    private void Start()
    {
        animator.runtimeAnimatorController = NPCSpawner.Instance.GetNPCVariant();
    }

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