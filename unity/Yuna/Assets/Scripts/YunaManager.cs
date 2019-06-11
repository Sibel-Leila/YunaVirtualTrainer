using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class YunaManager : MonoBehaviour
{

    public Animator anim;
    GameManager playMenu;
    public Transform[] YunaBones;
    Vector4 a;
    public Transform[] KinectBones;

    // Start is called before the first frame update
    void Start()
    {
        playMenu = FindObjectOfType<GameManager>();
        anim.SetInteger("exercise", playMenu.exercise);
        a.x
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  double Pitch(this Vector4 quaternion)
    {
        double value1 = 2.0 * (quaternion.w * quaternion.x + quaternion.y * quaternion.z);
        double value2 = 1.0 - 2.0 * (quaternion.x * quaternion.x + quaternion.y * quaternion.y);

        double roll = Math.Atan2(value1, value2);

        return roll * (180.0 / Math.PI);
    }

    public static double Yaw(this Vector4 quaternion)
    {
        double value = 2.0 * (quaternion.w * quaternion.y - quaternion.z * quaternion.x);
        value = value > 1.0 ? 1.0 : value;
        value = value < -1.0 ? -1.0 : value;

        double pitch = Math.Asin(value);

        return pitch * (180.0 / Math.PI);
    }

    public static double Roll(this Vector4 quaternion)
    {
        double value1 = 2.0 * (quaternion.w * quaternion.z + quaternion.x * quaternion.y);
        double value2 = 1.0 - 2.0 * (quaternion.y * quaternion.y + quaternion.z * quaternion.z);

        double yaw = Math.Atan2(value1, value2);

        return yaw * (180.0 / Math.PI);
    }
}
