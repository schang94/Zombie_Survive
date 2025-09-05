using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class WomanShooter : MonoBehaviourPun
{
    private readonly int hashReload = Animator.StringToHash("Reload");
    public Gun gun;
    public Transform gunPivot;
    public Transform leftHandleMount; // ���� ���� ������  �޼��� ��ġ�� ����
    public Transform rightHandleMount; // ���� ������ ������ �������� ��ġ�� ����

    public WomanInput input;
    private Animator animator;

    private void OnEnable()
    {
        //gun.gameObject.SetActive(true);    
    }

    void Start()
    {
        input = GetComponent<WomanInput>();
        animator = GetComponent<Animator>();
        gun = GetComponentInChildren<Gun>();
        gunPivot = gun.transform.parent;
        leftHandleMount = gun.transform.GetChild(1).transform;
        rightHandleMount = gun.transform.GetChild(2).transform;
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        // �Է��� ����ϰ� ���� �߻��ϴ� ����
        if (input.Fire)
        {
            gun.Fire();
        }
        else if (input.Reload)
        {
            if (gun.Reload())
            {
                //gun.Reload();
                animator.SetTrigger(hashReload);
            }
        }

        UpdateUI();
    }
    void UpdateUI()
    {
        if (gun != null && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateAmmoText(gun.magAmmo, gun.ammoRemaining);
        }
    }
    private void OnAnimatorIK(int layerIndex)
    {
        // �ִϸ��̼� IK�� ����Ͽ� �� ��ġ ����
        // �ִϸ������� �ǽð� IK ������Ʈ
        gunPivot.position = animator.GetIKHintPosition(AvatarIKHint.RightElbow); // ������ �Ȳ��� ��ġ�� �Ǻ� ��ġ�� ����
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f); // ������ IK ��ġ ����ġ ����
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f); // ������ IK ȸ�� ����ġ ����
        
        // IK�� ����Ͽ� ������ ��ġ�� ȸ���� ����
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandleMount.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandleMount.rotation);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f); // �޼� IK ��ġ ����ġ ����
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f); // �޼� IK ȸ�� ����ġ ����

        // IK�� ����Ͽ� �޼� ��ġ�� ȸ���� ����
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandleMount.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandleMount.rotation);
    }
    private void OnDisable()
    {
        //gun.gameObject.SetActive(false);
    }
}
