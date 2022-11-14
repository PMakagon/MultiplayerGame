using UnityEngine;

namespace MultiplayerGame
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [SerializeField] private float playerSpeed = 10f;
        [SerializeField] private int chargeDistance = 5;
        [SerializeField] [Range(0.1f,0.9f)] private float chargeVelocity = 0.5f;
        [SerializeField] private int hitCoolDown = 3;

        public float PlayerSpeed => playerSpeed;
        public int ChargeDistance => chargeDistance;

        public float ChargeVelocity => chargeVelocity;
        public int HitCoolDown => hitCoolDown;
    }
}