using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    private VotingChoiceCenterSpawner votingChoiceCenterSpawner;

    [SerializeField]
    private VotingBuddySpawner votingBuddySpawner;

    [SerializeField]
    private VotingBuddyMover votingBuddyMover;

    private List<VotingCandidateCenter> CandidateCenters = new();

    private List<VotingBuddyDataHolder> activeVotingBuddies = new();

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
        List<CandidateData> candidates;

        // allocate the necessary voting buddies
        for (int i = 0; i < votingBuddyCoint; ++i)
        {
            // TODO: HORRIBLY INEFFICIENT!!, JUST FOR QUICK TEST!
            candidates = new();
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
            Debug.Log($"Starting simulation round {currentRoundNumber}!");
            
            RevealCurrentVotes();

            MoveVotingBuddies();

            yield return new WaitUntil(votingBuddyMover.AllArrived);

            yield return new WaitForSeconds(0.5f);
            
            if (TryGetMajorityCandidate(out var winner))
            {
                Debug.Log(
                    $"Winner is {winner.CandidateData.candidateName} with {winner.VoteCount} votes!");

                break;
            }

            LowestCandidateChangeVotesToNextChoice();

            currentRoundNumber++;
        }

        DisplayResults();
    }

    private void RevealCurrentVotes()
    {
        foreach (var votingBuddy in activeVotingBuddies)
        {
            if(!votingBuddy.needsToMoveToNextCandidate)
                continue;
            
            var data = votingBuddy.Data;

            var currentChoice = data.GetCurrentChoice();

            var nextCandidateCenter = CandidateCenters.Find(
                candidateCenter => candidateCenter.CandidateData == currentChoice);

            if (nextCandidateCenter != null)
            {
                nextCandidateCenter.AssignBuddy(votingBuddy);
                
                // Set the color of the VotingBuddy
                votingBuddy.gameObject.GetComponent<MeshRenderer>().material.color = 
                    currentChoice.candidateColor;
            }
        }
        
        foreach (var candidateCenter in CandidateCenters)
        {
            Debug.Log(
                $"Candidate {candidateCenter.CandidateData.candidateName} has {candidateCenter.VoteCount} votes.");
        }
    }

    private void MoveVotingBuddies()
    {
        foreach (var votingChoiceCenter in CandidateCenters)
        {
            var assignedBuddies = votingChoiceCenter.assignedBuddies;
            foreach (var votingBuddy in assignedBuddies)
            {
                if(!votingBuddy.needsToMoveToNextCandidate)
                    continue;
                
                votingBuddyMover.RegisterMovement(
                    votingBuddy,
                    votingChoiceCenter.GetRandomPositionForVotingBuddy());
            }
        }
    }

    private bool TryGetMajorityCandidate(out VotingCandidateCenter votingCandidateCenter)
    {
        int numberToBeat = votingBuddyCoint / 2;

        votingCandidateCenter = null;
        
        foreach (var candidateCenter in CandidateCenters)
        {
            if (candidateCenter.VoteCount > numberToBeat)
                votingCandidateCenter = candidateCenter;
        }
        
        return votingCandidateCenter != null;
    }

    private void LowestCandidateChangeVotesToNextChoice()
    {
        var participatingCandidateCenters = CandidateCenters.FindAll(center => center.VoteCount > 0);
        
        // Find the candidate with the lowest amount of votes...
        var votingCandidateWithLowestPoints = participatingCandidateCenters[0];
        for (int i = 1; i < participatingCandidateCenters.Count; ++i)
        {
            var candidate = participatingCandidateCenters[i];
            
            if (candidate.assignedBuddies.Count < votingCandidateWithLowestPoints.assignedBuddies.Count)
            {
                votingCandidateWithLowestPoints = candidate;
            }
        }

        // assign buddies of this candidate to the next choice...
        foreach (var votingBuddy in votingCandidateWithLowestPoints.assignedBuddies)
        {
            votingBuddy.Data.AdvanceToNextChoice();
            votingBuddy.needsToMoveToNextCandidate = true;
        }
        Debug.Log(
            $"Candidate {votingCandidateWithLowestPoints.CandidateData.candidateName} has been eliminated!");
        
        votingCandidateWithLowestPoints.ClearAssignments();
    }

    private void DisplayResults()
    {
        Debug.Log($"Simulation complete after {currentRoundNumber} rounds. Displaying results...");

        CurrentState = SimulationState.DisplayingResults;

        // Display final results
    }
}