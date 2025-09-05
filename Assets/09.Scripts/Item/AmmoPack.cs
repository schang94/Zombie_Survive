using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmmoPack : MonoBehaviourPun, IItem
{
    public int ammo = 30;
    public void Use(GameObject target)
    {
        WomanShooter womanShooter = target.GetComponent<WomanShooter>();

        if (womanShooter != null && womanShooter.gun != null)
        {
            //womanShooter.gun.ammoRemaining += ammo;
            womanShooter.gun.photonView.RPC("AddAmmo", RpcTarget.All, ammo);
        }

        //Destroy(gameObject);
        // 네트워크 객체가 삭제
        PhotonNetwork.Destroy(gameObject);
    }
}
