using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 6.0f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        [Header("Crouch Settings")]
        public float CrouchHeight = 1f;
        public float StandingHeight = 2f;
        public float CrouchSpeed = 2.0f;

        [Header("Stamina Settings")]
        public float MaxStamina = 5.0f;
        public float StaminaDecreaseRate = 1.0f;
        public float StaminaRecoveryRate = 1.0f;

        [Header("Camara")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Image blackScreen;
        [SerializeField] private GameObject dieText;

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // crouch
        private bool _isCrouching = false;
        private float _cameraHeightVelocity = 0f; // camera speed for SmoothDamp
        public float CameraHeightSmoothTime = 0.2f; // smoothing time for camera transition

        // stamina
        private float _currentStamina;
        private bool _canSprint = true;

        // dead
        private bool isDead = false;


#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        public float CurrentStamina { get => _currentStamina; set => _currentStamina = value; }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            _currentStamina = MaxStamina;
        }

        private void Update()
        {
            if (isDead) return;

            JumpAndGravity();
            GroundedCheck();
            HandleStamina();
            Move();
            HandleCrouch();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            // do not move the camera if the game is paused
            if (Time.timeScale == 0f) return;

            // if there is an input
            if (_input.look.sqrMagnitude >= _threshold)
            {
                //Don't multiply mouse input by Time.deltaTime
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Update Cinemachine camera target pitch
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void HandleStamina()
        {
            if (_input.sprint && _canSprint)
            {
                _currentStamina -= StaminaDecreaseRate * Time.deltaTime;
                if (_currentStamina <= 0)
                {
                    _currentStamina = 0;
                    _canSprint = false;
                }
            }
            else
            {
                _currentStamina += StaminaRecoveryRate * Time.deltaTime;
                if (_currentStamina >= MaxStamina)
                {
                    _currentStamina = MaxStamina;
                    _canSprint = true;
                }
            }
        }

        private void Move()
        {
            // disable sprint while crouching
            if (_isCrouching || !_canSprint)
            {
                _input.sprint = false; // prevent sprint from being activated
            }

            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                // move
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump (only if we are not crouching)
                if (_input.jump && _jumpTimeoutDelta <= 0.0f && !_isCrouching)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void HandleCrouch()
        {
            if (_input.crouch)
            {
                if (!_isCrouching)
                {
                    _controller.height = CrouchHeight;
                    MoveSpeed = CrouchSpeed;
                    _isCrouching = true;
                }
            }
            else
            {
                if (_isCrouching)
                {
                    Vector3 raycastOrigin = transform.position + Vector3.up * (CrouchHeight - 0.1f);
                    RaycastHit hit;

                    // check if there is room to stande
                    if (!Physics.Raycast(raycastOrigin, Vector3.up, out hit, StandingHeight - CrouchHeight))
                    {
                        _controller.height = Mathf.Lerp(_controller.height, StandingHeight, Time.deltaTime * 5f);
                        MoveSpeed = 4.0f; // restore normal speed
                        _isCrouching = false;
                    }
                }
            }
            // smooth the camera transition too
            float targetHeight = _isCrouching ? CrouchHeight : StandingHeight;
            float currentHeight = Mathf.SmoothDamp(_controller.height, targetHeight, ref _cameraHeightVelocity, CameraHeightSmoothTime);
            _controller.height = currentHeight;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        public void Die(Transform enemy)
        {
            isDead = true;
            _controller.enabled = false;
            _input.enabled = false; ;
            StartCoroutine(DeathCinematic(enemy));
        }

        private IEnumerator DeathCinematic(Transform enemy)
        {
            blackScreen.gameObject.SetActive(true);
            dieText.SetActive(true);

            CanvasGroup dieTextCanvasGroup = dieText.GetComponent<CanvasGroup>();
            if (dieTextCanvasGroup == null)
            {
                dieTextCanvasGroup = dieText.AddComponent<CanvasGroup>();
            }
            dieTextCanvasGroup.alpha = 0;

            float fadeDuration = 2f;
            float fadeTime = 0f;
            Color startColor = blackScreen.color;
            blackScreen.color = new Color(startColor.r, startColor.g, startColor.b, 0);

            while (fadeTime < fadeDuration)
            {
                fadeTime += Time.deltaTime;
                float alphaValue = Mathf.Clamp01(fadeTime / fadeDuration);

                blackScreen.color = new Color(startColor.r, startColor.g, startColor.b, alphaValue);
                dieTextCanvasGroup.alpha = alphaValue;
                yield return null;
            }

            Vector3 enemyPosition = enemy.position;
            Quaternion targetRotation = Quaternion.LookRotation(enemyPosition - mainCamera.transform.position);
            float rotationSpeed = 2f;

            while (Quaternion.Angle(mainCamera.transform.rotation, targetRotation) > 0.1f)
            {
                mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}