// follow.cs

using UnityEngine;

public class follow : MonoBehaviour
{
    private Transform playerTransform;

    [Header("Camera Base Offset (Local Space)")]
    public float localOffsetY = 5f;     // 相机相对于玩家的固定Y轴偏移 (局部)
    public float localOffsetZ = -10f;   // 相机相对于玩家的固定Z轴偏移 (局部)

    [Header("X-Axis Parallax Settings")]
    public float worldAnchorX = 0f;      // 相机X轴试图保持的世界坐标锚点 (例如，赛道中心X=0)
    [Range(0f, 1f)]
    public float parallaxStrengthX = 0.5f; // X轴视差强度:
                                         // 0 = 相机X完全固定在 worldAnchorX (不跟随玩家X)
                                         // 1 = 相机X完全跟随玩家X (就像没有这个脚本一样)
                                         // 0.5 = 相机X移动玩家X移动量的一半 (相对于worldAnchorX)

    [Header("Smoothing")]
    public float smoothSpeed = 5f;       // 平滑移动的速度

    private float currentLocalX;         // 用于平滑的当前相机局部X值

    void Start()
    {
        if (transform.parent == null)
        {
            Debug.LogError("Camera Follow (Parallax): Camera is not a child of the Player. This script expects the camera to be parented.");
            enabled = false; // 禁用脚本
            return;
        }
        playerTransform = transform.parent;

        // 初始化时，计算一次正确的 currentLocalX，以避免第一帧跳动
        // 并设置初始的局部Y和Z
        float initialDesiredWorldX = worldAnchorX + (playerTransform.position.x - worldAnchorX) * parallaxStrengthX;
        currentLocalX = initialDesiredWorldX - playerTransform.position.x; // 转换回局部X
        
        transform.localPosition = new Vector3(currentLocalX, localOffsetY, localOffsetZ);
    }

    void LateUpdate()
    {
        if (playerTransform == null) return;

        // 1. 计算相机期望的世界X坐标
        //    公式: 锚点 + (玩家世界X - 锚点) * 视差强度
        //    这表示相机X位置在“锚点”和“玩家当前X”之间，由parallaxStrengthX决定其位置。
        float desiredCameraWorldX = worldAnchorX + (playerTransform.position.x - worldAnchorX) * parallaxStrengthX;

        // 2. 将期望的世界X坐标转换为相机相对于玩家的局部X坐标
        //    因为: CameraWorldX = PlayerWorldX + CameraLocalX
        //    所以: CameraLocalX = CameraWorldX - PlayerWorldX
        float targetLocalX = desiredCameraWorldX - playerTransform.position.x;

        // 3. 平滑地更新相机的局部X坐标
        currentLocalX = Mathf.Lerp(currentLocalX, targetLocalX, smoothSpeed * Time.deltaTime);

        // 4. 更新相机的局部位置
        //    Y和Z使用固定的局部偏移量，X使用计算出的视差值
        transform.localPosition = new Vector3(currentLocalX, localOffsetY, localOffsetZ);

        // (可选) 如果你还希望相机始终朝向某个方向或者某个点 (比如玩家前方一点的位置)
        // transform.LookAt(...); 
        // 但对于跑酷游戏，通常相机朝向是固定的，或者只是略微向下倾斜。
        // 如果玩家会转向，可能需要更复杂的LookAt逻辑。
    }
}