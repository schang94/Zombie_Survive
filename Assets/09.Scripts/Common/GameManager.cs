using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager m_instance;
    public static GameManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }
    private int score = 0;
    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        CreateWoman();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(score);
        }
        else
        {
            score = (int)stream.ReceiveNext();
            UIManager.Instance.UpdateScoreText(score);
        }
    }

    private void Start()
    {

        FindObjectOfType<WomanHealth>().OnDeath += EndGame;
        // 매우 느림 씬에 있는 모든 게임 오브젝트를 순회하며 검색하기 때문에
        // 느리기 때문에 Awake나 Start에서만 사용한다.
        // public WomanHealth wh 같이 변수 선언 후 드래그 앤 드롭으로 해서 사용
        // wh.OnDeath += EnGame;

    }


    public void AddScore (int newScroe)
    {
        if (!IsGameOver)
        {
            score += newScroe;

            UIManager.Instance.UpdateScoreText(score);
        }
    }

    public void EndGame()
    {
        IsGameOver = true;
        UIManager.Instance.SetActiveGameoverUI(true);
    }

    private void CreateWoman()
    {
        Vector3 randomPos = Random.insideUnitSphere * 5;
        randomPos.y = 0f;
        PhotonNetwork.Instantiate("Woman", randomPos, Quaternion.identity);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
