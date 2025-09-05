using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class ZombieSpawner : MonoBehaviourPun, IPunObservable
{
    public Zombie zombiePrefab;
    public ZombieData[] zombieDatas;

    public List<Transform> spwanPoints;
    public List<Zombie> zombieList = new List<Zombie>();
    private int wave;
    private int zombieCount = 0;

    private void Awake()
    {
        PhotonPeer.RegisterType(typeof(Color), 128, ColorSerialization.SerializeColor, ColorSerialization.DeserializeColor);
        // Pun2 에서 Color 타입은 RPC메서드의 입력으로 첨부 할 수 없다.
        // 기존의 RPC에서 지원하지 않는 타입을 직접 지원하도록 하는 것이
        // PhotonPeer.ResigterType(타입, 번호, 직렬화메서드, 역직렬화 메서드)메서드 이다.
    }
    private void Start()
    {
        zombieDatas = Resources.LoadAll<ZombieData>("ZombieData");

        var sp = GameObject.Find("Spawn Points");
        if (sp != null )
        {
            sp.GetComponentsInChildren<Transform>(spwanPoints);
        }
        spwanPoints.RemoveAt(0);
    }
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            {
                return;
            }

            if (zombieList.Count <= 0)
            {
                SpawnWave();
            }
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            UIManager.Instance.UpdateWaveText(wave, zombieList.Count);
        }
        else
        {
            UIManager.Instance.UpdateWaveText(wave, zombieCount);
        }
            
    }
    private void SpawnWave()
    {
        wave++;
        int spawnCount = Mathf.RoundToInt(wave * 1.5f); // 반올림해서 정수로 반환

        for (int i = 0; i < spawnCount; i++)
        {
            CreateZombie();
        }
    }

    private void CreateZombie()
    {
        ZombieData zombieData = zombieDatas[Random.Range(0, zombieDatas.Length)];
        Transform spawnPoint = spwanPoints[Random.Range(0, spwanPoints.Count)];
        GameObject createZombie = PhotonNetwork.Instantiate(zombiePrefab.gameObject.name, spawnPoint.position, spawnPoint.rotation);

        Zombie zombie = createZombie.GetComponent<Zombie>();

        zombie.photonView.RPC("SetUp", RpcTarget.All, zombieData.health, zombieData.damage, zombieData.speed, zombieData.skinColor);
        //zombie.SetUp(zombieData);

        zombieList.Add(zombie);

        zombie.OnDeath += () => zombieList.Remove(zombie);
        zombie.OnDeath += () => StartCoroutine(DestroyAfter(zombie.gameObject, 10f));
        zombie.OnDeath += () => GameManager.Instance.AddScore(100);
    }

    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (target != null)
        {
            PhotonNetwork.Destroy(target);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(zombieList.Count);
            stream.SendNext(wave);
        }
        else
        {
            zombieCount = (int) stream.ReceiveNext();
            wave = (int)stream.ReceiveNext();
        }
    }
}
