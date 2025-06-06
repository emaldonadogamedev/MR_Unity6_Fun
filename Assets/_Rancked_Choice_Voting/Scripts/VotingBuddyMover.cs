using System.Collections.Generic;
using UnityEngine;

public class VotingBuddyMover : MonoBehaviour
{
    [SerializeField]
    [Range(1f, 10f)]
    private float moveSpeed = 3f;

    private List<(Transform buddy, Vector3 target)> movements = new();

    public void RegisterMovement(Transform buddy, Vector3 target)
    {
        movements.Add((buddy, target));
    }

    void Update()
    {
        for (int i = 0; i < movements.Count; i++)
        {
            var (buddy, target) = movements[i];

            buddy.position = Vector3.MoveTowards(
                buddy.position,
                target,
                Time.deltaTime * moveSpeed);

            if (Vector3.Distance(buddy.position, target) <= 0.01f)
                movements.RemoveAt(i--);
        }
    }

    public bool AllArrived()
    {
        return movements.Count == 0;
    }
}