using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Animations.Rigging;
using Mirror;

namespace playerSystem
{
    public class ThirdPersonShooter : NetworkBehaviour
    {
        [SerializeField] CinemachineVirtualCamera _aimCamera;
        [SerializeField] private LayerMask _aimLayerMask;
        [SerializeField] private float _aimSensetivity;
        [SerializeField] private float _normalSensetivity;
        [SerializeField] private ThirdPersonController thirdPersonController;
        [SerializeField] private WeaponManager _weapon;

        [SerializeField] private Rig rig;
        [SerializeField]private Transform targetAim;
        [SerializeField] private GameObject _FlashPoint;

        private float _angle = 0;
        private float _angleDist = 0;

        private Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
        private void Start()
        {
            if (isOwned)
            {
                FindObjectOfType<CameraControllerSetter>().Set(thirdPersonController);
                _weapon = GetComponent<WeaponManager>();
                FindObjectOfType<UiManager>().Init(this);
                targetAim.SetParent(null);

                gameObject.SetActive(false);

                gameObject.SetActive(true);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (isOwned)
            {
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    _aimCamera.gameObject.SetActive(true);
                    thirdPersonController.SetSensitivity(_aimSensetivity);
                    thirdPersonController.SetBoolRotateMove(false);
                    Ray ray = Camera.main.ScreenPointToRay(center);
                    if (Physics.Raycast(ray, out RaycastHit hit, 999f, _aimLayerMask)) 
                    {
                        Vector3 worldAimTarget = hit.point;
                        if (isServer)
                        {
                            RpcCheck(1, hit.point);
                        }
                        else
                        {
                            CmdCheck(1, hit.point);
                        }
                        worldAimTarget.y = transform.position.y;
                        //worldAimTarget = worldAimTarget.normalized;
                        _angle = Mathf.Atan2(worldAimTarget.x - transform.position.x, worldAimTarget.z - transform.position.z) * Mathf.Rad2Deg;
                        _angle = _angle < 0 ? 360 + _angle : _angle;
                        if (Mathf.Abs(_angle - transform.eulerAngles.y) >= 90)
                        {
                            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
                            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 5);
                        }
                    }

                    ShootWeapon();
                }
                else
                {
                    _aimCamera.gameObject.SetActive(false);
                    thirdPersonController.SetSensitivity(_normalSensetivity);
                    thirdPersonController.SetBoolRotateMove(true);
                    if (isServer)
                    {
                        RpcCheck(0, Vector3.zero);
                    }
                    else
                    {
                        CmdCheck(0, Vector3.zero);
                    }
                }

                SwapWeapon();
            }
        }
        private void ShootWeapon() {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                _weapon.Shoot();
            }
        }
        private void SwapWeapon()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (isServer)
                {
                    RpcToggleFlashPoint();
                    return;
                }
                else
                    CmdToggleFlashPoint();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                _weapon.Reload();
            }
        }
        [Command]
        private void CmdToggleFlashPoint()
        {
            RpcToggleFlashPoint();
        }
        [ClientRpc]
        private void RpcToggleFlashPoint()
        {
            _FlashPoint.SetActive(!_FlashPoint.active);
        }
        [Command]
        private void CmdCheck(int i, Vector3 pos)
        {
            RpcCheck(i, pos);
        }
        [ClientRpc]
        private void RpcCheck(int i, Vector3 pos)
        {
            if(i == 0)
            {
                rig.weight = i;
                return;
            }
            rig.weight = i;
            targetAim.position = pos;
        }
        public void SetAimCamera(CinemachineVirtualCamera aimCamera)
        {
            _aimCamera = aimCamera;
        }
    }
}
