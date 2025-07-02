using UnityEngine;
using UnityEngine.InputSystem;

public class VotingChoiceCenterSpawnPlaneDetector : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private LayerMask spawnerPlaneMask;

    [SerializeField]
    private GameObject votingChoiceCenterCursor;

    private GameObject votingChoiceCenterCursorInstance;

    private VotingChoiceCenterSpawnerCursor votingChoiceCenterSpawnerCursor;

    private bool CanPlaceNewVotingChoice => 
        votingChoiceCenterSpawnerCursor != null &&
            votingChoiceCenterSpawnerCursor.CanPlaceNewVotingChoice;

    private void Start()
    {
        if(mainCamera == null)
            mainCamera = Camera.main;

        if(votingChoiceCenterCursor != null)
        {
            votingChoiceCenterCursorInstance = Instantiate(votingChoiceCenterCursor);

            votingChoiceCenterCursorInstance.SetActive(false);

            votingChoiceCenterSpawnerCursor = 
                votingChoiceCenterCursorInstance.
                    GetComponent<VotingChoiceCenterSpawnerCursor>();
        }
    }

    private void Update()
    {
        var mousePosition = Mouse.current.position.ReadValue();
        var rayFromCamera = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(rayFromCamera, out var hit, 1000f, spawnerPlaneMask))
        {
            // draw a preview...
            if (votingChoiceCenterCursorInstance != null)
            {
                votingChoiceCenterCursorInstance.SetActive(true);
                votingChoiceCenterCursorInstance.transform.position = hit.point;
            }

            if (!Mouse.current.leftButton.wasPressedThisFrame || !CanPlaceNewVotingChoice)
                return;

            // Add a new voting choice center at the random position
            var newCandidate = new CandidateData
            {
                candidateColor = Random.ColorHSV(),
                candidateName = $"Candidate_"
            };

            RankedChoicedVotingSimManager.Instance.AddCandidateCenter(
                hit.point,
                newCandidate);
        }
        else
        {
            // hide the cursor
            if (votingChoiceCenterCursorInstance != null)
            {
                votingChoiceCenterCursorInstance.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        if (votingChoiceCenterCursorInstance != null)
            Destroy(votingChoiceCenterCursorInstance);
    }
}