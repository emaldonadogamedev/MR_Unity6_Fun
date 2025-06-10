using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RankedChoicedVotingSimManager))]
public class RankedChoicedVotingSimManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!EditorApplication.isPlaying)
            return;

        EditorGUILayout.LabelField("-- We're playing! --");

        // Add a button to spawn a voting choice center at a random position
        if (GUILayout.Button("Add Random Voting Choice Center"))
        {
            var manager = (RankedChoicedVotingSimManager)target;

            // Find the plane in the scene
            GameObject plane = GameObject.Find("Plane");
            if (plane != null)
            {
                // Get the bounds of the plane
                MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
                if (planeRenderer != null)
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
                        candidateName = $"Random Candidate_{Random.Range(0, 100)}"
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
            var manager = (RankedChoicedVotingSimManager)target;
            
            manager.StartSimulation();
        }
    }
}