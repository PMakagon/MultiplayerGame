using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace MultiplayerGame
{
    public class MyNetworkRoomManager : NetworkRoomManager
    {
        [SerializeField] private MatchConfig matchConfig;
        private bool _isStartBtnActive;
        private bool _isGameOn;

        public static List<PlayerScoreData> PlayerList { get; private set; }
        public static PlayerScoreData Winner { get; set; }

        public static event Action OnPlayerVictory = delegate { };

        public static void AddPlayerToPlayerList(PlayerScoreData player)
        {
            PlayerList.Add(player);
            Debug.Log("PLAYER " + player.PlayerIndex + " ADDED");
        }

        public static void ResetData()
        {
            PlayerList = new List<PlayerScoreData>();
            Winner = null;
        }

        public static void ResetScore()
        {
            Debug.Log("RESET SCORE");
            Winner = null;
            foreach (var playerScoreData in PlayerList)
            {
                playerScoreData.PlayerScore = 0;
            }
        }

        public override void OnStartServer()
        {
            ResetData();
            base.OnStartServer();
        }

        public override void OnStopServer()
        {
            ResetData();
            base.OnStopServer();
        }

        public override void OnRoomClientSceneChanged()
        {
            Debug.Log("OnRoomClientSceneChanged");
            
            base.OnRoomClientSceneChanged();
        }

        public override void OnRoomServerSceneChanged(string sceneName)
        {
            _isGameOn = sceneName == GameplayScene;
        }

        public override void Update()
        {
            
            if (_isGameOn)
            {
                CheckForWinner();
                if (Winner != null)
                {
                    _isGameOn = false;
                    OnPlayerVictory?.Invoke();
                    RestartMatch();
                }
            }

            base.Update();
        }

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer,
            GameObject gamePlayer)
        {
            Debug.Log("PLAYER" + conn.connectionId + " LOADED");
            PlayerScoreData playerScoreData = gamePlayer.GetComponent<PlayerScoreData>();
            SetPlayerScoreData(playerScoreData, roomPlayer);
            AddPlayerToPlayerList(playerScoreData);
            // PlayerController playerController = gamePlayer.GetComponent<PlayerController>();
            return true;
        }

        private void SetPlayerScoreData(PlayerScoreData playerScoreData, GameObject roomPlayer)
        {
            playerScoreData.PlayerIndex = roomPlayer.GetComponent<MyNetworkRoomPlayer>().index + 1;
            playerScoreData.PlayerScore = 0;
        }

        private IEnumerator StartMatchRestartCountdown()
        {
            Debug.Log("MATCH RESTART IN " + matchConfig.MatchRestartTime);
            yield return new WaitForSecondsRealtime(matchConfig.MatchRestartTime);
            ResetScore();
            ServerChangeScene(GameplayScene);
        }


        private void RestartMatch()
        {
            StartCoroutine(StartMatchRestartCountdown());
        }

        private void CheckForWinner()
        {
            foreach (var playerScoreData in PlayerList)
            {
                if (playerScoreData.PlayerScore >= matchConfig.HitsToWin)
                {
                    Winner = playerScoreData;
                }
            }
        }

        public override void OnRoomServerPlayersReady()
        {
#if UNITY_SERVER
            base.OnRoomServerPlayersReady();
#else
            _isStartBtnActive = true;
#endif
        }

        public override void OnGUI()
        {
            base.OnGUI();
            if (allPlayersReady && _isStartBtnActive && GUI.Button(new Rect(150, 300, 120, 20), "START GAME"))
            {
                _isStartBtnActive = false;

                ServerChangeScene(GameplayScene);
            }
        }
    }
}