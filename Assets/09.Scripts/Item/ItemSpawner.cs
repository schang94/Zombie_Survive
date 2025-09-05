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
        // 센터를 중심으로 반지름이 maxDistance인 구 안에 랜덤한 위치 하나를 저장
        // Random.insideUnitSphere 반지금이 1인 구안에서 랜덤한 한점을 반환하는 프로퍼티
        Vector3 randomPos = Random.insideUnitSphere * distance + center;

        NavMeshHit hit; // 내비메시 샘플링의 결과를 저장하는 변수

        // maxDistance 반역안에서, randomPos에 가장 가까운 내비메시 위의 한점을 찾음
        NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas);
        return hit.position;
    }
}
