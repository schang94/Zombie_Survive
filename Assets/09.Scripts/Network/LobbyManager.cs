using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 유니티 전용
using Photon.Realtime;
using UnityEngine.UI;

// 매치메이킹(마스터) 서버와 룸 접속 담당
public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1.0";
    public Text connectInfoTxt; // 네트워크 정보 표시할 텍스트
    public Button joinBtn; // 룸 접속 버튼

    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings(); // 포톤네트워크로 버전 별로 접속
        joinBtn.interactable = false;
        connectInfoTxt.text = $"마스터 서버에 접속 중 .......";
    }

    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster()
    {
        joinBtn.interactable = true;
        connectInfoTxt.text = $"온라인 : 마스터 서버와 연결됨 .......";
    }
    // 마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        joinBtn.interactable = false;
        connectInfoTxt.text = $"오프라인 : 마스터 서버와 연결 되지 않음\n접속 재시도 .......";
    }
    
    // 룸 접속 시도 (버튼에 넣기)
    public void Connect()
    {
        joinBtn.interactable = false; // 중복 접속을 막기 위해
        if (PhotonNetwork.IsConnected)
        {
            connectInfoTxt.text = "룸에 접속 .......";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connectInfoTxt.text = $"오프라인 : 마스터 서버와 연결 되지 않음\n접속 재시도 .......";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // 랜덤 접속시 빈방이 없을 때
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectInfoTxt.text = "빈 방이 없습니다. .......";
        // 최대 4명
        PhotonNetwork.CreateRoom(null, new RoomOptions { IsOpen =true, IsVisible = true, MaxPlayers = 4 }, TypedLobby.Default);
    }
    
    // 룸에 참가된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        connectInfoTxt.text = "룸 접속 성공 ........";
        // 모든 참가자가 MainScene 씬을 로드하게 하였음
        PhotonNetwork.LoadLevel("MainScene");
        // 위의 메서드가 실행 되면 다른 플레이어들의 컴퓨터에서도 자동으로
        // PhotonNetwork.LoadLevel("MainScene") 이 실행되어 방장과
        // 같은 씬을 로드하게 된다.
        // PhotonNetwork.LoadLevel 하면 좋은점 :
        // 뒤늦게 해당 룸에 입장한 다른 플레이어가 PhotonNetwork.LoadLevel로 
        // 기존 플레이어들과 같은 씬에 도착했을 때 도중에 참가한 플레이어도 해당 씬의 모습이
        // 다른 플레이어보는 씬의 모습과 동일하게 자동 구성되어 굉장히 편하다.
    }
}
