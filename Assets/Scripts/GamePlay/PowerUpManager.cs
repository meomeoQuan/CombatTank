using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Collections;

public class PowerUpManager : MonoBehaviour
{
    [Header("Rewards")]
    public List<RewardData> possibleRewards;

    [Header("UI")]
    public TMP_Text rewardDisplayText; // Kéo TMP_Text vào đây để hiển thị reward

    // We'll need references to the other scripts on the tank.
    // Make sure your tank has these components!
    [Header("Player Components")]
    public ArrowShooter arrowShooter; // Ô trống sẽ hiện ra trong Inspector
    private DualPlayerMovement playerMovement;
    // private PlayerHealth playerHealth;
    // private PlayerShooting playerShooting;
    // private DroneManager droneManager;

    void Awake()
    {
        // Example of getting references:
        // droneManager = GetComponent<DroneManager>();

        // Lấy component DualPlayerMovement khi game bắt đầu
        playerMovement = GetComponent<DualPlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PowerUpManager không tìm thấy script DualPlayerMovement trên tank!");
        }

        if (arrowShooter == null)
        {
            Debug.LogError("PowerUpManager không tìm thấy script ArrowShooter trên tank!");
        }

    }
    void Start()
    {
         // Lấy component ArrowShooter
        // arrowShooter = GetComponent<ArrowShooter>();
 
        
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

        // Hiển thị reward trên UI
        if (rewardDisplayText != null)
        {
            StartCoroutine(ShowRewardText(reward.rewardName));
        }

        switch (reward.rewardType)
        {
            case RewardType.Health:
                playerMovement.Heal((int)reward.value);

                break;
            case RewardType.DamageBoost:
                arrowShooter.ApplyDamageBoost(reward.value, reward.duration);
                    
                break;
            case RewardType.DroneSupport:
                // droneManager.SpawnDrone(reward.prefab, reward.duration);
                break;
            case RewardType.AmmoUpgrade:
                if (arrowShooter != null)
                    arrowShooter.UpgradeAmmo((int)reward.value);

                break;
        }
    }

    // Coroutine để hiển thị text reward tạm thời với hiệu ứng fade out
    private IEnumerator ShowRewardText(string rewardName)
    {
        rewardDisplayText.text = rewardName;
        rewardDisplayText.gameObject.SetActive(true);

        // Lấy Color ban đầu
        Color originalColor = rewardDisplayText.color;
        Color fadeColor = originalColor;
        fadeColor.a = 0f; // Alpha = 0 để mờ dần

        // Hiển thị 1 giây, sau đó fade out trong 1 giây
        yield return new WaitForSeconds(1f);

        // Fade out trong 1 giây
        float fadeTime = 1f;
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            rewardDisplayText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Reset alpha và tắt
        rewardDisplayText.color = originalColor;
        rewardDisplayText.gameObject.SetActive(false);
    }
}