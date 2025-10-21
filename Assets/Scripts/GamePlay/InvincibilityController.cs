using System.Collections;
using UnityEngine;

public class InvincibilityController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private HealthController _healthController;

    private void Awake()
    {
        _healthController = GetComponent<HealthController>();
    }

    public void StartInvincibility(float invincibilityDuration) //lấy khoảng tgian mong muốn làm tham số
    {
        StartCoroutine(InvincibilityCoroutine(invincibilityDuration));
    }

    private IEnumerator InvincibilityCoroutine(float invicibilityDuration)
    {
        _healthController.IsInvincible = true; //kích hoạt bất khả chiến bại
        yield return new WaitForSeconds(invicibilityDuration); //bất khả trong khoảng tgian nhất định rồi trả lại False
        _healthController.IsInvincible = false;
    }
    

}

