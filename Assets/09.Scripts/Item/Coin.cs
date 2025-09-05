using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Coin : MonoBehaviourPun, IItem
{
    private int score = 200;
    public void Use(GameObject target)
    {
        GameManager.Instance.AddScore(score);

        PhotonNetwork.Destroy(gameObject);
        //PhotonNetwork.Instantiate
        //PhotonNetwork.InstantiateRoomObject
    }

}
