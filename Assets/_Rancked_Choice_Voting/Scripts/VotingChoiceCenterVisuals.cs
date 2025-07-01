using TMPro;
using UnityEngine;

public class VotingChoiceCenterVisuals : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer visualMeshRenderer;

    [SerializeField]
    private TMP_Text nameTextbox;

    [SerializeField]
    private TMP_Text voteCountTextbox;

    private int voteCount = 0;
    private string candidateName;

    public void SetName(string name)
    {
        candidateName = name;
        nameTextbox.text = candidateName;
    }

    public void SetVoteCount(int voteCount)
    {
        this.voteCount = voteCount;
        UpdateVoteTexbox();
    }

    public void IncreaseVote()
    {
        voteCount++;
        UpdateVoteTexbox();
    }

    public void DecreaseVote()
    {
        voteCount--;
        UpdateVoteTexbox();
    }

    public void SetMeshColor(Color color)
    {
        visualMeshRenderer.material.color = color;
    }

    public void SetAsWinner()
    {
        nameTextbox.text = $"{candidateName}\nWins!";
    }

    private void UpdateVoteTexbox()
    {
        int totalVotes = RankedChoicedVotingSimManager.Instance.VotingBuddyCoint;

        float percentage = ((float)voteCount / totalVotes);

        voteCountTextbox.text = $"Votes: {voteCount}, {percentage:0.0%}";
    }
}