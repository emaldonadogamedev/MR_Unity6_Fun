using UnityEngine;

static public class VotingBuddyMaterialChanger
{
    static public void ChangeToMovingToNextDestination(
        Material voterBuddyMaterial,
        Color newColor)
    {
        SetVotingBuddyMaterialBaseColor(
            voterBuddyMaterial,
            newColor);

        SetVotingBuddyMaterialHoppingValues(
            voterBuddyMaterial,
            Random.Range(20f, 27f),
            Random.Range(-2f, 2f));
    }

    static public void ChangeToNotMoving(Material voterBuddyMaterial)
    {
        SetVotingBuddyMaterialHoppingValues(voterBuddyMaterial);
    }

    static private void SetVotingBuddyMaterialBaseColor(
        Material voterBuddyMaterial,
        Color color)
    {
        voterBuddyMaterial.SetColor("_BaseColor", color);
    }

    static private void SetVotingBuddyMaterialHoppingValues(
        Material voterBuddyMaterial,
        float hopMultiplier = 0f,
        float hopDelay = 0f)
    {
        voterBuddyMaterial.SetFloat("_HopMultiplier", hopMultiplier);
        voterBuddyMaterial.SetFloat("_HopDelay", hopDelay);
    }
}