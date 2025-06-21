using UnityEngine;
using UnityEngine.Pool;

public class VotingBuddySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject votingBuddyPrefab;

    private ObjectPool<GameObject> votingBuddyPool;

    static private readonly int MAX_VOTING_BUDDY_COUNT = 1000;

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        votingBuddyPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject buddy = Instantiate(votingBuddyPrefab);
                buddy.SetActive(false);
                return buddy;
            },
            actionOnGet: buddy =>
            {
                buddy.SetActive(true);
            },
            actionOnRelease: buddy =>
            {
                buddy.SetActive(false);
            },
            actionOnDestroy: buddy =>
            {
                Destroy(buddy);
            },
            collectionCheck: false, // Disable collection check for performance
            defaultCapacity: MAX_VOTING_BUDDY_COUNT,
            maxSize: MAX_VOTING_BUDDY_COUNT
        );
    }

    public VotingBuddyBallotHolder SpawnVotingBuddy(
        Vector3 position,
        VotingBuddyBallot data)
    {
        GameObject newVotingBuddy = votingBuddyPool.Get();
        newVotingBuddy.transform.position = position;

        var votingBuddyDataHolder =
            newVotingBuddy.GetComponent<VotingBuddyBallotHolder>();
        
        votingBuddyDataHolder.Ballot = data;
        
        return votingBuddyDataHolder;
    }

    public void DespawnVotingBuddy(VotingBuddyBallotHolder votingBuddyDataHolder)
    {
        votingBuddyPool.Release(votingBuddyDataHolder.gameObject);
    }

    private void OnDestroy()
    {
        // Dispose of the pool when the manager is destroyed
        votingBuddyPool?.Clear();
    }
}