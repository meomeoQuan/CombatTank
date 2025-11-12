// Tên file: PlayerData.cs
// Thư mục: Assets/Scripts/DataController/
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DataController
{
    public class PlayerData
    {
        public long CurrentCoint { get; private set; }

        /// <summary>
        /// (MỚI) Danh sách các ID của Drone đã được "Mở Khóa" (Unlock)
        /// Dùng HashSet để truy cập cực nhanh
        /// </summary>
        public HashSet<string> UnlockedDroneBlueprints { get; private set; }

        /// <summary>
        /// (CŨ) Danh sách SỐ LƯỢNG drone đang sở hữu.
        /// (Key sẽ không tồn tại nếu số lượng là 0)
        /// </summary>
        public Dictionary<string, int> OwnedDrones { get; private set; }

        public PlayerData(long startingCoint)
        {
            CurrentCoint = startingCoint;
            // Khởi tạo cả 2 danh sách
            UnlockedDroneBlueprints = new HashSet<string>();
            OwnedDrones = new Dictionary<string, int>();
        }

        // --- QUẢN LÝ TIỀN (như cũ) ---
        public bool CanAfford(long cost) => CurrentCoint >= cost;
        public void AddCoint(long amount) => CurrentCoint += amount;
        public bool SpendCoint(long cost)
        {
            if (CanAfford(cost))
            {
                CurrentCoint -= cost;
                return true;
            }
            return false;
        }

        // --- LOGIC MỚI QUẢN LÝ DRONE ---

        /// <summary>
        /// 1. Kiểm tra xem drone đã được MỞ KHÓA chưa?
        /// </summary>
        public bool IsDroneUnlocked(string droneID)
        {
            return UnlockedDroneBlueprints.Contains(droneID);
        }

        /// <summary>
        /// 2. Lấy SỐ LƯỢNG drone đang sở hữu
        /// </summary>
        public int GetDroneQuantity(string droneID)
        {
            OwnedDrones.TryGetValue(droneID, out int quantity);
            return quantity; // Tự động trả về 0 nếu không tìm thấy
        }

        /// <summary>
        /// 3. Hành động MỞ KHÓA (Unlock)
        /// </summary>
        public void UnlockDroneBlueprint(string droneID)
        {
            if (!IsDroneUnlocked(droneID))
            {
                UnlockedDroneBlueprints.Add(droneID);
                Debug.Log($"[PlayerData] Đã mở khóa Blueprint cho: {droneID}");
            }
        }

        /// <summary>
        /// 4. Hành động THÊM SỐ LƯỢNG (Purchase)
        /// </summary>
        public void AddDrone(string droneID, int amount = 1)
        {
            if (OwnedDrones.TryGetValue(droneID, out int currentQty))
            {
                // Nếu đã có, cộng thêm
                OwnedDrones[droneID] = currentQty + amount;
            }
            else
            {
                // Nếu là con đầu tiên, thêm mới
                OwnedDrones.Add(droneID, amount);
            }
            Debug.Log($"[PlayerData] Đã thêm {amount}x {droneID}. Tổng số lượng: {OwnedDrones[droneID]}");
        }
    }
}