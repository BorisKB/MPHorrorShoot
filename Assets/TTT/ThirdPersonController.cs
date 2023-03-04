using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;

namespace playerSystem
{
    public class ThirdPersonController : NetworkBehaviour
    {
        [SerializeField] private AudioSource _shagAudio;

        [SerializeField] private CharacterController _controller;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float SpeedChangeRate = 10.0f;
        [SerializeField] private float RotationSmoothTime = 0.12f;
        [SerializeField] private float Gravity = -9f;
        private float _speed;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;

        private bool _rotateOnMove = true;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        [SerializeField] private float sensetivity = 1f;
        [SerializeField] private float BottomClamp;
        [SerializeField] private float TopClamp;
        [SerializeField] private float CameraAngleOverride = 0.0f;
        public GameObject CinemachineCameraTarget;

        [SerializeField] private NetworkAnimator _animator;
        private float _animationBlend;

        private bool LockCameraPosition = false;
        private const float _threshold = 0.01f;
        private GameObject _mainCamera;

        private void Start()
        {
            if (hasAuthority)
            {
                /*List<PlayerNetwork> players = ((PLNetworkManager)NetworkManager.singleton).players;
                foreach (PlayerNetwork p in players)
                {
                    if (p.hasAuthority)
                    {
                        CmdChangeMatColor(p);
                    }
                }*/
                _mainCamera = Camera.main.gameObject;
                _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            }
        }
        private void Update()
        {
            if (hasAuthority)
            {
                Move();
            }
        }

        private void LateUpdate()
        {
            if (hasAuthority)
            {
                CameraRotation();
            }
        }
        private void Move()
        {
            _verticalVelocity += Gravity * Time.deltaTime;

            float targetSpeed = _moveSpeed;
            Vector2 vel = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (vel == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = vel.magnitude;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                    currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(vel.x, 0.0f, vel.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (vel != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                if (_rotateOnMove)
                {
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, -_verticalVelocity, 0.0f) * Time.deltaTime);

            _animator.animator.SetFloat("Run", inputMagnitude);
        }
        private void CameraRotation()
        {
            Vector2 Axis = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));
            if (Axis.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                

                _cinemachineTargetYaw += Axis.x * sensetivity;
                _cinemachineTargetPitch += Axis.y  * sensetivity;
            }
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                    _cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        public void SetSensitivity(float newSensetivity)
        {
            sensetivity = newSensetivity;
        }
        public void SetBoolRotateMove(bool newState)
        {
            _rotateOnMove = newState;
        }

        public void PlayAudioShag()
        {
            _shagAudio.pitch = Random.Range(0.9f, 1.05f);
            _shagAudio.Play();
        }
        [Command]
        private void CmdChangeMatColor(PlayerNetwork playerNetwork)
        {
            ChangeMatColor(playerNetwork);
        }
        [ClientRpc]
        private void ChangeMatColor(PlayerNetwork playerNetwork)
        {
            GetComponent<Renderer>().material.SetColor("_BaseColor", playerNetwork.GetTeamColor());
        }
    }
}
