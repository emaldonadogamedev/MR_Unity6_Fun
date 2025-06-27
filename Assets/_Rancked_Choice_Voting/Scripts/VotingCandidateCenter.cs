using System.Collections.Generic;
using UnityEngine;

public class VotingCandidateCenter : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer visualsMeshRenderer;

    //[SerializeField]
    public VotingChoiceCenterVisuals votingChoiceCenterVisuals;

    public CandidateData CandidateData { get; private set; }

    public int VoteCount => assignedBuddies.Count;

    public HashSet<VotingBuddyBallotHolder> assignedBuddies { get; private set; } = new();

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
        float randomAngle = Random.Range(0, 360) * Mathf.Deg2Rad;
        
        float x = Mathf.Cos(randomAngle);
        float y = Mathf.Sin(randomAngle);
        
        float dirAmount = Random.Range(-3.5f, 3.5f);
        
        Vector3 randomPosition = transform.position + (new Vector3(x, 0f, y) * dirAmount);
        
        return randomPosition;
    }
}