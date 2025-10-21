using UnityEngine;

public class PlayerDamagedInvincibility : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    private float _invincibilityDuration;
    
    private InvincibilityController _invincibilityController;

    private void Awake()
    {
        _invincibilityController = GetComponent<InvincibilityController>();
    }

    public void StartInvincibility()
    {
        _invincibilityController.StartInvincibility(_invincibilityDuration);
    }

}
