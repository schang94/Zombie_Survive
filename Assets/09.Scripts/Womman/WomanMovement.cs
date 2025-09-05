using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// 1. 컴퍼넌트 애니메이터 리지디바디, WonmanInput 
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

    void FixedUpdate() // 물리적인 갱신, 주기마다 움직임, 회전, 애니메이션 처리
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
        rb.rotation = rb.rotation * Quaternion.Euler(0f, trun, 0f); // 원하는 각도로 회전
                                                                    // Quaternion.LookRotation() 원하는 방향으로 회전
    }
}
