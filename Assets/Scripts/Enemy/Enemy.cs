using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class Enemy : NetworkBehaviour
{
    [SerializeField] private List<Transform> _playersList = new List<Transform>();
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator animator;

    [SerializeField] private int _damage = 5;
    [SerializeField] private float _attackRate = 2f;

    [SerializeField] private AudioClip[] _audioShags;
    [SerializeField] private AudioClip _smert;
    [SerializeField] private AudioSource _voi;
    [SerializeField] private AudioSource _ruck;
    [SerializeField] private AudioSource audioSource;
    private Transform _currentTarget;
    private float _timer = 0;
    private bool _isAction = false;
    private bool _isRuck = false;
    
    public void AddTarget(Transform target)
    {
        _playersList.Add(target);
    }
    public void RemoveTarget(Transform target)
    {
        _playersList.Remove(target);
        _currentTarget = null;
    }

    private void Start()
    {
        if (isServer)
        {
            if(Random.Range(0, 4) == 1)
            {
                RpcPlayRuckOrVoi(0);
            }
            _agent.speed = Random.Range(5, 15);
            _damage = Random.Range(10, 50);
        }
    }
    [ClientRpc]
    private void RpcPlayRuckOrVoi(int index)
    {
        if(index == 0)
        {
            _voi.pitch = Random.Range(0.5f, 0.8f);
            _voi.Play();
        }
        else
        {
            _ruck.pitch = Random.Range(0.5f, 0.8f);
            _ruck.Play();
        }
    }
    private void Update()
    {
        if (isServer)
        {
            _timer += Time.deltaTime;
            if (_timer > 0.2f)
            {
                if (!_isAction)
                {
                    _timer = 0;
                    if (_currentTarget == null)
                    {
                        _currentTarget = FindTarget();
                    }
                    else
                    {
                        float dist = (_playersList[0].position - transform.position).sqrMagnitude;
                        if(!_isRuck && dist <= 15 * 15)
                        {
                            
                            _isRuck = true;
                            RpcPlayRuckOrVoi(1);
                        }
                        if (dist >= 4f)
                        {
                            _agent.SetDestination(_currentTarget.position);
                        }
                        else
                        {
                            Attack();
                        }
                    }
                }
            }
            if(_agent.velocity.magnitude > 0.1f)
            {
                animator.SetBool("isWalk", true);
            }
            else
            {
                animator.SetBool("isWalk", false);
            }
        }
    }
    private void Attack()
    {
        _currentTarget.GetComponent<IDamagable>().TakeDamage(_damage);
        _agent.ResetPath();
        _agent.velocity = Vector3.zero;
        animator.SetBool("Attack", true);
        StopAllCoroutines();
        StartCoroutine(AttackRate());
    }
    private IEnumerator AttackRate()
    {
        _isAction = true;
        yield return new WaitForSeconds(_attackRate);
        _isAction = false;
        animator.SetBool("Attack", false);
    }
    private Transform FindTarget()
    {
        if(_playersList.Count == 0)
        {
            return null;
        }
        float dist = (_playersList[0].position - transform.position).sqrMagnitude;
        Transform target = _playersList[0];
        if(_playersList.Count > 1)
        {
            for (int i = 1; i < _playersList.Count; i++)
            {
                if((_playersList[i].position - transform.position).sqrMagnitude < dist)
                {
                    dist = (_playersList[i].position - transform.position).sqrMagnitude;
                    target = _playersList[i];
                }
            }
        }
        return target;
    }

    public void OnFootStep()
    {
        audioSource.clip = _audioShags[Random.Range(0, _audioShags.Length)];
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(audioSource.clip);
    }
    public void OnDeathEnemy()
    {
        _agent.ResetPath();
        _agent.velocity = Vector3.zero;
        audioSource.clip = _smert;
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(audioSource.clip);

    }
}
