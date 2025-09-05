using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class WomanShooter : MonoBehaviourPun
{
    private readonly int hashReload = Animator.StringToHash("Reload");
    public Gun gun;
    public Transform gunPivot;
    public Transform leftHandleMount; // 총의 왼쪽 손잡이  왼손이 위치할 지점
    public Transform rightHandleMount; // 총의 오른쪽 손잡이 오른손이 위치할 지점

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
        // 입력을 잠시하고 총을 발사하는 로직
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
        // 애니메이션 IK를 사용하여 손 위치 조정
        // 애니메이터의 실시간 IK 업데이트
        gunPivot.position = animator.GetIKHintPosition(AvatarIKHint.RightElbow); // 오른팔 팔꿈지 위치를 피봇 위치로 설정
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f); // 오른손 IK 위치 가중치 설정
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f); // 오른손 IK 회전 가중치 설정
        
        // IK를 사용하여 오른손 위치와 회전을 설정
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandleMount.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandleMount.rotation);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f); // 왼손 IK 위치 가중치 설정
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f); // 왼손 IK 회전 가중치 설정

        // IK를 사용하여 왼손 위치와 회전을 설정
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandleMount.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandleMount.rotation);
    }
    private void OnDisable()
    {
        //gun.gameObject.SetActive(false);
    }
}
