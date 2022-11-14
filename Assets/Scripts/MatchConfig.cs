using UnityEngine;

namespace MultiplayerGame
{
    [CreateAssetMenu(fileName = "MatchConfig", menuName = "MatchConfig")]
    public class MatchConfig : ScriptableObject
    {
        [SerializeField] private int matchRestartTime = 5;
        [SerializeField] private int hitsToWin = 3;
        public int MatchRestartTime => matchRestartTime;
        public int HitsToWin => hitsToWin;
        
    }
}