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
        // �ſ� ���� ���� �ִ� ��� ���� ������Ʈ�� ��ȸ�ϸ� �˻��ϱ� ������
        // ������ ������ Awake�� Start������ ����Ѵ�.
        // public WomanHealth wh ���� ���� ���� �� �巡�� �� ������� �ؼ� ���
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
