// File: ArrowShooter.cs

using Assets.Scripts.DataController;
using Assets.Scripts.Models.Characters;
using Assets.Scripts.Models.Equipments;
using System.Linq;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class ArrowShooter : MonoBehaviour
{
    public enum CharacterType { CharacterA, CharacterB }
    [Header("Nhân vật bắn")]
    public CharacterType character;

    [Header("Cài đặt")]
    public Transform firePoint;
    private float bulletSpeed;
    private string bulletTypeTag;

    // 🔥 CÁC BIẾN CHO CƠ CHẾ ĐẠN/RELOAD
    public GameObject bulletPrefab;
    private int maxAmmo;
    private float reloadRate;
    private int currentAmmo;
    public Image bulletIcon;
    private bool isReloading = false;
    // 🔥 KẾT THÚC CÁC BIẾN MỚI

    // 🔥 BIẾN UI
    [Header("Hiển thị UI")]
    public TMP_Text ammoAndReloadText;
    // 🔥 KẾT THÚC BIẾN UI

    [Header("Phím Bắn & Nạp Đạn")]
    public KeyCode shootKey = KeyCode.B;
    public KeyCode reloadKey = KeyCode.M; // 🔥 ĐÃ THÊM: Biến cho phím Nạp đạn

    void Start()
    {
        // ... (Code lấy nhân vật và vũ khí giữ nguyên) ...
        Character selectedChar = (character == CharacterType.CharacterA)
             ? DataController.Characters[0] // CharacterA
             : DataController.Characters[1]; // CharacterB

        var weapon = selectedChar.GetEquippedItems().OfType<Weapon>().FirstOrDefault();

        if (weapon != null)
        {
            bulletSpeed = weapon.BulletSpeed;
            bulletTypeTag = weapon.Id + (character == CharacterType.CharacterA ? "1" : "2");

            // 🔥 Gán các giá trị mới
            maxAmmo = weapon.MaxAmmo;
            reloadRate = weapon.ReloadRate;
            currentAmmo = maxAmmo;

            // 🔥 Load Prefab đạn từ Resources
            if (bulletPrefab == null)
            {
                bulletPrefab = Resources.Load<GameObject>("Shell/" + bulletTypeTag);
            }
        }
        else
        {
            // ... (Code xử lý thiếu Weapon giữ nguyên) ...
            Debug.LogWarning($"{character} chưa có Weapon!");
            bulletSpeed = 5f;
            bulletTypeTag = "DefaultShell";
            // 🔥 Gán giá trị mặc định khi thiếu Weapon
            maxAmmo = 10;
            reloadRate = 2f;
            currentAmmo = maxAmmo;
        }

        // 🔥 Xử lý hiển thị Icon đạn
        if (bulletIcon != null && bulletPrefab != null)
        {
            SpriteRenderer sr = bulletPrefab.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                bulletIcon.sprite = sr.sprite;
                bulletIcon.enabled = true;
            }
            else
            {
                Debug.LogWarning($"Prefab '{bulletPrefab.name}' không có SpriteRenderer!");
            }
        }

        // 🔥 HIỂN THỊ ĐẠN LẦN ĐẦU KHI VÀO GAME
        UpdateAmmoUI();
    }

    void Update()
    {
        if (!GameManager.Instance || !GameManager.Instance.IsGameRunning)
            return;

        // 🔥 Chặn bắn khi đang reload
        if (Input.GetKeyDown(shootKey) && !isReloading)
        {
            TryShoot();
        }

        // 🔥 ĐÃ THÊM: Kiểm tra phím Nạp đạn (reloadKey)
        if (Input.GetKeyDown(reloadKey) && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }

        // 🔥 NẾU ĐANG RELOAD VÀ ĐÃ HẾT ĐẠN, cho phép nhấn phím bắn để xem hiệu ứng "Click" (TÙY CHỌN)
        else if (Input.GetKeyDown(shootKey) && isReloading && currentAmmo == 0)
        {
            // TODO: Thêm âm thanh "click" hết đạn tại đây
        }
    }

    // 🔥 HÀM MỚI: Kiểm tra đạn và bắt đầu bắn/reload
    void TryShoot()
    {
        if (currentAmmo > 0)
        {
            Shoot();
            currentAmmo--;
            UpdateAmmoUI(); // 🔥 CẬP NHẬT UI SAU KHI BẮN

            // Nếu đạn về 0 sau khi bắn, bắt đầu reload tự động
            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
            }
        }
        // else: Đã hết đạn (currentAmmo <= 0)
        else if (!isReloading)
        {
            // Nếu hết đạn và chưa nạp, tự động nạp khi cố gắng bắn
            StartCoroutine(Reload());
        }
    }


    // 🔥 THAY THẾ TOÀN BỘ HÀM Shoot() (Giữ nguyên)
    void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError($"Không có Prefab đạn cho tag: {bulletTypeTag}");
            return;
        }

        // 1. SỬ DỤNG INSTANTIATE THAY CHO POOLING
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 2. Setup viên đạn
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.myBulletTag = bulletTypeTag;
            bullet.owner = (OwnerType)character;
        }


        // 3. Tính toán và áp dụng vận tốc
        bulletGO.transform.position = firePoint.position;
        bulletGO.transform.rotation = Quaternion.identity;

        float angleZ = transform.localEulerAngles.z;
        if (angleZ > 180f) angleZ -= 360f;

        float rad = (angleZ + 90f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

        Rigidbody2D rb = bulletGO.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }

        // 4. Xoay sprite đạn theo hướng bay
        float angleDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bulletGO.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg - 90f);
    }
    // 🔥 KẾT THÚC HÀM Shoot()


    // 🔥 HÀM MỚI: Coroutine Reload
    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log($"[{character}] Bắt đầu Reload... ({reloadRate}s)");

        // TODO: Phát âm thanh nạp đạn/animation nạp đạn tại đây

        float timer = 0f;
        while (timer < reloadRate)
        {
            timer += Time.deltaTime;
            // 🔥 Cập nhật UI hiển thị thời gian nạp đạn
            if (ammoAndReloadText != null)
            {
                // Làm tròn lên số nguyên gần nhất
                ammoAndReloadText.text = $"<color=#FF0000>Reload {Mathf.Ceil(reloadRate - timer):0}s</color>";
            }
            yield return null;
        }

        // Nạp đạn đầy đủ
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log($"[{character}] Reload hoàn tất. Đạn: {currentAmmo}/{maxAmmo}");

        // 🔥 Cập nhật UI sau khi reload, chuyển về hiển thị số đạn
        UpdateAmmoUI();
    }

    // 🔥 HÀM MỚI: Cập nhật hiển thị số đạn
    private void UpdateAmmoUI()
    {
        if (ammoAndReloadText != null)
        {
            // Hiển thị số đạn
            ammoAndReloadText.text = $"{currentAmmo}/{maxAmmo}";
        }
    }
    public void ReloadImmediate()
    {
        StopAllCoroutines();       // Ngăn coroutine reload cũ (nếu có)
        currentAmmo = maxAmmo;     // Nạp đầy
        isReloading = false;       // Tắt trạng thái reload
        UpdateAmmoUI();            // Cập nhật UI
        Debug.Log($"[{character}] Reset Ammo: {currentAmmo}/{maxAmmo}");
    }
    public void ShootFromBot()
    {
        if (!isReloading && currentAmmo > 0)
        {
            Shoot();
            //Debug.Log("Bot bắn 1 mũi tên");
            currentAmmo--;
            UpdateAmmoUI();
        }
        else if (!isReloading && currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }
    }

}