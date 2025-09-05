using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class WomanHealth : LivingEntity
{
    private readonly int hashDie = Animator.StringToHash("Die");

    public Slider healthSlider;
    public AudioClip deadClip;
    public AudioClip hitClip;
    public AudioClip itemPackUpClip;
    public AudioSource source;

    private Animator animator;
    private WomanMovement movement;
    private WomanShooter shooter;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
        movement = GetComponent<WomanMovement>();
        shooter = GetComponent<WomanShooter>();
        animator = GetComponent<Animator>();
    }

    protected override void OnEnable()
    {
        base.OnEnable(); // �θ�� ���� �ϰڴ�.
        healthSlider.gameObject.SetActive(true);
        healthSlider.value = Health;
        healthSlider.maxValue = startHealth;

        movement.enabled = true; // ������Ʈ Ȱ��ȭ
        shooter.enabled = true; // ������Ʈ Ȱ��ȭ
    }

    [PunRPC]
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!Dead)
        {
            source.PlayOneShot(hitClip);
        }
        base.OnDamage(damage, hitPoint, hitNormal);

        healthSlider.value = Health;
    }

    [PunRPC]
    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth);

        healthSlider.value = Health;
    }

    public override void Die()
    {

        healthSlider.gameObject.SetActive(false);
        source.PlayOneShot(deadClip);

        animator.SetTrigger(hashDie);

        movement.enabled = false;
        shooter.enabled = false;

        UIManager.Instance.SetActiveGameoverUI(true);

        Invoke("Respawn", 5f);
        base.Die();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!Dead)
        {
            IItem item = col.GetComponent<IItem>();
            if (item != null)
            {
                // ȣ��Ʈ�� ������ ���� ��� ����
                // ȣ��Ʈ���� �������� ��� ��, ���� �������� ȿ���� ��� Ŭ���̾�Ʈ�鿡�� ����ȭ ��Ŵ
                if (PhotonNetwork.IsMasterClient)
                {
                    item.Use(gameObject);
                }
                source.PlayOneShot(itemPackUpClip);
            }
        }
    }

    public void Respawn()
    {
        if (photonView.IsMine)
        {
            Vector3 randomSpawnPos = Random.insideUnitSphere * 5;
            randomSpawnPos.y = 0f;

            transform.position = randomSpawnPos;
        }

        gameObject.SetActive(false);
        gameObject.SetActive(true);

    }
}
