using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VotingCandidateCenter))]
public class VotingCandidateCenterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!EditorApplication.isPlaying)
            return;

        EditorGUILayout.LabelField("-- We're playing! --");

        var candidateCenter = (VotingCandidateCenter)target;

        // Add a button to spawn a voting choice center at a random position
        if (GUILayout.Button("Remove this Choice Center"))
        {
            RankedChoicedVotingSimManager.Instance.RemoveCandidateCenter(candidateCenter);
        }
    }
}