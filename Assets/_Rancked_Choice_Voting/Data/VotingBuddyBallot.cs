using System;
using System.Collections.Generic;
    
[Serializable]
public class VotingBuddyBallot
{
    public List<CandidateData> RankedChoices = new();
    public bool isExhausted { get; private set; } = false;

    private int CurrentRoundIndex = 0;

    public CandidateData GetCurrentChoice()
    {
        return RankedChoices[CurrentRoundIndex];
    }

    public void AdvanceToNextChoice(List<CandidateData> activeCandidates)
    {
        if(isExhausted)
            return;

        while (true)
        {
            CurrentRoundIndex++;

            // if we're passed the last choice, ballot is exhausted
            if (CurrentRoundIndex >= RankedChoices.Count)
            {
                isExhausted = true;
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