using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerGame
{
    public class MatchScoreTable : NetworkBehaviour
    {
        [SerializeField] private MatchConfig matchConfig;
        [SerializeField] private GameObject winnerPanel;
        [SerializeField] private Text winnerName;
        [SerializeField] private Text infoMessage;

        private void Start()
        {
            MyNetworkRoomManager.OnPlayerVictory += ShowWinner;
        }

        private void OnDestroy()
        {
            MyNetworkRoomManager.OnPlayerVictory -= ShowWinner;
        }
        
        [ClientRpc]
        public void ShowWinner()
        {
            winnerPanel.SetActive(true);
            // winnerName.text = "PLAYER " + matchData.Winner.PlayerIndex + " WIN";
            infoMessage.text = "Match restarts in " + matchConfig.MatchRestartTime + " seconds";
        }
    }
}