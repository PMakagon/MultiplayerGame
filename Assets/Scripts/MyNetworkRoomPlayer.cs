using Mirror;
using UnityEngine;

namespace MultiplayerGame
{
    public class MyNetworkRoomPlayer : NetworkRoomPlayer
    {
        public override void OnStartClient()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
