using System.Collections;
using System.Collections.Generic;
using System;
using Kinect = Windows.Kinect;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class YunaManager : MonoBehaviour
{

    public Animator anim;
    GameManager playMenu;
    public Transform[] YunaBones;
    public Transform[] KinectBones;
    public int Score = 0;
    public TextMeshProUGUI YunaScoreText;
    public TextMeshProUGUI DebugYunaScoreText;
    public TextMeshProUGUI DebugExerciseSelected;
    public TextMeshProUGUI Timer;
    public GameObject GameOverPanel;
    public bool gameEnded = false;

    GameObject Bod = null;

    // Start is called before the first frame update
    void Start()
    {
        playMenu = FindObjectOfType<GameManager>();
        Debug.LogError("Exercise: " + playMenu.exercise);
        DebugExerciseSelected.SetText(playMenu.exercise.ToString());
        anim.SetInteger("exercise", playMenu.exercise);
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log("YunaArmRot " + YunaBones[0].rotation);
    }

    public void ShowEndScreen()
    {
        gameEnded = true;
        GameOverPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if(Score < 5)
        {
            YunaScoreText.SetText("Try Harder!");
        }
        if (Score < 10 && Score > 5)
        {
            YunaScoreText.SetText("Good!");
        }
        if(Score > 10 && Score < 20)
        {
            YunaScoreText.SetText("Very Good!");
        }
        if(Score > 20)
        {
            YunaScoreText.SetText("Fantastic!");
        }
    }

    public void OnCLickBack()
    {
        Debug.LogError("Click");
        SceneManager.LoadScene("MainMenu");
    }


    
}


/*
using System;
using Microsoft.Kinect;

namespace LightBuzz.Vitruvius
{
    public static class JointOrientationExtensionspublic static class JointOrientationExtensions
    {
        /// Rotates the specified quaternion around the X axis.
        public static double Pitch(this Vector4 quaternion)
        {
            double value1 = 2.0 * (quaternion.w * quaternion.x + quaternion.y * quaternion.z);
            double value2 = 1.0 - 2.0 * (quaternion.x * quaternion.x + quaternion.y * quaternion.y);

            double roll = Math.Atan2(value1, value2);

            return roll * (180.0 / Math.PI);
        }

        /// <summary>
        /// Rotates the specified quaternion around the Y axis.
        public static double Yaw(this Vector4 quaternion)
        {
            double value = 2.0 * (quaternion.w * quaternion.y - quaternion.z * quaternion.x);
            value = value > 1.0 ? 1.0 : value;
            value = value < -1.0 ? -1.0 : value;

            double pitch = Math.Asin(value);

            return pitch * (180.0 / Math.PI);
        }

        /// <summary>
        /// Rotates the specified quaternion around the Z axis.
        public static double Roll(this Vector4 quaternion)
        {
            double value1 = 2.0 * (quaternion.w * quaternion.z + quaternion.x * quaternion.y);
            double value2 = 1.0 - 2.0 * (quaternion.y * quaternion.y + quaternion.z * quaternion.z);

            double yaw = Math.Atan2(value1, value2);

            return yaw * (180.0 / Math.PI);
        }
    }
}
*/
