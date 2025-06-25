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

    public void SetName(string name)
    {
        nameTextbox.text = name;
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

    private void UpdateVoteTexbox()
    {
        voteCountTextbox.text = $"Votes: {voteCount}";
    }
}