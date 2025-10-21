using UnityEngine;

public enum RewardType
{
    Health,
    DamageBoost,
    DroneSupport
}

[CreateAssetMenu(fileName = "NewReward", menuName = "Rewards/Reward Data")]
public class RewardData : ScriptableObject // một kiểu dữ liệu đặc biệt trong Unity dùng để lưu trữ thông tin mà không cần gắn vào GameObject
{
    [Header("Reward Info")]
    public RewardType rewardType;
    public string rewardName; // e.g., "+20 Health", "Damage Boost!"
    [Tooltip("The chance of this reward dropping. Higher is more common.")]
    public int weight;

    [Header("Reward Effects")]
    public float value;    // How much health, damage multiplier, etc.
    public float duration; // How long a temporary effect lasts.
    public GameObject prefab;   // The drone prefab to spawn, if any.
}