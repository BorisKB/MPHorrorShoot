using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticles : NetworkBehaviour
{
    [SerializeField] float _timer = 1f;
    [SerializeField] float _timerForLight = 0.1f;
    [SerializeField] Light _light;

    private void Start()
    {
        if(_light != null)
        {
            StartCoroutine(DeactivedLight());
        }
        StartCoroutine(DestroyParticle());
    }
    private IEnumerator DestroyParticle()
    {
        yield return new WaitForSeconds(_timer);
        NetworkServer.Destroy(gameObject);
    }
    private IEnumerator DeactivedLight()
    {
        yield return new WaitForSeconds(_timerForLight);
        _light.gameObject.SetActive(false);
    }
}
