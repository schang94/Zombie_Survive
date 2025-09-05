using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// �÷��̾� ĳ���͸� ���� �ϱ� ���� ����� �Է��� ����
// ������ �Է°��� �ٸ� ���۳�Ʈ�� ����� �� �ֵ��� ����.
public class WomanInput : MonoBehaviourPun
{
    private readonly string moveAxisName = "Vertical"; // �����̵�
    private readonly string rotateAxisName = "Horizontal"; // ȸ���̵�
    private readonly string fireButtonName = "Fire1";
    private readonly string reloadButtonName = "Reload";
    
    public float Move {  get; private set; }
    public float Rotate { get; private set; }
    public bool Fire { get; private set; }
    public bool Reload { get; private set; }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            Move = 0;
            Rotate = 0;
            Fire = false;
            Reload = false;

            return;
        }

        Move = Input.GetAxis(moveAxisName);
        Rotate = Input.GetAxis(rotateAxisName);
        Fire = Input.GetButton(fireButtonName);
        Reload = Input.GetButton(reloadButtonName);


    }
}
