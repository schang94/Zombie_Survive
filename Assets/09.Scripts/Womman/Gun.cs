using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gun : MonoBehaviourPun, IPunObservable
{
    public enum State
    {
        READY, FIRE, RELOAD, EMPTY
    }
    //public State state = State.READY;
    public State state { get; private set; } = State.READY;
    public Transform firePos;
    public ParticleSystem muzzleFlashEffect;
    public ParticleSystem shellejectEffect;
    private LineRenderer lineRenderer;

    public GunData gunData;
    private float fireDistance = 100f;
    private AudioSource source;
    public int ammoRemaining; // 남은 전체 탄약 수
    public int magAmmo; // 현재 탄창에 남은 탄약 수
    private float lastFireTime;
    //private Vector3 hitPosition;
    private WaitForSeconds shotEffectWs;
    private WaitForSeconds reloadWs;
    void Awake()
    {
        source = GetComponent<AudioSource>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; //라인 렌더러의 포지션 개수를 2로 설정(사작과 끝
        lineRenderer.enabled = false;

        shotEffectWs = new WaitForSeconds(0.03f);
        reloadWs = new WaitForSeconds(gunData.reloadTime);
    }

    void Start()
    {
        // 통신 전송 방식 TCP/IP로 설정
        //photonView.Synchronization = ViewSynchronization.ReliableDeltaCompressed;
        //photonView.ObservedComponents[0] = this;
    }
    private void OnEnable()
    {
        // 총 상태 초기화
        ammoRemaining = gunData.startAmmoRemaining;
        magAmmo = gunData.magCapacity;
        state = State.READY;
        lastFireTime = 0f;
    }

    public void Fire() // 발사
    {
        if (state == State.READY && Time.time >= lastFireTime + gunData.timeBetFire)
        {
            lastFireTime = Time.time;
            Shot();
        }
    }

    void Shot() // 실제 발사 처리
    {
        // 실제 사격 처리는 호스트에서 대리
        // 실제 사격 처리부분을 호스트에서만 실행 나머지는 클라이언트에서 상태 동기화
        photonView.RPC("ShotProcessOnServer", RpcTarget.MasterClient);

        
        magAmmo--;

        if (magAmmo <= 0)
        {
            state = State.EMPTY;
        }
    }

    [PunRPC]
    private void ShotProcessOnServer()
    {
        RaycastHit hit;
        Vector3 hitPos = Vector3.zero;
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, fireDistance))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.OnDamage(gunData.damage, hit.point, hit.normal);
            }

            hitPos = hit.point;
        }
        else
        {
            hitPos = firePos.position + firePos.forward * fireDistance;
        }

        photonView.RPC("ShotEffectProcessOnClients", RpcTarget.All, hitPos);
    }

    [PunRPC]
    private void ShotEffectProcessOnClients(Vector3 hitPos)
    {
        StartCoroutine(ShotEffect(hitPos));
    }

    IEnumerator ShotEffect(Vector3 hitPosition)
    {
        muzzleFlashEffect.Play();
        shellejectEffect.Play();
        source.PlayOneShot(gunData.shotClip, 1.0f);

        lineRenderer.SetPosition(0, firePos.position); // 라인 렌더러 시작점 설정
        lineRenderer.SetPosition(1, hitPosition); // 라인 렌더러 끝점 설정

        lineRenderer.enabled = true;
        yield return shotEffectWs;

        lineRenderer.enabled = false;
    }

    public bool Reload()
    {
        if (state == State.RELOAD || ammoRemaining <= 0 || magAmmo >= gunData.magCapacity)
        {
            return false;
        }
        StartCoroutine(ReloadRoutine());
        return true;
    }

    IEnumerator ReloadRoutine()
    {
        state = State.RELOAD;
        source.PlayOneShot(gunData.reloadClip, 1.0f);

        yield return reloadWs;

        int ammoToFill = gunData.magCapacity - magAmmo; // 채워야 할 총알 수 계산
        if (ammoRemaining < ammoToFill)
        {
            ammoToFill = ammoRemaining;
        }

        magAmmo += ammoToFill;
        ammoRemaining -= ammoToFill;
        state = State.READY;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting) // 로컬 오브젝트라면 쓰기
        {
            stream.SendNext(ammoRemaining);
            stream.SendNext(magAmmo);
            stream.SendNext(state);
        }
        else // 리모드 오브젝트라면 읽기
        {
            ammoRemaining = (int)stream.ReceiveNext();
            magAmmo = (int)stream.ReceiveNext();
            state = (State)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void AddAmmo(int ammo)
    {
        ammoRemaining += ammo;
    }
}
