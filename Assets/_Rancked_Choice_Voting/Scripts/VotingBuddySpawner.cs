using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

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

    public VotingBuddyDataHolder SpawnVotingBuddy(
        Vector3 position,
        VotingBuddyData data)
    {
        GameObject newVotingBuddy = votingBuddyPool.Get();
        newVotingBuddy.transform.position = position;

        var votingBuddyDataHolder =
            newVotingBuddy.GetComponent<VotingBuddyDataHolder>();
        
        votingBuddyDataHolder.Data = data;
        
        return votingBuddyDataHolder;
    }

    public void DespawnVotingBuddy(VotingBuddyDataHolder votingBuddyDataHolder)
    {
        votingBuddyPool.Release(votingBuddyDataHolder.gameObject);
    }

    private void OnDestroy()
    {
        // Dispose of the pool when the manager is destroyed
        votingBuddyPool?.Clear();
    }
}