using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// 플레이어 캐릭터를 조작 하기 위한 사용자 입력을 감지
// 감지된 입력값을 다른 컴퍼넌트가 사용할 수 있도록 전달.
public class WomanInput : MonoBehaviourPun
{
    private readonly string moveAxisName = "Vertical"; // 수평이동
    private readonly string rotateAxisName = "Horizontal"; // 회전이동
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
