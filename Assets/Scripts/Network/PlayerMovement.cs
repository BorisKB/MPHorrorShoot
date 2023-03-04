using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{

    [SerializeField] private float _speed;
    [SerializeField] private LayerMask _aimLayerMask;
    // Start is called before the first frame update
    void Start()
    {
        if (hasAuthority)
        {

            List<PlayerNetwork> players = ((PLNetworkManager)NetworkManager.singleton).players;
            foreach (PlayerNetwork p in players)
            {
                if (p.hasAuthority)
                {
                    CmdChangeMatColor(p);
                    Debug.Log("Heelooo");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority)
        {
            Movement();
            AimTowardMouse();
        }
    }

    [Command]
    private void CmdChangeMatColor(PlayerNetwork playerNetwork)
    {
        ChangeMatColor(playerNetwork);
    }
    [ClientRpc]
    private void ChangeMatColor(PlayerNetwork playerNetwork)
    {
        GetComponent<Renderer>().material.SetColor("_BaseColor",playerNetwork.GetTeamColor());
        Debug.Log("Heelooo111");

    }
    private void Movement()
    {
        if (transform.position.y < 0) { transform.position += Vector3.up * 0.05f; }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        if (movement.magnitude > 0)
        {
            transform.Translate(movement.normalized * (_speed * Time.deltaTime), Space.World);
        }
    }
    private void AimTowardMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _aimLayerMask))
        {
            var _direction = hitInfo.point - transform.position;
            _direction.y = 0;
            _direction.Normalize();
            transform.forward = _direction;
        }
    }

}
