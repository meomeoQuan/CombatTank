using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PowerUpManager : MonoBehaviour
{
    [Header("Rewards")]
    public List<RewardData> possibleRewards;
    
    // We'll need references to the other scripts on the tank.
    // Make sure your tank has these components!
    private DualPlayerMovement playerMovement;
    // private PlayerHealth playerHealth;
    // private PlayerShooting playerShooting;
    // private DroneManager droneManager;

    void Awake()
    {
        // Example of getting references:
        // playerHealth = GetComponent<PlayerHealth>();
        // playerShooting = GetComponent<PlayerShooting>();
        // droneManager = GetComponent<DroneManager>();
        
        // Lấy component DualPlayerMovement khi game bắt đầu
        playerMovement = GetComponent<DualPlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PowerUpManager không tìm thấy script DualPlayerMovement trên tank!");
        }
    }

    // This is the public method our BlindBox will call!
    public void GrantRandomReward()
    {
        if (possibleRewards == null || possibleRewards.Count == 0)
        {
            Debug.LogError("No possible rewards assigned in the PowerUpManager!");
            return;
        }

        RewardData chosenReward = GetRandomReward();
        if (chosenReward != null)
        {
            ApplyReward(chosenReward);
        }
    }

    private RewardData GetRandomReward()
    {
        // int totalWeight = 0;
        // foreach (var reward in possibleRewards)
        // {
        //     totalWeight += reward.weight;
        // }

        int randomWeight = Random.Range(0, possibleRewards.Sum(r => r.weight));

        foreach (var reward in possibleRewards)
        {
            if (randomWeight < reward.weight)
            {
                return reward;
            }
            randomWeight -= reward.weight;
        }
        return null; // Should never happen
    }

    private void ApplyReward(RewardData reward)
    {
        // For now, we'll just log it. You will replace this with calls
        // to your actual player scripts.
        Debug.Log($"<color=cyan>Reward Granted: {reward.rewardName}!</color>");

        switch (reward.rewardType)
        {
            case RewardType.Health:
                if (playerMovement != null)
                {
                    // Gọi hàm Heal và truyền vào giá trị từ ScriptableObject
                    // Chuyển đổi reward.value (float) thành int
                    playerMovement.Heal((int)reward.value);
                }
                break;
            case RewardType.DamageBoost:
                // playerShooting.ApplyDamageBoost(reward.value, reward.duration);
                break;
            case RewardType.DroneSupport:
                // droneManager.SpawnDrone(reward.prefab, reward.duration);
                break;
        }
    }
}