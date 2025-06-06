using System.Collections.Generic;
using UnityEngine;

public class VotingCandidateCenter : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer visualsMeshRenderer;

    public CandidateData CandidateData { get; private set; }

    public int VoteCount => assignedBuddies.Count;

    private List<Transform> assignedBuddies = new();

    public void Initialize(CandidateData candidate)
    {
        CandidateData = candidate;

        visualsMeshRenderer.material.color = candidate.candidateColor;
    }

    public void AssignBuddy(Transform buddy)
    {
        assignedBuddies.Add(buddy);
    }

    public void ClearAssignments()
    {
        assignedBuddies.Clear();
    }
}