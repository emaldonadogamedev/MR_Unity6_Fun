using UnityEngine;

public class VotingBuddyDataHolder : MonoBehaviour
{
    public VotingBuddyData Data;

    [HideInInspector]
    public bool needsToMoveToNextCandidate = true;
}