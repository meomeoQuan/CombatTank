using Assets.Scripts.Models.Characters;
using TMPro;
using UnityEngine;

public class CharacterInfoUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text atkText;
    public TMP_Text speedText;
    public TMP_Text dodgeText;
    public TMP_Text armorText;
    public TMP_Text regenText;

    private Character currentCharacter;

    public void Show(Character character)
    {
        if (character == null)
        {
            Debug.LogWarning("⚠ Character is null — cannot show info!");
            return;
        }

        currentCharacter = character;
        gameObject.SetActive(true); // Hiện panel

        // Hiển thị thông tin
        nameText.text = $"Name: {character.Name}";
        hpText.text = $"HP: {character.HP}";
        atkText.text = $"ATK: {character.ATK}";
        speedText.text = $"Speed: {character.Speed:F1}";
        dodgeText.text = $"Dodge: {character.Dodge:F1}%";
        armorText.text = $"Armor: {character.Armor}";
        regenText.text = $"Regen: {character.Regen}";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void OnCloseButton()
    {
        Hide();
    }
}
