using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayMenu : MonoBehaviour
{
    public int exerciseNuber = 0;
    public static GameManager gm;

    public int exercise;
    public static PlayMenu Instance;


    void Awake()
    {
        if (Instance != null)
        {
            GameObject.Destroy(Instance);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }


    public void OnButtonClick(int id)
    {
        gm.exercise = id;
        Debug.LogError(gm.exercise);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }
}
