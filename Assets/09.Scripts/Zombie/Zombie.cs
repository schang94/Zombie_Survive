using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Zombie : LivingEntity
{
    private readonly int hashHasTarget = Animator.StringToHash("HasTarget");
    private readonly int hashDie = Animator.StringToHash("Die");

    public LayerMask wahtIsTarget; // ������� ���̾�
    private LivingEntity targetEntity; // ������ ��� LivingEntity

    public AudioClip deadClip;
    public AudioClip hitClip;
    public ParticleSystem hitEffect;

    private Animator animator;
    private NavMeshAgent agent;
    private AudioSource source;
    //public MeshRenderer meshRenderer;
    public SkinnedMeshRenderer skinnedMeshRenderer;

    public float damage = 20f;
    public float timeBetAttack = 0.5f;
    private float lastAttckTime;

    private WaitForSeconds traceWs;
    private bool hasTarget
    {
        get
        {
            if (targetEntity != null && !targetEntity.Dead)
            {
                return true;
            }
            return false;
        }
    }
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        skinnedMeshRenderer = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        //meshRenderer = GetComponent<MeshRenderer>();
        traceWs = new WaitForSeconds(0.25f);
    }

    [PunRPC]
    public void SetUp(float newHealth, float newDamage,float newSpeed, Color skinColor) // ���� AI �ʱ� ������ �����ϴ� �¾� �޼���
    {
        startHealth = newHealth;
        Health = startHealth;

        damage = newDamage;
        agent.speed = newSpeed;
        skinnedMeshRenderer.material.color = skinColor;
    }

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        StartCoroutine(UpdatePath());
    }
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        animator.SetBool(hashHasTarget, hasTarget);
    }

    IEnumerator UpdatePath()
    {
        while (!Dead)
        {
            if (hasTarget)
            {
                agent.isStopped = false;
                agent.SetDestination(targetEntity.transform.position); // Ÿ����ġ�� ��� ����
                //agent.destination = targetEntity.transform.position;
            }
            else
            {
                agent.isStopped = true;

                Collider[] cols = Physics.OverlapSphere(transform.position, 20f, wahtIsTarget);

                for (int i = 0; i < cols.Length; i++)
                {
                    LivingEntity livingEntity = cols[i].GetComponent<LivingEntity>();
                    if (livingEntity != null && !livingEntity.Dead)
                    {
                        targetEntity = livingEntity; // ���� ��� ����
                        break;
                    }
                }
            }
            yield return traceWs;
        }


    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!Dead)
        {
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
            source.PlayOneShot(hitClip);
        }

        base.OnDamage(damage, hitPoint, hitNormal);
    }

    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);
    }

    public override void Die()
    {
        Collider[] zombieCols = GetComponents<Collider>();
        foreach (Collider col in zombieCols)
        {
            col.enabled = false;
        }
        agent.isStopped = true;
        agent.enabled = false;
        animator.SetTrigger(hashDie);
        source.PlayOneShot(deadClip);
        base.Die();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Ʈ���Ű� �浹�� ���� ���ӿ�����Ʈ�� ���� ����̶�� ���� ����
        if (!Dead && Time.time >= lastAttckTime + timeBetAttack)
        {
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();
            if (attackTarget != null && attackTarget==targetEntity)
            {
                lastAttckTime = Time.time;
                Vector3 hitPoint = other.ClosestPoint(transform.position); // ������ �ǰ� ��ġ�� �ǰ� ������ �ٻ����� ���
                Vector3 hitNormal = (transform.position - other.transform.position);
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
                
            }
        }
    }
}
