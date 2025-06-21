using UnityEngine;

public class VotingBuddyBallotHolder : MonoBehaviour
{
    public VotingBuddyBallot Ballot;

    public Material VotingBuddyMaterial => m_Renderer.material;

    [SerializeField]
    private MeshRenderer m_Renderer;

    [HideInInspector]
    public bool needsToMoveToNextCandidate = true;
}