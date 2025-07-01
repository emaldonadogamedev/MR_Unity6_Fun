using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RankedChoicedVotingSimManager))]
public class RankedChoicedVotingSimManagerEditor : Editor
{
    int candidateNumber = 1;

    private void OnEnable()
    {
        candidateNumber = 1;
        EditorApplication.playModeStateChanged += OnEditorPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnEditorPlayModeStateChanged;
    }

    private void OnEditorPlayModeStateChanged(PlayModeStateChange playModeStateChange)
    {
        if(playModeStateChange == PlayModeStateChange.ExitingPlayMode)
        {
            candidateNumber = 1;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!EditorApplication.isPlaying)
            return;

        EditorGUILayout.LabelField("-- We're playing! --");

        var manager = (RankedChoicedVotingSimManager)target;

        // Add a button to spawn a voting choice center at a random position
        if (GUILayout.Button("Add Random Voting Choice Center"))
        {
            // Find the plane in the scene
            var plane = manager.PlaneFloorMeshRenderer;
            if (plane != null)
            {
                // Get the bounds of the plane
                if (plane.TryGetComponent<MeshRenderer>(out var planeRenderer))
                {
                    Bounds bounds = planeRenderer.bounds;

                    // Generate a random position within the plane's bounds
                    Vector3 randomPosition = new(
                        Random.Range(bounds.min.x, bounds.max.x),
                        bounds.center.y,
                        Random.Range(bounds.min.z, bounds.max.z)
                    );

                    // Add a new voting choice center at the random position
                    var newCandidate = new CandidateData
                    {
                        candidateColor = Random.ColorHSV(),
                        candidateName = $"Candidate_{candidateNumber++}"
                    };
                    
                    Debug.Log(
                        $"Spawning new candidate with name: {newCandidate.candidateName} and color: {newCandidate.candidateColor}");
                    
                    manager.AddCandidateCenter(randomPosition, newCandidate);
                }
                else
                {
                    Debug.LogWarning("Plane does not have a MeshRenderer component.");
                }
            }
            else
            {
                Debug.LogWarning("Plane object not found in the scene.");
            }
        }
        
        // Add a button to spawn a voting choice center at a random position
        if (GUILayout.Button("Start simulation!"))
        {
            manager.StartSimulation();
        }
    }
}