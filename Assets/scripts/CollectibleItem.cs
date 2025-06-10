using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private Transform playerTransform; // 玩家的Transform
    private bool isFollowing = false;  // 是否正在被吸引
    private float attractionSpeed;     // 飞向玩家的速度

    void Update()
    {
        // 如果isFollowing为true，则让道具飞向玩家
        if (isFollowing && playerTransform != null)
        {
            // 使用MoveTowards平滑地移动到玩家位置
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, attractionSpeed * Time.deltaTime);
        }
    }

    // 玩家脚本将会调用这个公共方法来启动吸引效果
    public void StartFollowingPlayer(Transform target, float speed)
    {
        // 防止一个已经被吸引的道具再次被命令
        if (!isFollowing) 
        {
            isFollowing = true;
            playerTransform = target;
            attractionSpeed = speed;

            // 为了防止道具被吸引后还被其他物理效果影响（如果它有Rigidbody），
            // 我们可以禁用它的碰撞器触发功能，或者直接让它变为isKinematic。
            // 这里我们选择一个更简单的方式，让它飞过来，最终由玩家的OnTriggerEnter处理。
            // 如果你的道具有Rigidbody，可以取消下面这行的注释
            // Rigidbody rb = GetComponent<Rigidbody>();
            // if (rb != null) rb.isKinematic = true;
        }
    }
}