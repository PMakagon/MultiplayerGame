using System;
using UnityEngine;

namespace MultiplayerGame
{
    public class InputHandler
    {
        private Vector2 _inputVector;
        private Vector3 _mouseInputVector;
        private bool _isInputLocked;
        private bool _isPaused;
        public static event Action OnPausePressed = delegate { };
        public static event Action OnPauseUnPressed = delegate { };

        public bool HasInput => _inputVector != Vector2.zero;
        public Vector2 InputVector => _inputVector;
        public Vector3 MouseInputVector => _mouseInputVector;
        public bool IsChargeClicked { get; private set; }

        public bool isInputLocked
        {
            get => _isInputLocked;
            set => _isInputLocked = value;
        }

        private void SetInputPauseState()
        {
            _isPaused = !_isPaused;
            isInputLocked = _isPaused;
            ChangeCursorState(!_isPaused);
            if (_isPaused)
            {
                OnPausePressed?.Invoke();
            }
            else
            {
                OnPauseUnPressed?.Invoke();
            }
        }

        public void ChangeCursorState(bool isLocked)
        {
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.Confined;
            Cursor.visible = !isLocked;
        }

        public void ReadInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) SetInputPauseState();
            if (_isInputLocked) return;
            _inputVector.x = Input.GetAxisRaw("Horizontal");
            _inputVector.y = Input.GetAxisRaw("Vertical");
            _mouseInputVector.x += Input.GetAxis("Mouse X");
            _mouseInputVector.y -= Input.GetAxis("Mouse Y");
            IsChargeClicked = Input.GetKeyDown(KeyCode.Mouse0);
        }
    }
}