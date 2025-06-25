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

    class CandidateSimulationData
    {
        public VotingCandidateCenter votingCandidateCenter;
        public bool isEliminated = false;
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

    private List<CandidateSimulationData> candidateCenters = new();

    private List<VotingBuddyBallotHolder> activeVotingBuddies = new();

    private int currentRoundNumber = 1;

    public void AddCandidateCenter(Vector3 position, CandidateData candidate)
    {
        var newVotingChoiceCenter =
            votingChoiceCenterSpawner.SpawnVotingCandidateCenter(
                position,
                candidate);

        candidateCenters.Add(
            new CandidateSimulationData()
            {
                isEliminated = false,
                votingCandidateCenter = newVotingChoiceCenter
            });
    }

    public void StartSimulation()
    {
        if (candidateCenters.Count < 2)
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
        foreach (var center in candidateCenters)
        {
            center.isEliminated = false;
            center.votingCandidateCenter.ClearAssignments();
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
        List<VotingCandidateCenter> candidates;

        // allocate the necessary voting buddies
        for (int i = 0; i < votingBuddyCoint; ++i)
        {
            // TODO: HORRIBLY INEFFICIENT!!, JUST FOR QUICK TEST!
            candidates = new();
            foreach (var candidateCenters in candidateCenters)
            {
                candidates.Add(candidateCenters.votingCandidateCenter);
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

            EliminateCandidateWithLowestVotes();

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
            
            var ballot = votingBuddy.Ballot;

            if(ballot.IsExhausted)
            {
                // Set the color of the VotingBuddy
                votingBuddy.gameObject.GetComponent<MeshRenderer>().material.color =
                    new Color(.5f, .5f, .5f, 0.5f);

                continue;
            }

            var currentChoice = ballot.GetCurrentChoice();

            var nextCandidateCenter = candidateCenters.Find(
                candidateCenter =>
                    !candidateCenter.isEliminated &&
                    candidateCenter.votingCandidateCenter == currentChoice);

            if (nextCandidateCenter != null)
            {
                nextCandidateCenter.votingCandidateCenter.AssignBuddy(votingBuddy);
            }
        }
        
        foreach (var candidateCenter in candidateCenters)
        {
            Debug.Log(
                $"Candidate {candidateCenter.votingCandidateCenter.CandidateData.candidateName} has {candidateCenter.votingCandidateCenter.VoteCount} votes.");
        }
    }

    private void MoveVotingBuddies()
    {
        foreach (var votingBuddy in activeVotingBuddies)
        {
            if (!votingBuddy.needsToMoveToNextCandidate)
                continue;

            // For now, by default, assume vote is exhausted (cheaper function)
            var nextDestination = this.transform.position;

            if (!votingBuddy.Ballot.IsExhausted)
            {
                var currCandidateData = votingBuddy.Ballot.GetCurrentChoice();

                // find the next candidate center that's not eliminated
                var nextCandidateCenter = 
                    candidateCenters.Find(center =>
                        !center.isEliminated &&
                        currCandidateData == center.votingCandidateCenter);

                nextDestination =
                    nextCandidateCenter.votingCandidateCenter.
                        GetRandomPositionForVotingBuddy();
            }

            var newMovementTask = votingBuddyMover.RegisterMovement(
                votingBuddy,
                nextDestination);

            newMovementTask.OnMovementTaskBegun.AddListener(OnVoteBuddyMovementBegin);
            newMovementTask.OnMovementTaskDone.AddListener(OnVoteBuddyMovementDone);
        }
    }

    private void OnVoteBuddyMovementDone(VotingBuddyBallotHolder votingBuddyBallotHolder)
    {
        VotingBuddyMaterialChanger.ChangeToNotMoving(
            votingBuddyBallotHolder.VotingBuddyMaterial);

        var currentCandidate = votingBuddyBallotHolder.Ballot.GetCurrentChoice();

        currentCandidate.votingChoiceCenterVisuals.IncreaseVote();
    }

    private void OnVoteBuddyMovementBegin(VotingBuddyBallotHolder votingBuddyBallotHolder)
    {
        var currentCandidate = votingBuddyBallotHolder.Ballot.GetCurrentChoice();

        VotingBuddyMaterialChanger.ChangeToMovingToNextDestination(
            votingBuddyBallotHolder.VotingBuddyMaterial,
            currentCandidate.CandidateData.candidateColor);

        if (currentRoundNumber > 1)
        {
            currentCandidate.votingChoiceCenterVisuals.DecreaseVote();
        }
    }

    private bool TryGetMajorityCandidate(out VotingCandidateCenter votingCandidateCenter)
    {
        int numberToBeat = votingBuddyCoint / 2;

        votingCandidateCenter = null;
        
        foreach (var candidateCenter in candidateCenters)
        {
            if (candidateCenter.votingCandidateCenter.VoteCount > numberToBeat)
                votingCandidateCenter = candidateCenter.votingCandidateCenter;
        }
        
        return votingCandidateCenter != null;
    }

    private void EliminateCandidateWithLowestVotes()
    {
        var participatingCandidateCenters = 
            candidateCenters.FindAll(center => center.isEliminated == false);
        
        // Find the candidate with the lowest amount of votes...
        var candidateWithLowestVotes = participatingCandidateCenters[0];
        for (int i = 1; i < participatingCandidateCenters.Count; ++i)
        {
            var candidate = participatingCandidateCenters[i];
            
            if (candidate.votingCandidateCenter.assignedBuddies.Count < 
                candidateWithLowestVotes.votingCandidateCenter.assignedBuddies.Count)
            {
                candidateWithLowestVotes = candidate;
            }
        }
        candidateWithLowestVotes.isEliminated = true;

        // prepare the list of remaining active candidates
        var activeCandidates = new List<VotingCandidateCenter>();
        foreach(var participatingCandidate in participatingCandidateCenters)
        {
            if (!participatingCandidate.isEliminated)
                activeCandidates.Add(
                    participatingCandidate.votingCandidateCenter);
        }

        // advance the buddies of this candidate to their next choice
        foreach (var votingBuddy in 
            candidateWithLowestVotes.votingCandidateCenter.assignedBuddies)
        {
            votingBuddy.Ballot.AdvanceToNextChoice(activeCandidates);
            votingBuddy.needsToMoveToNextCandidate = true;
        }

        candidateWithLowestVotes.votingCandidateCenter.ClearAssignments();

        var candidateName = 
            candidateWithLowestVotes.votingCandidateCenter.CandidateData.candidateName;

        Debug.Log($"Candidate {candidateName} has been eliminated!");
    }

    private void DisplayResults()
    {
        Debug.Log(
            $"Simulation complete after {currentRoundNumber} rounds. Displaying results...");

        CurrentState = SimulationState.DisplayingResults;

        // Display final results
    }
}