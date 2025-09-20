using System.Collections.Generic;
using UnityEngine;

public static class VotingBuddyDataGenerator
{
    public static VotingBuddyBallot GetNewRandomVotingData(List<VotingCandidateCenter> candidates)
    {
        var newVotingBuddyBallot = new VotingBuddyBallot();

        while (candidates.Count > 0)
        {
            int randomIndex = Random.Range(0, candidates.Count);

            newVotingBuddyBallot.AddCandidate(candidates[randomIndex]);

            candidates.RemoveAt(randomIndex);
        }

        return newVotingBuddyBallot;
    }
}
