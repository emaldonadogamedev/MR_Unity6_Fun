using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VotingCandidateCenter : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer visualsMeshRenderer;

    public VotingChoiceCenterVisuals votingChoiceCenterVisuals;

    public CandidateData CandidateData { get; private set; }

    public int VoteCount => 
        assignedBuddies == null ? 0 : assignedBuddies.Count;

    public bool IsEliminated => 
        CandidateData == null ? false : CandidateData.isEliminated;

    public HashSet<VotingBuddyBallotHolder> assignedBuddies { get; private set; } = new();

    private readonly float voteBuddyPlacementRadius = 6f;

    private float GetCurrentCandidatePercentage()
    {
        int totalVotes = RankedChoicedVotingSimManager.Instance.VotingBuddyCount;

        float percentage = ((float)VoteCount / totalVotes);

        return percentage;
    }

    public void Initialize(CandidateData candidate)
    {
        CandidateData = candidate;

        votingChoiceCenterVisuals.SetName(candidate.candidateName);
        votingChoiceCenterVisuals.SetMeshColor(candidate.candidateColor);
        votingChoiceCenterVisuals.SetVoteCount(0);
    }

    public void AssignBuddy(VotingBuddyBallotHolder buddy)
    {
        assignedBuddies.Add(buddy);
        votingChoiceCenterVisuals.IncreaseVote();
    }

    public void RemoveBuddy(VotingBuddyBallotHolder buddy)
    {
        assignedBuddies.Remove(buddy);
        votingChoiceCenterVisuals.DecreaseVote();
    }

    public void ClearAssignments()
    {
        assignedBuddies.Clear();
        votingChoiceCenterVisuals.SetVoteCount(0);
    }

    public Vector3 GetRandomPositionForVotingBuddy()
    {
        float randomAngleInRadians = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        
        float x = Mathf.Cos(randomAngleInRadians);
        float y = Mathf.Sin(randomAngleInRadians);

        float addedRadius = GetCurrentCandidatePercentage();

        float dirAmount = Random.Range(0f, voteBuddyPlacementRadius + addedRadius);
        
        Vector3 randomPosition = transform.position + (new Vector3(x, 0f, y) * dirAmount);
        
        return randomPosition;
    }
}