using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class RankedChoicedVotingSimManager : MonoBehaviour
{
    public enum SimulationState : byte
    { 
        Idle,
        SettingUp,
        Running,
        DisplayingResults
    }

    public SimulationState CurrentState { get; private set; } = SimulationState.Idle;

    [SerializeField]
    [Range(50, 1000)]
    private int votingBuddyCoint = 300;

    public int VotingBuddyCoint => votingBuddyCoint;

    [SerializeField]
    private VotingChoiceCenterSpawner votingChoiceCenterSpawner;

    [SerializeField]
    private VotingBuddySpawner votingBuddySpawner;

    [SerializeField]
    private VotingBuddyMover votingBuddyMover;

    public List<VotingCandidateCenter> CandidateCenters { get; private set; } = new();

    public List<VotingBuddyDataHolder> activeVotingBuddies { get; private set; } = new();

    private int currentRoundNumber = 1;

    public void AddCandidateCenter(Vector3 position, CandidateData candidate)
    {
        var newVotingChoiceCenter =
            votingChoiceCenterSpawner.SpawnVotingCandidateCenter(
                position,
                candidate);

        CandidateCenters.Add(newVotingChoiceCenter);
    }

    public void StartSimulation()
    {
        if (CandidateCenters.Count < 2)
        {
            Debug.LogWarning(
                "At least two candidates are required to start the simulation.");

            return;
        }

        ResetSimulationData();

        SpawnVotingBuddiesForSimulation();

        // Initialize votes and start the simulation logic
        StartCoroutine(SimulationRoutine());
    }

    private void ResetSimulationData()
    {
        foreach (var center in CandidateCenters)
        {
            center.ClearAssignments();
        }

        //TODO: HORRIBLY INEFFICIENT!
        // clear and return voting buddies
        foreach (var votingBuddyData in activeVotingBuddies)
        {
            votingBuddySpawner.DespawnVotingBuddy(votingBuddyData);
        }
        activeVotingBuddies.Clear();

        CurrentState = SimulationState.Running;
        currentRoundNumber = 1;
    }

    private void SpawnVotingBuddiesForSimulation()
    {
        List<CandidateData> candidates = new();

        // allocate the necessary voting buddies
        for (int i = 0; i < votingBuddyCoint; ++i)
        {
            // TODO: HORRIBLY INEFFICIENT!!, JUST FOR QUICK TEST!
            foreach (var candidateCenters in CandidateCenters)
            {
                candidates.Add(candidateCenters.CandidateData);
            }

            var newVotingBuddyData =
                VotingBuddyDataGenerator.GetNewRandomVotingData(candidates);

            activeVotingBuddies.Add(votingBuddySpawner.SpawnVotingBuddy(
                Vector3.zero,
                newVotingBuddyData));
        }
    }

    private IEnumerator SimulationRoutine()
    {
        while (true)
        {
            RevealCurrentVotes();

            MoveVotingBuddies();

            yield return new WaitUntil(votingBuddyMover.AllArrived);

            yield return new WaitForSeconds(0.5f);

            var winner = GetMajorityCandidate();
            if (winner != null)
            {
                Debug.Log($"Winner is {winner.CandidateData.candidateName}!");

                break;
            }

            EliminateLowestCandidate();

            currentRoundNumber++;
        }

        DisplayResults();
    }

    private void RevealCurrentVotes()
    {
        foreach (var votingBuddyDataHolder in activeVotingBuddies)
        {
            var data = votingBuddyDataHolder.Data;

            var currentChoice = data.GetCurrentChoice();

            var nextCandidateCenter = CandidateCenters.Find(
                candidateCenter => candidateCenter.CandidateData == currentChoice);

            if (nextCandidateCenter != null)
            {
                //center.AssignBuddy

                // Set the color of the VotingBuddy
                //var renderer = newVotingBuddy.GetComponent<MeshRenderer>();
                //renderer.material.color = color;
            }
        }
    }

    private void MoveVotingBuddies()
    {
        foreach (var votingChoiceCenter in CandidateCenters)
        {
            // get the center with the least amount of votes
        }

        // Move 
        //buddyMover.RegisterMovement(buddy, center.BuddyArea.position);
    }

    private VotingCandidateCenter GetMajorityCandidate()
    {
        foreach (var votingCandidateCenter in CandidateCenters)
        {
            if (votingCandidateCenter.VoteCount > (votingBuddyCoint / 2))
                return votingCandidateCenter;
        }
        return null;
    }

    public VotingCandidateCenter GetCenterForCandidate(CandidateData candidate)
    {
        return CandidateCenters.Find(center =>
            center.CandidateData == candidate);
    }

    private void EliminateLowestCandidate()
    {
        // change eliminated candidate to eliminated status
    }

    public void DisplayResults()
    {
        Debug.Log("Simulation complete. Displaying results...");

        CurrentState = SimulationState.DisplayingResults;

        // Display final results
    }
}