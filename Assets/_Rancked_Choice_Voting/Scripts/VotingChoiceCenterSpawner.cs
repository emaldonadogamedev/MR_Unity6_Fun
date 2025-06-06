using UnityEngine;
using System.Collections.Generic;

public class VotingChoiceCenterSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject votingChoiceCenterPrefab;

    public VotingCandidateCenter SpawnVotingCandidateCenter(
        Vector3 position,
        CandidateData candidate)
    {
        GameObject newVotingChoiceCenterGO = Instantiate(
            votingChoiceCenterPrefab,
            position,
            Quaternion.identity);

        if (newVotingChoiceCenterGO.TryGetComponent<VotingCandidateCenter>(
            out var votingChoiceCenter))
        {
            votingChoiceCenter.Initialize(candidate);
        }

        return votingChoiceCenter;
    }

    public void DespawnVotingChoiceCenter(VotingCandidateCenter center)
    {
        Destroy(center.gameObject);
    }
}