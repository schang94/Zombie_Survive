using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// 1. ���۳�Ʈ �ִϸ����� ������ٵ�, WonmanInput 
public class WomanMovement : MonoBehaviourPun
{
    private readonly int hashMove = Animator.StringToHash("Move");

    private WomanInput input;
    private Rigidbody rb;
    private Animator animator;
    private float moveSpeed = 5f;
    private float rotSpeed = 180f;

    
    void Start()
    {
        input = GetComponent<WomanInput>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate() // �������� ����, �ֱ⸶�� ������, ȸ��, �ִϸ��̼� ó��
    {
        if (!photonView.IsMine)
            return;

        Move();
        Rotate();
    }

    void Move()
    {
        Vector3 moveDistance = input.Move * transform.forward * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDistance);
        animator.SetFloat(hashMove, input.Move);
    }

    void Rotate()
    {
        float trun = input.Rotate * rotSpeed * Time.fixedDeltaTime;
        rb.rotation = rb.rotation * Quaternion.Euler(0f, trun, 0f); // ���ϴ� ������ ȸ��
                                                                    // Quaternion.LookRotation() ���ϴ� �������� ȸ��
    }
}
