using System.Collections.Generic;
using UnityEngine;

static public class VotingBuddyDataGenerator
{
    public static VotingBuddyBallot GetNewRandomVotingData(List<CandidateData> candidates)
    {
        var result = new VotingBuddyBallot();

        while (candidates.Count > 0)
        {
            int randomIndex = Random.Range(0, candidates.Count);

            result.RankedChoices.Add(candidates[randomIndex]);

            candidates.RemoveAt(randomIndex);
        }

        return result;
    }
}
