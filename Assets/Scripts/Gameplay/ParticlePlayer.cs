using System.Collections;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _particles;
    [SerializeField] private AudioSource _audio;
    [SerializeField] float _timerForLight = 0.1f;
    [SerializeField] private Light _light;
    public void PlayEffect()
    {
        if(_audio != null)
        {
            _audio.pitch = Random.Range(0.85f, 1f);
            _audio.PlayOneShot(_audio.clip);
        }
        if(_light != null)
        {
            _light.gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(DeactivedLight());
        }
        for (int i = 0; i < _particles.Length; i++)
        {
            _particles[i].Play();
        }
    }
    private IEnumerator DeactivedLight()
    {
        yield return new WaitForSeconds(_timerForLight);
        _light.gameObject.SetActive(false);
    }
}
