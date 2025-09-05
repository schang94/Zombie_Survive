using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // ����Ƽ ����
using Photon.Realtime;
using UnityEngine.UI;

// ��ġ����ŷ(������) ������ �� ���� ���
public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1.0";
    public Text connectInfoTxt; // ��Ʈ��ũ ���� ǥ���� �ؽ�Ʈ
    public Button joinBtn; // �� ���� ��ư

    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings(); // �����Ʈ��ũ�� ���� ���� ����
        joinBtn.interactable = false;
        connectInfoTxt.text = $"������ ������ ���� �� .......";
    }

    // ������ ���� ���� ������ �ڵ� ����
    public override void OnConnectedToMaster()
    {
        joinBtn.interactable = true;
        connectInfoTxt.text = $"�¶��� : ������ ������ ����� .......";
    }
    // ������ ���� ���� ���н� �ڵ� ����
    public override void OnDisconnected(DisconnectCause cause)
    {
        joinBtn.interactable = false;
        connectInfoTxt.text = $"�������� : ������ ������ ���� ���� ����\n���� ��õ� .......";
    }
    
    // �� ���� �õ� (��ư�� �ֱ�)
    public void Connect()
    {
        joinBtn.interactable = false; // �ߺ� ������ ���� ����
        if (PhotonNetwork.IsConnected)
        {
            connectInfoTxt.text = "�뿡 ���� .......";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connectInfoTxt.text = $"�������� : ������ ������ ���� ���� ����\n���� ��õ� .......";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // ���� ���ӽ� ����� ���� ��
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectInfoTxt.text = "�� ���� �����ϴ�. .......";
        // �ִ� 4��
        PhotonNetwork.CreateRoom(null, new RoomOptions { IsOpen =true, IsVisible = true, MaxPlayers = 4 }, TypedLobby.Default);
    }
    
    // �뿡 ������ ��� �ڵ� ����
    public override void OnJoinedRoom()
    {
        connectInfoTxt.text = "�� ���� ���� ........";
        // ��� �����ڰ� MainScene ���� �ε��ϰ� �Ͽ���
        PhotonNetwork.LoadLevel("MainScene");
        // ���� �޼��尡 ���� �Ǹ� �ٸ� �÷��̾���� ��ǻ�Ϳ����� �ڵ�����
        // PhotonNetwork.LoadLevel("MainScene") �� ����Ǿ� �����
        // ���� ���� �ε��ϰ� �ȴ�.
        // PhotonNetwork.LoadLevel �ϸ� ������ :
        // �ڴʰ� �ش� �뿡 ������ �ٸ� �÷��̾ PhotonNetwork.LoadLevel�� 
        // ���� �÷��̾��� ���� ���� �������� �� ���߿� ������ �÷��̾ �ش� ���� �����
        // �ٸ� �÷��̾�� ���� ����� �����ϰ� �ڵ� �����Ǿ� ������ ���ϴ�.
    }
}
