using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerGame
{
    public class GameMenu : MonoBehaviour
    {
        [SerializeField] private GameObject menuScreen;
        [SerializeField] private Button exitBtn;

        private void Start()
        {
            exitBtn.onClick.AddListener(OnExitBtnClicked);
            InputHandler.OnPausePressed += OnPausePressed;
            InputHandler.OnPauseUnPressed += ContinueGame;
        }

        private void OnDestroy()
        {
            InputHandler.OnPausePressed -= OnPausePressed;
            InputHandler.OnPauseUnPressed -= ContinueGame;
        }

        private void SetPauseScreenActive(bool state)
        {
            menuScreen.SetActive(state);
        }

        private void OnPausePressed()
        {
            SetPauseScreenActive(true);
        }

        private void ContinueGame()
        {
            SetPauseScreenActive(false);
        }

        private void OnExitBtnClicked()
        {
            Application.Quit();
        }
    }
}