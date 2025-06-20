using UnityEngine;

public class VotingBuddyBallotHolder : MonoBehaviour
{
    public VotingBuddyBallot Ballot;

    [HideInInspector]
    public bool needsToMoveToNextCandidate = true;
}