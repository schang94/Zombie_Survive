using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// �θ� Ŭ���� IDamageable�� ��ӹ޴� LivingEntity Ŭ���� -> Woman, Enemy �� ����ü�� ���� ����� ����
// ���� ���������� �÷��̾ �� ĳ������ �״� ������ ������ ��ġ�� �ణ �ٸ� �� ������ �������� ó���Ǿ���
public class LivingEntity : MonoBehaviourPun, IDamageable
{
    public float startHealth = 100f;
    public float Health {  get; protected set; }
    public bool Dead { get; protected set; }
    public event Action OnDeath;

    
    [PunRPC]
    public void ApplyUpdatedHealth(float newHealth, bool newDead)
    {
        Health = newHealth;
        Dead = newDead;
    }
    // ����ÿ� �ߵ� �� �̺�Ʈ
    protected virtual void OnEnable()
    {
        Dead = false;
        Health = startHealth;
    }

    // ������ ó��
    // ȣ��Ʈ���� ���� �ܵ� ����ǰ�, ȣ��Ʈ�� ���� �ٸ� Ŭ���̾�Ʈ�� �ϰ� ����
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (photonView.IsMine)
        {
            // ������ ��ŭ ����
            Health -= damage;

            // ȣ��Ʈ���� Ŭ���̾�Ʈ ����ȭ
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, Health, Dead);

            // �ٸ� Ŭ���̾�Ʈ�鵵 OnDamage�� ����
            photonView.RPC("OnDamage", RpcTarget.Others, damage, hitPoint, hitNormal);
        }

        if (Health <= 0 && !Dead)
        {
            Die();
        }

    }

    public virtual void RestoreHealth(float newHealth)
    {
        if (Dead) return;

        if (photonView.IsMine)
        {
            Health += newHealth;

            // ȣ��Ʈ���� Ŭ���̾�Ʈ ����ȭ
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, Health, Dead);

            // �ٸ� Ŭ���̾�Ʈ�鵵 OnDamage�� ����
            photonView.RPC("RestoreHealth", RpcTarget.Others, newHealth);
        }
        
    }
    public virtual void Die()
    {
        if (OnDeath != null)
        {
            OnDeath();
        }
        Dead = true;
        //gameObject.SetActive(false);
    }
}
