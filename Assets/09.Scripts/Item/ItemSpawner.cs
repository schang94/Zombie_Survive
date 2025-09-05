using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class ItemSpawner : MonoBehaviourPun
{
    public GameObject[] items;
    public Transform playerTr;

    public float maxDistance = 5f;
    
    public float timeBetSpawnMax = 7f;
    public float timeBetSpawnMin = 2f;

    private float timeBetSpawn;

    private float lastSpawnTime;

    void Start()
    {
        playerTr = GameObject.FindWithTag("Player").transform;
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        lastSpawnTime = 0;
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (Time.time >= lastSpawnTime + timeBetSpawn && playerTr != null)
        {
            lastSpawnTime = Time.time;
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);

            Spaw();
        }
    }

    private void Spaw()
    {
        Vector3 spawnPosition = GetRandomPointOnNavMesh(playerTr.position, maxDistance);
        spawnPosition += Vector3.up * 0.5f;

        GameObject selectedItem = items[Random.Range(0, items.Length)];
        GameObject item = PhotonNetwork.Instantiate(selectedItem.name, spawnPosition, Quaternion.identity);

        //Destroy(item, 5f);
        StartCoroutine(DestoryAfter(item, 5f));
    }

    IEnumerator DestoryAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (target != null)
        {
            PhotonNetwork.Destroy(target);
        }
    }
    private Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance)
    {
        // ���͸� �߽����� �������� maxDistance�� �� �ȿ� ������ ��ġ �ϳ��� ����
        // Random.insideUnitSphere �������� 1�� ���ȿ��� ������ ������ ��ȯ�ϴ� ������Ƽ
        Vector3 randomPos = Random.insideUnitSphere * distance + center;

        NavMeshHit hit; // ����޽� ���ø��� ����� �����ϴ� ����

        // maxDistance �ݿ��ȿ���, randomPos�� ���� ����� ����޽� ���� ������ ã��
        NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas);
        return hit.position;
    }
}
