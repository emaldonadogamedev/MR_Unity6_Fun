using UnityEngine;

public class VotingChoiceCenterSpawnerCursor : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer pyramidMeshRenderer;

    [SerializeField]
    private MeshRenderer markerCubeMeshRenderer;

    public bool CanPlaceNewVotingChoice { get; private set; } = true;
    
    private readonly Color VALID_COLOR = new Color(0.092f, 0.983f, 0f, 0.282f);

    private readonly Color INVALID_COLOR = Color.indianRed;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("triggered!");

        pyramidMeshRenderer.material.color = INVALID_COLOR;
        markerCubeMeshRenderer.material.color = INVALID_COLOR;

        CanPlaceNewVotingChoice = false;
    }

    private void OnTriggerExit(Collider other)
    {
        pyramidMeshRenderer.material.color = VALID_COLOR;
        markerCubeMeshRenderer.material.color = VALID_COLOR;

        CanPlaceNewVotingChoice = true;
    }
}
