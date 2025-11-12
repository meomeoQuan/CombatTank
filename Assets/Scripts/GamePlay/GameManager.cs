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

    public bool IsPvsBot { get; private set; }
    public int BotDifficulty { get; private set; }
    public int PlayerSide { get; private set; }
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
        //==============================NHỚ XÓA========================//
        if (DataController.Equipments == null || DataController.Equipments.Count == 0)
            DataController.Initialize();
        //==============================================================//
        countdownText.gameObject.SetActive(false);
        gameWinUI.SetActive(false);
    }
    public void ShowReadyPanel_PvsP()
    {
        gamePlay.SetActive(true);
        isAReady = false;
        isBReady = false;
        UpdateUI();
    }

    public void ShowReadyPanel_PvsB(int playerSide, int difficulty)
    {
        gamePlay.SetActive(true);
        IsPvsBot = true;
        BotDifficulty = difficulty;
        PlayerSide = playerSide;

        if (playerSide == 1)
        {
            isAReady = false; // Player
            isBReady = true;  // Bot ready
        }
        else
        {
            isAReady = true;  // Bot ready
            isBReady = false; // Player
        }

        UpdateUI();
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

        string playerALabel = GameManager.Instance.IsPvsBot && PlayerSide == 2 ? "BOT" : "Player A";
        string playerBLabel = GameManager.Instance.IsPvsBot && PlayerSide == 1 ? "BOT" : "Player B";

        aReadyText.text = $"{playerALabel}: " + (isAReady ? "READY" : "WAITING (PRESS N)\n" + dots);
        bReadyText.text = $"{playerBLabel}: " + (isBReady ? "READY" : "WAITING (PRESS 2)\n" + dots);
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

        gamePlay.SetActive(false);
        countdownText.gameObject.SetActive(false);
        aReadyText.gameObject.SetActive(false);
        bReadyText.gameObject.SetActive(false);



        foreach (var enabler in FindObjectsByType<RequireGameStart>(FindObjectsSortMode.None))
        {
            enabler.EnableComponent();
        }
        if (IsPvsBot)
        {
            // Lấy tất cả các thành phần cần thiết
            var allPlayers = FindObjectsByType<DualPlayerMovement>(FindObjectsSortMode.None);
            var allShooters = FindObjectsByType<ArrowShooter>(FindObjectsSortMode.None);

            foreach (var p in allPlayers)
            {
                bool isBot =
                    (PlayerSide == 1 && p.characterType == DualPlayerMovement.CharacterType.CharacterB) ||
                    (PlayerSide == 2 && p.characterType == DualPlayerMovement.CharacterType.CharacterA);

                if (isBot)
                {
                    // 1. THÊM BOTAI VÀO PLAYER
                    if (!p.gameObject.GetComponent<BotAI>())
                        p.gameObject.AddComponent<BotAI>();

                    var bot = p.GetComponent<BotAI>();

                    // 2. TÌM SHOOTER TƯƠNG ỨNG DỰA TRÊN CharacterType
                    ArrowShooter botShooter = null;
                    foreach (var shooter in allShooters)
                    {
                        // So sánh CharacterType của Player (p) và Shooter (shooter)
                        // DualPlayerMovement.CharacterType và ArrowShooter.CharacterType phải khớp nhau
                        if (shooter.character.ToString() == p.characterType.ToString())
                        {
                            botShooter = shooter;
                            break;
                        }
                    }

                    // 3. GÁN THAM CHIẾU VÀO BOTAI
                    if (botShooter != null)
                    {
                        // Gọi hàm SetShooter mới trong BotAI
                        bot.SetShooter(botShooter);
                    }
                    else
                    {
                        // Rất quan trọng để biết lỗi nếu không tìm thấy nòng súng
                        Debug.LogError($"Không tìm thấy ArrowShooter cho Bot: {p.characterType}");
                    }

                    // 4. Thiết lập độ khó cho Bot
                    switch (BotDifficulty)
                    {
                        case 1: // Dễ
                            bot.decisionInterval = 1.5f;    // Thời gian đổi hướng lâu hơn
                            bot.aimAccuracy = 20f;          // Độ lệch lớn
                            bot.burstCount = 6;             // Ít đạn mỗi loạt
                            bot.burstDelay = 0.12f;         // Độ trễ giữa các viên đạn lâu hơn

                            // shootInterval: Giữ nguyên shootInterval mặc định (0.2f) 
                            // hoặc đặt một giá trị cố định nếu Bot bắn dựa trên góc.
                            // TÔI ĐẶT shootInterval = 0.8f để Bot có thời gian ngắm (logic mới)
                            bot.shootInterval = 0.8f;
                            break;

                        case 2: // Trung bình
                            bot.decisionInterval = 1.2f;
                            bot.aimAccuracy = 15f;
                            bot.burstCount = 8;
                            bot.burstDelay = 0.1f;
                            bot.shootInterval = 0.4f; // Giảm thời gian chờ giữa các loạt
                            break;

                        case 3: // Khó
                            bot.decisionInterval = 0.8f;
                            bot.aimAccuracy = 10f;
                            bot.burstCount = 10;
                            bot.burstDelay = 0.08f;
                            bot.shootInterval = 0.2f; // Bắn liên tục hơn, ít thời gian ngắm hơn
                            break;
                    }
                }
            }
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
        foreach (var shooter in FindObjectsByType<ArrowShooter>(FindObjectsSortMode.None))
        {
            shooter.ReloadImmediate();
        }
        // Reset UI
        gameWinUI.SetActive(false);
        gamePlay.SetActive(true);
        aReadyText.gameObject.SetActive(true);
        bReadyText.gameObject.SetActive(true);

        foreach (var bot in FindObjectsByType<BotAI>(FindObjectsSortMode.None))
        {
            Destroy(bot);
        }
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

}
