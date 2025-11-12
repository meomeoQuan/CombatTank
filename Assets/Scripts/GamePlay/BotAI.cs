// BotAI.cs (Phiên bản đã cải tiến)

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DualPlayerMovement))]
public class BotAI : MonoBehaviour
{
    private DualPlayerMovement player;
    private ArrowShooter shooter;
    private Transform target;
    private Vector2 moveDir;

    private float decisionTimer;
    private float shootTimer;

    [Header("Cài đặt Bot")]
    // decisionInterval, shootInterval, aimAccuracy sẽ được GameManager gán giá trị mới
    public float decisionInterval = 1.2f; // Thời gian đổi hướng
    public float shootInterval = 0.4f;    // Thời gian giữa mỗi lần bắn (khoảng nghỉ giữa các loạt)
    public float aimAccuracy = 8f;        // Sai số khi ngắm (ĐỘ LỆCH TỐI ĐA)
    public float boundaryPadding = 0.3f;  // Tránh chạm biên

    [Header("Bắn liên tiếp")]
    public bool aggressiveMode = true;   // Bắn liên tục
    public int burstCount = 8;           // Số mũi tên mỗi đợt
    public float burstDelay = 0.1f;      // Khoảng nghỉ giữa mũi tên

    private Coroutine burstCoroutine;

    public void SetShooter(ArrowShooter s)
    {
        shooter = s;
    }

    void Start()
    {
        player = GetComponent<DualPlayerMovement>();

        // Tìm đối thủ
        foreach (var p in FindObjectsByType<DualPlayerMovement>(FindObjectsSortMode.None))
        {
            if (p.characterType != player.characterType)
            {
                target = p.transform;
                break;
            }
        }

        moveDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f)).normalized;

        decisionTimer = decisionInterval;
        // Đảm bảo min < max, dùng shootInterval (được gán từ GameManager)
        shootTimer = Random.Range(0.1f, shootInterval);
    }

    void Update()
    {
        if (!GameManager.Instance || !GameManager.Instance.IsGameRunning || shooter == null || target == null)
            return;

        HandleMovementDecision();

        // 1. KIỂM TRA ĐỘ LỆCH GÓC TRƯỚC
        if (IsAimedCorrectly())
        {
            shootTimer -= Time.deltaTime;

            if (shootTimer <= 0)
            {
                shootTimer = shootInterval;
                if (aggressiveMode)
                {
                    if (burstCoroutine == null)
                        burstCoroutine = StartCoroutine(BurstFire());
                }
                else
                {
                    shooter.ShootFromBot();
                }
            }
        }
        else
        {
            // Nếu không ngắm đúng, reset shotTimer để Bot chờ ngắm lại
            // Điều này buộc Bot phải ngắm trúng trước khi bắn
            shootTimer = Mathf.Max(shootTimer, 0.1f); // Giữ shotTimer ở mức nhỏ nếu đang chờ
        }
    }

    private void HandleMovementDecision()
    {
        // ... (Giữ nguyên logic HandleMovementDecision)
        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0)
        {
            decisionTimer = decisionInterval;
            moveDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f)).normalized;
        }

        if (player.boundaryCollider != null)
        {
            Bounds bounds = player.boundaryCollider.bounds;
            Vector2 pos = transform.position;

            if (pos.x <= bounds.min.x + boundaryPadding || pos.x >= bounds.max.x - boundaryPadding)
                moveDir.x *= -1;

            if (pos.y <= bounds.min.y + boundaryPadding || pos.y >= bounds.max.y - boundaryPadding)
                moveDir.y *= -1;
        }

        player.SetAIMovement(moveDir);
    }

    // HÀM MỚI: TÍNH TOÁN ĐỘ LỆCH VÀ KIỂM TRA ĐIỀU KIỆN BẮN
    private bool IsAimedCorrectly()
    {
        // Vị trí nòng súng
        Vector2 shooterPos = shooter.transform.position;
        // Vị trí mục tiêu
        Vector2 targetPos = target.position;

        // 1. Góc lý tưởng: Góc từ nòng súng đến mục tiêu
        Vector2 idealDir = targetPos - shooterPos;
        float idealAngle = Mathf.Atan2(idealDir.y, idealDir.x) * Mathf.Rad2Deg;

        // 2. Góc hiện tại: Góc quay Z của nòng súng (Arrow2)
        // Unity sử dụng Quaternion.Euler, Rotation Z = 0 là hướng X+
        // Thông thường, nòng súng sẽ xoay sao cho Vector.up của nó hướng về mục tiêu.
        // Nếu nòng súng của bạn được xoay với angle - 90, thì góc quay Z chính là angle - 90.
        float currentAngle = shooter.transform.rotation.eulerAngles.z;

        // 3. Chuẩn hóa góc: Chuyển đổi góc quay từ 0 đến 360 sang -180 đến 180 để tính toán độ lệch ngắn nhất
        // Thường xuyên gặp vấn đề này trong Unity
        float angleDifference = Mathf.DeltaAngle(currentAngle, idealAngle - 90);

        // Ví dụ: Nếu aimAccuracy là 5f, Bot chỉ bắn khi độ lệch < 5 độ
        if (Mathf.Abs(angleDifference) <= aimAccuracy)
        {
            return true;
        }

        return false;
    }

    private IEnumerator BurstFire()
    {
        for (int i = 0; i < burstCount; i++)
        {
            shooter.ShootFromBot();
            yield return new WaitForSeconds(burstDelay);
        }
        burstCoroutine = null;
    }
}