using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;
using playerSystem;

public class CameraControllerSetter : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _aimCamera;
    [SerializeField] CinemachineVirtualCamera _defaultCamera;

    public void Set(ThirdPersonController thirdPersonController)
    {
        /*List<ThirdPersonController> players = ((PLNetworkManager)NetworkManager.singleton).playerHeroes;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].hasAuthority)
            {
                _aimCamera.Follow = players[i].CinemachineCameraTarget.transform;
                _defaultCamera.Follow = players[i].CinemachineCameraTarget.transform;
                players[i].GetComponent<ThirdPersonShooter>().SetAimCamera(_aimCamera);
                Debug.Log("hello1122");
            }
            else
            {
                Debug.Log("wro1122");
            }
        }*/
        _aimCamera.Follow = thirdPersonController.CinemachineCameraTarget.transform;
        _defaultCamera.Follow = thirdPersonController.CinemachineCameraTarget.transform;
        thirdPersonController.GetComponent<ThirdPersonShooter>().SetAimCamera(_aimCamera);
        Debug.Log("hello1122");

    }
}
