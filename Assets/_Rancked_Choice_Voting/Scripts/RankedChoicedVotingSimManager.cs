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
                //center.AssignBuddy
                nextCandidateCenter.AssignBuddy(votingBuddy);
                
                // Set the color of the VotingBuddy
                votingBuddy.gameObject.GetComponent<MeshRenderer>().material.color = 
                    currentChoice.candidateColor;
            }
        }
    }

    private void MoveVotingBuddies()
    {
        foreach (var votingChoiceCenter in CandidateCenters)
        {
            var assignedBuddies = votingChoiceCenter.assignedBuddies;
            foreach (var votingBuddy in assignedBuddies)
            {
                votingBuddyMover.RegisterMovement(
                    votingBuddy,
                    votingChoiceCenter.GetRandomPositionForVotingBuddy());
            }
        }
    }

    private VotingCandidateCenter GetMajorityCandidate()
    {
        int numberToBeat;
        if (votingBuddyCoint % 2 == 0) // even
        {
            numberToBeat = votingBuddyCoint / 2;
        }
        else // odd
        {
            numberToBeat = (votingBuddyCoint / 2) + 1;
        }
        
        foreach (var votingCandidateCenter in CandidateCenters)
        {
            if (votingCandidateCenter.VoteCount > numberToBeat)
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
        //find candidate with lowest amount of votes...
        VotingCandidateCenter votingCandidateWithLowestPoints = CandidateCenters[0];
        for (int i = 1; i < CandidateCenters.Count; ++i)
        {
            var candidate = CandidateCenters[i];

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
        votingCandidateWithLowestPoints.ClearAssignments();
    }

    public void DisplayResults()
    {
        Debug.Log("Simulation complete. Displaying results...");

        CurrentState = SimulationState.DisplayingResults;

        // Display final results
    }
}