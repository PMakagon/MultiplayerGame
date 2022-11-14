using Mirror;
using UnityEngine;

namespace MultiplayerGame
{
    public class PlayerScoreData : NetworkBehaviour
    {
        [SyncVar] private int _playerIndex;
        [SyncVar] private int _playerScore;

        public int PlayerIndex
        {
            get => _playerIndex;
            set => _playerIndex = value;
        }

        public int PlayerScore
        {
            get => _playerScore;
            set => _playerScore = value;
        }

        void OnGUI()
        {
            GUI.Box(new Rect(10f + (_playerIndex * 110), 10f, 100f, 25f), $"P{_playerIndex}: {_playerScore:0}");
        }
    }
}