using UnityEngine;

public class VotingChoiceCenterSpawnerCursor : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer pyramidMeshRenderer;

    [SerializeField]
    private MeshRenderer markerCubeMeshRenderer;

    public bool CanPlaceNewVotingChoice { get; private set; } = true;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("triggered!");

        pyramidMeshRenderer.material.color = Color.red;
        markerCubeMeshRenderer.material.color = Color.red;

        CanPlaceNewVotingChoice = false;
    }

    private void OnTriggerExit(Collider other)
    {
        pyramidMeshRenderer.material.color = Color.greenYellow;
        markerCubeMeshRenderer.material.color = Color.greenYellow;

        CanPlaceNewVotingChoice = true;
    }
}
