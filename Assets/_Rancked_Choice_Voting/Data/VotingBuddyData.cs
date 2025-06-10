using System;
using System.Collections.Generic;

[Serializable]
public class VotingBuddyData
{
    public List<CandidateData> RankedChoices = new();
    private int CurrentRoundIndex = 0;

    public CandidateData GetCurrentChoice()
    {
        return RankedChoices[CurrentRoundIndex];
    }

    public void AdvanceToNextChoice()
    {
        if (CurrentRoundIndex < RankedChoices.Count - 1)
            CurrentRoundIndex++;
    }
}