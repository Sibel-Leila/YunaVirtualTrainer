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

     public void OnButtonClick(int id)
    {
        gm.exercise = id;
        Debug.Log(id);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }
}
