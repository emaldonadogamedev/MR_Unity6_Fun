using System;
using System.Collections.Generic;
    
[Serializable]
public class VotingBuddyBallot
{
    public bool IsExhausted { get; private set; } = false;

    private readonly List<CandidateData> RankedChoices = new();

    private int CurrentRoundIndex = 0;

    public void AddCandidate(CandidateData candidate)
    {
        RankedChoices.Add(candidate);
    }

    public CandidateData GetCurrentChoice()
    {
        return RankedChoices[CurrentRoundIndex];
    }

    public void AdvanceToNextChoice(List<CandidateData> activeCandidates)
    {
        if(IsExhausted)
            return;

        while (true)
        {
            CurrentRoundIndex++;

            // if we're passed the last choice, ballot is exhausted
            if (CurrentRoundIndex >= RankedChoices.Count)
            {
                IsExhausted = true;
                return;
            }

            var currentCandidate = RankedChoices[CurrentRoundIndex];

            if (activeCandidates.Contains(currentCandidate))
            {
                // ballot moved to next valid candidate
                return;
            }
        }
    }
}