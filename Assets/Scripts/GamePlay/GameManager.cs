using Assets.Scripts.DataController;
using System.Collections;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Ready Check (TMP)")]
    public TMP_Text aReadyText;
    public TMP_Text bReadyText;
    public TMP_Text countdownText;
    public GameObject gamePlay;        // panel khi chờ READY
    public GameObject gameWinUI;       // panel khi có người thắng
    public TMP_Text winText;           // text hiển thị ai thắng
    [Header("Spawn Points")]
    public Transform spawnA;
    public Transform spawnB;

    private DualPlayerMovement playerA;
    private DualPlayerMovement playerB;

    private bool isAReady = false;
    private bool isBReady = false;
    private bool gameStarted = false;
    public bool IsGameRunning => gameStarted;
                                        

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        UpdateUI();
        DataController.Initialize();
        countdownText.gameObject.SetActive(false);
        gameWinUI.SetActive(false);
    }

    void Update()
    {
        UpdateUI();
        if (!gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                isAReady = true;
                
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                isBReady = true;
            }

            if (isAReady && isBReady && !gameStarted && !gameWinUI.activeSelf)
            {
                StartCoroutine(StartCountdown());
            }
        }
    }

    void UpdateUI()
    {
        int dotCount = (int)(Time.time % 4);
        string dots = new string('.', dotCount);

        aReadyText.text = "Player A: " + (isAReady ? "READY" : "WAITING" + " (PRESS N) \n" + dots);
        bReadyText.text = "Player B: " + (isBReady ? "READY" : "WAITING" + " (PRESS 2) \n" + dots);
    }


    IEnumerator StartCountdown()
    {
        gameStarted = true;
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "FIGHT!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
        aReadyText.gameObject.SetActive(false);
        bReadyText.gameObject.SetActive(false);
        gamePlay.SetActive(false);


        foreach (var enabler in FindObjectsByType<RequireGameStart>(FindObjectsSortMode.None))
        {
            enabler.EnableComponent();
        }
    }
    public void PlayAgain()
    {
        foreach (var player in FindObjectsByType<DualPlayerMovement>(FindObjectsSortMode.None))
        {
            if (player.characterType == DualPlayerMovement.CharacterType.CharacterA)
                player.spawnPoint = spawnA;
            else if (player.characterType == DualPlayerMovement.CharacterType.CharacterB)
                player.spawnPoint = spawnB;

            player.ResetCharacter();
        }

        // Reset UI
        gameWinUI.SetActive(false);


        gamePlay.SetActive(true);
        aReadyText.gameObject.SetActive(true);
        bReadyText.gameObject.SetActive(true);

        UpdateUI();
    }


    public void EndGame(DualPlayerMovement.CharacterType loser)
    {
        foreach (var req in FindObjectsByType<RequireGameStart>(FindObjectsSortMode.None))
        {
            req.DisableComponent();
        }
        gameStarted = false;
        isAReady = false;
        isBReady = false;

        // Tắt Game UI, bật màn hình thắng
        //gameUI.SetActive(false);

        gameWinUI.SetActive(true);

        // Ai thua thì người kia thắng
        string winner = (loser == DualPlayerMovement.CharacterType.CharacterA) ? "Player B" : "Player A";
        winText.text = winner + " WINS!";
    }

    // Tho
    public void RegisterPlayer(DualPlayerMovement player)
    {
        if (player.characterType == DualPlayerMovement.CharacterType.CharacterA)
        {
            playerA = player;
            Debug.Log("Player A has registered.");
        }
        else if (player.characterType == DualPlayerMovement.CharacterType.CharacterB)
        {
            playerB = player;
            Debug.Log("Player B has registered.");
        }
    }
    public DualPlayerMovement GetPlayerByType(OwnerType ownerType)
    {
        if (ownerType == OwnerType.CharacterA)
        {
            return playerA;
        }
        else if (ownerType == OwnerType.CharacterB)
        {
            return playerB;
        }
        return null; // Trả về null nếu ownerType là Null hoặc không hợp lệ
    }
}
