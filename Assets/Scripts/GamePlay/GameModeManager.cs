using UnityEngine;
using UnityEngine.UI;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;

    [Header("Main Panels")]
    public GameObject gameModeUI;
    public GameObject difficultyPanel;

    [Header("Buttons")]
    public Button btnPvsP;
    public Button btnPvsB;
    public Button btnP1;
    public Button btnP2;
    public Button btnStart;
    public Button btnDiff1;
    public Button btnDiff2;
    public Button btnDiff3;

    private enum GameMode { None, PvsP, PvsB }
    private GameMode selectedMode = GameMode.None;
    private int selectedDifficulty = 1;
    private int selectedPlayer = 1;

    // Selected = black, Normal = white (bạn muốn màu đen cho selected)
    private Color selectedColor = new Color(0.96f, 0.84f, 0.67f);
    private Color normalColor = Color.white;
    // Những màu phụ cho trạng thái không-selected
    private Color normalHighlighted = new Color(0.95f, 0.95f, 0.95f);
    private Color normalPressed = new Color(0.9f, 0.9f, 0.9f);

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        gameModeUI.SetActive(true);
        difficultyPanel.SetActive(false);

        // Gán sự kiện click
        btnPvsP.onClick.AddListener(() => SelectMode(GameMode.PvsP));
        btnPvsB.onClick.AddListener(() => SelectMode(GameMode.PvsB));
        btnP1.onClick.AddListener(() => SelectPlayer(1));
        btnP2.onClick.AddListener(() => SelectPlayer(2));
        btnDiff1.onClick.AddListener(() => SelectDifficulty(1));
        btnDiff2.onClick.AddListener(() => SelectDifficulty(2));
        btnDiff3.onClick.AddListener(() => SelectDifficulty(3));
        btnStart.onClick.AddListener(StartGameMode);

        // Mặc định
        SelectPlayer(1);
        SelectDifficulty(1);
        UpdateModeButtonColors();
    }

    void SelectMode(GameMode mode)
    {
        selectedMode = mode;

        if (mode == GameMode.PvsP)
        {
            difficultyPanel.SetActive(false);
            Debug.Log("Selected: Player vs Player");
        }
        else if (mode == GameMode.PvsB)
        {
            difficultyPanel.SetActive(true);
            Debug.Log("Selected: Player vs Bot");
        }

        UpdateModeButtonColors();
    }

    void SelectPlayer(int playerNum)
    {
        selectedPlayer = playerNum;
        Debug.Log("Play as: Player " + playerNum);
        UpdatePlayerButtonColors();
    }

    void SelectDifficulty(int diff)
    {
        selectedDifficulty = diff;
        Debug.Log("Difficulty: " + diff);
        UpdateDifficultyButtonColors();
    }

    void StartGameMode()
    {
        if (selectedMode == GameMode.None)
        {
            Debug.LogWarning("Please select a mode first!");
            return;
        }

        gameModeUI.SetActive(false);

        if (selectedMode == GameMode.PvsP)
            GameManager.Instance.ShowReadyPanel_PvsP();
        else if (selectedMode == GameMode.PvsB)
            GameManager.Instance.ShowReadyPanel_PvsB(selectedPlayer, selectedDifficulty);
    }

    // -----------------------
    // Update colors
    // -----------------------
    void UpdateModeButtonColors()
    {
        SetButtonTint(btnPvsP, selectedMode == GameMode.PvsP);
        SetButtonTint(btnPvsB, selectedMode == GameMode.PvsB);
    }

    void UpdatePlayerButtonColors()
    {
        SetButtonTint(btnP1, selectedPlayer == 1);
        SetButtonTint(btnP2, selectedPlayer == 2);
    }

    void UpdateDifficultyButtonColors()
    {
        SetButtonTint(btnDiff1, selectedDifficulty == 1);
        SetButtonTint(btnDiff2, selectedDifficulty == 2);
        SetButtonTint(btnDiff3, selectedDifficulty == 3);
    }

    // Hàm set ColorBlock hợp lý cho button (đảm bảo hover/pressed không đổi sang màu lạ)
    void SetButtonTint(Button btn, bool isSelected)
    {
        ColorBlock cb = btn.colors;

        if (isSelected)
        {
            // Nếu được chọn: tất cả trạng thái đều là màu đen
            cb.normalColor = selectedColor;
            cb.highlightedColor = selectedColor;
            cb.pressedColor = selectedColor;
            cb.selectedColor = selectedColor;
        }
        else
        {
            // Nếu không chọn: nền trắng, hover/press nhẹ khác
            cb.normalColor = normalColor;
            cb.highlightedColor = normalHighlighted;
            cb.pressedColor = normalPressed;
            cb.selectedColor = normalColor;
        }

        // Tốc độ chuyển màu (tweak nếu muốn mượt hơn)
        cb.colorMultiplier = 1f;
        cb.fadeDuration = 0.05f;

        btn.colors = cb;
    }
}
