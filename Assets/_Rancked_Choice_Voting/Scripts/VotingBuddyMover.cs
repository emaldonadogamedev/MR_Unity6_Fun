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
    private float moveSpeed = 7f;

    class MovementTask
    {
        public VotingBuddyBallotHolder buddy;
        public Vector3 target;
        public float speed;
        public float startDelay;

        public UnityEvent<VotingBuddyBallotHolder> OnMovementTasktDone;
    }

    private readonly List<MovementTask> movements = new();

    public void RegisterMovement(
        VotingBuddyBallotHolder buddy,
        Vector3 target,
        UnityAction<VotingBuddyBallotHolder> OnDoneCallback)
    {
        var newMovementTask = new MovementTask()
        {
            buddy = buddy,
            target = target,
            speed = moveSpeed + Random.Range(-1f, 2f),
            startDelay = Random.Range(-0.5f, 0.5f),
            OnMovementTasktDone = new UnityEvent<VotingBuddyBallotHolder>()
        };

        newMovementTask.OnMovementTasktDone.AddListener(OnDoneCallback);

        movements.Add(newMovementTask);
    }

    void Update()
    {
        for (int i = 0; i < movements.Count; i++)
        {
            var movementTask = movements[i];

            // take care of the delay first
            movementTask.startDelay -= Time.deltaTime;
            if (movementTask.startDelay > 0f)
                continue;

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
                movementTask.OnMovementTasktDone.Invoke(movementTask.buddy);
                movementTask.OnMovementTasktDone.RemoveAllListeners();

                movements.RemoveAt(i--);
            }
        }
    }

    public bool AllArrived()
    {
        return movements.Count == 0;
    }
}