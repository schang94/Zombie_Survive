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
    public int ammoRemaining; // ���� ��ü ź�� ��
    public int magAmmo; // ���� źâ�� ���� ź�� ��
    private float lastFireTime;
    //private Vector3 hitPosition;
    private WaitForSeconds shotEffectWs;
    private WaitForSeconds reloadWs;
    void Awake()
    {
        source = GetComponent<AudioSource>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; //���� �������� ������ ������ 2�� ����(���۰� ��
        lineRenderer.enabled = false;

        shotEffectWs = new WaitForSeconds(0.03f);
        reloadWs = new WaitForSeconds(gunData.reloadTime);
    }

    void Start()
    {
        // ��� ���� ��� TCP/IP�� ����
        //photonView.Synchronization = ViewSynchronization.ReliableDeltaCompressed;
        //photonView.ObservedComponents[0] = this;
    }
    private void OnEnable()
    {
        // �� ���� �ʱ�ȭ
        ammoRemaining = gunData.startAmmoRemaining;
        magAmmo = gunData.magCapacity;
        state = State.READY;
        lastFireTime = 0f;
    }

    public void Fire() // �߻�
    {
        if (state == State.READY && Time.time >= lastFireTime + gunData.timeBetFire)
        {
            lastFireTime = Time.time;
            Shot();
        }
    }

    void Shot() // ���� �߻� ó��
    {
        // ���� ��� ó���� ȣ��Ʈ���� �븮
        // ���� ��� ó���κ��� ȣ��Ʈ������ ���� �������� Ŭ���̾�Ʈ���� ���� ����ȭ
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

        lineRenderer.SetPosition(0, firePos.position); // ���� ������ ������ ����
        lineRenderer.SetPosition(1, hitPosition); // ���� ������ ���� ����

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

        int ammoToFill = gunData.magCapacity - magAmmo; // ä���� �� �Ѿ� �� ���
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

        if (stream.IsWriting) // ���� ������Ʈ��� ����
        {
            stream.SendNext(ammoRemaining);
            stream.SendNext(magAmmo);
            stream.SendNext(state);
        }
        else // ����� ������Ʈ��� �б�
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
