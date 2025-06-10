using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class VotingCandidateCenter : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer visualsMeshRenderer;

    public CandidateData CandidateData { get; private set; }

    public int VoteCount => assignedBuddies.Count;

    public List<VotingBuddyDataHolder> assignedBuddies { get; private set; } = new();

    public void Initialize(CandidateData candidate)
    {
        CandidateData = candidate;

        visualsMeshRenderer.material.color = candidate.candidateColor;
    }

    public void AssignBuddy(VotingBuddyDataHolder buddy)
    {
        assignedBuddies.Add(buddy);
    }

    public void ClearAssignments()
    {
        assignedBuddies.Clear();
    }

    public Vector3 GetRandomPositionForVotingBuddy()
    {
        float randomAngle = Random.Range(0, 360) * Mathf.Deg2Rad;
        
        float x = Mathf.Cos(randomAngle);
        float y = Mathf.Sin(randomAngle);
        
        var dirAmount = Random.Range(-3.5f, 3.5f);
        
        var randomPosition = transform.position + (new Vector3(x, 0, y) * dirAmount);
        
        return randomPosition;
    }
}