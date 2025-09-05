using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>(); // 해당 씬에 UIManager가 있는지  찾는다 없으면 새로 생성한다.
            }

            return m_instance;
        }
    }

    private static UIManager m_instance;

    public Text ammoText;
    public Text scoreText;
    public Text waveText;
    public GameObject gameoverUI;

    public void UpdateAmmoText(int magAmmo, int remainAmmo)
    {
        ammoText.text = magAmmo + "/" + remainAmmo;
    }
    public void UpdateScoreText(int newScore)
    {
        scoreText.text = "Score : " + newScore;
    }

    public void UpdateWaveText(int waves, int count)
    {
        waveText.text = $"Wave : {waves}\nEnemy Left : {count}";
    }
    public void SetActiveGameoverUI(bool active)
    {
        transform.GetChild(3).gameObject.SetActive(active);
    }
    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
