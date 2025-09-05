using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// 부모 클래스 IDamageable을 상속받는 LivingEntity 클래스 -> Woman, Enemy 등 생명체의 공통 기능을 구현
// 이전 로직에서는 플레이어나 적 캐릭터의 죽는 과정이 데미지 수치만 약간 다를 뿐 동일한 로직으로 처리되었음
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
    // 사망시에 발동 할 이벤트
    protected virtual void OnEnable()
    {
        Dead = false;
        Health = startHealth;
    }

    // 데미지 처리
    // 호스트에서 먼저 단독 실행되고, 호스트를 통해 다른 클라이언트에 일괄 실행
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (photonView.IsMine)
        {
            // 데미지 만큼 감소
            Health -= damage;

            // 호스트에서 클라이언트 동기화
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, Health, Dead);

            // 다른 클라이언트들도 OnDamage를 실행
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

            // 호스트에서 클라이언트 동기화
            photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, Health, Dead);

            // 다른 클라이언트들도 OnDamage를 실행
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
