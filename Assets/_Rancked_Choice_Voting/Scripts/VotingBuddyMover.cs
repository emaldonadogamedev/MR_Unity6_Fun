using System.Collections.Generic;
using UnityEngine;

public class VotingBuddyMover : MonoBehaviour
{
    [SerializeField]
    [Range(1f, 10f)]
    private float moveSpeed = 3f;

    private readonly List<(VotingBuddyDataHolder buddy, Vector3 target)> movements = new();

    public void RegisterMovement(VotingBuddyDataHolder buddy, Vector3 target)
    {
        movements.Add((buddy, target));
    }

    void Update()
    {
        for (int i = 0; i < movements.Count; i++)
        {
            var (buddy, target) = movements[i];

            buddy.transform.position = Vector3.MoveTowards(
                buddy.transform.position,
                target,
                Time.deltaTime * moveSpeed);

            if (Vector3.Distance(buddy.transform.position, target) <= 0.01f)
            {
                buddy.needsToMoveToNextCandidate = false;
                movements.RemoveAt(i--);
            }
        }
    }

    public bool AllArrived()
    {
        return movements.Count == 0;
    }
}