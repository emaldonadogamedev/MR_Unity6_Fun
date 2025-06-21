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

    public void SetName(string name)
    {
        nameTextbox.text = name;
    }

    public void SetVoteCount(int voteCount)
    {
        voteCountTextbox.text = $"{voteCount}";
    }

    public void SetMeshColor(Color color)
    {
        visualMeshRenderer.material.color = color;
    }
}