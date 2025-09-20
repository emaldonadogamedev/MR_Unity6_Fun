using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class VotingBuddyMover : MonoBehaviour
{
    [SerializeField]
    [Range(1f, 15f)]
    private float moveSpeed = 8f;

    public class MovementTask
    {
        public VotingBuddyBallotHolder buddy;
        public Vector3 target;
        public float speed;
        public float startDelay;
        public bool isDelayDone = false;

        public UnityEvent<VotingBuddyBallotHolder> OnMovementTaskBegun;
        public UnityEvent<VotingBuddyBallotHolder> OnMovementTaskDone;
    }

    private readonly List<MovementTask> movements = new();

    public MovementTask CreateMovementTask(
        VotingBuddyBallotHolder buddy,
        Vector3 target)
    {
        var newMovementTask = new MovementTask()
        {
            buddy = buddy,
            target = target,
            speed = moveSpeed + Random.Range(-1f, 2f),
            startDelay = Random.Range(-0.5f, 0.5f),
            OnMovementTaskBegun = new UnityEvent<VotingBuddyBallotHolder>(),
            OnMovementTaskDone = new UnityEvent<VotingBuddyBallotHolder>()
        };

        movements.Add(newMovementTask);

        return newMovementTask;
    }

    private void Update()
    {
        for (int i = 0; i < movements.Count; i++)
        {
            var movementTask = movements[i];

            if (!movementTask.isDelayDone)
            {
                ProcessInitialDelay(ref movementTask);
            }
            else
            {
                // continue with the actual movement
                movementTask.buddy.transform.position = Vector3.MoveTowards(
                    movementTask.buddy.transform.position,
                    movementTask.target,
                    Time.deltaTime * movementTask.speed);

                float distance = math.distancesq(
                    movementTask.buddy.transform.position,
                    movementTask.target);

                if (distance >= -0.001f && distance <= 0.001f)
                {
                    movementTask.buddy.needsToMoveToNextCandidate = false;

                    movementTask.OnMovementTaskDone.Invoke(movementTask.buddy);
                    movementTask.OnMovementTaskDone.RemoveAllListeners();

                    movements.RemoveAt(i--);
                }
            }
        }
    }

    private void ProcessInitialDelay(ref MovementTask movementTask)
    {
        // take care of the delay first
        movementTask.startDelay -= Time.deltaTime;
        if (movementTask.startDelay > 0f)
            return;

        movementTask.startDelay = 0f;
        movementTask.OnMovementTaskBegun.Invoke(movementTask.buddy);
        movementTask.OnMovementTaskBegun.RemoveAllListeners();
        movementTask.isDelayDone = true;
    }

    public bool AllArrived()
    {
        return movements.Count == 0;
    }
}