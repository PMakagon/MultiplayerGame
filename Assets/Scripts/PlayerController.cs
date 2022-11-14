using System.Collections;
using Mirror;
using UnityEngine;

namespace MultiplayerGame
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private TextMesh playerName;
        [SerializeField] private TextMesh playerScore;
        private PlayerScoreData _playerScoreData;
        private CharacterController _characterController;
        private InputHandler _inputHandler;
        private Renderer _playerRenderer;

        private bool _isCharging;
        private bool _isInvincible;
        private bool _isColliding;

        private Vector3 _moveDirection;
        private float _moveSpeed;

        private MaterialPropertyBlock _matBlock;
        private readonly int _hitColor = Shader.PropertyToID("_Color");

        public bool IsCharging => _isCharging;
        public bool IsInvincible => _isInvincible;

        public override void OnStartLocalPlayer()
        {
            Initialize();
        }


        private void Update()
        {
            if (isLocalPlayer)
            {
                GetInput();
                if (_isCharging) return;
                CalculateRotation();
                CalculateDirection();
                CalculateSpeed();
                ApplyMovement();
            }
        }

        private void Initialize()
        {
            _characterController = GetComponent<CharacterController>();
            _playerScoreData = GetComponent<PlayerScoreData>();
            _inputHandler = new InputHandler();
            _playerRenderer = GetComponent<Renderer>();
            _matBlock = new MaterialPropertyBlock();
            playerCamera.gameObject.SetActive(true);
            _inputHandler.ChangeCursorState(true);
            SetPlayerName();
        }

        private void GetInput()
        {
            _inputHandler.ReadInput();
            if (_inputHandler.IsChargeClicked && !_isCharging) StartCharge();
        }

        private void CalculateRotation()
        {
            transform.rotation = Quaternion.Euler(0f, _inputHandler.MouseInputVector.x, 0f);
        }

        private void CalculateDirection()
        {
            _moveDirection = transform.right * -_inputHandler.InputVector.x +
                             transform.forward * -_inputHandler.InputVector.y;
        }

        private void CalculateSpeed()
        {
            _moveSpeed = _inputHandler.HasInput ? 10f : 0f;
        }

        private void ApplyMovement()
        {
            _characterController.Move(_moveDirection * _moveSpeed * Time.deltaTime);
        }

        private void StartCharge()
        {
            StartCoroutine(nameof(Charge));
        }

        [Client]
        private IEnumerator Charge()
        {
            if (_moveDirection == Vector3.zero) yield break;
            _isCharging = true;
            _inputHandler.isInputLocked = true;
            Vector3 chargeStartPosition = transform.position;
            _moveSpeed = 0f;
            while (playerConfig.ChargeDistance >= (chargeStartPosition - transform.position).magnitude)
            {
                _moveSpeed += playerConfig.ChargeVelocity;
                ApplyMovement();
                yield return null;
                if (_isColliding || _moveSpeed >= 150f)
                {
                    StopCharge();
                    yield break;
                }
            }

            StopCharge();
        }

        private void StopCharge()
        {
            _moveSpeed = 0f;
            _inputHandler.isInputLocked = false;
            _isCharging = false;
        }

        [Server]
        private void ChangeColor()
        {
            _matBlock.SetColor(_hitColor, _isInvincible ? Color.red : Color.white);
            _playerRenderer.SetPropertyBlock(_matBlock);
        }
        
        
        private IEnumerator StartHitCooldown()
        {
            yield return new WaitForSecondsRealtime(playerConfig.HitCoolDown);
            _isInvincible = false;
            ChangeColor();
        }

        [ClientRpc]
        private void IncreaseScore()
        {
            ++_playerScoreData.PlayerScore;
            UpdateScore();
        }

        [Server]
        private void OnTriggerEnter(Collider other)
        {
            if (_isColliding) return;
            if (_isInvincible) return;
            _isColliding = true;
            if (other.CompareTag("Player"))
            {
                if (!TryGetComponent(out PlayerController otherPlayer)) return;
                if (_isCharging && !otherPlayer.IsInvincible)
                {
                    Debug.Log("HIT");
                    IncreaseScore();
                    _isColliding = false;
                    return;
                }

                if (otherPlayer.IsCharging)
                {
                    _isInvincible = true;
                    ChangeColor();
                    StartCoroutine(nameof(StartHitCooldown));
                }
            }

            _isColliding = false;
        }

        [ClientRpc]
        public void SetPlayerName()
        {
            playerName.text = "Player " + _playerScoreData.PlayerIndex;
        }

        [ClientRpc]
        private void UpdateScore()
        {
            playerScore.text = _playerScoreData.PlayerScore.ToString();
        }
    }
}