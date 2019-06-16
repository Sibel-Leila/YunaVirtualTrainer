using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using System;

public class BodySourceView : MonoBehaviour 
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;
    public YunaManager Yuna;
    public List<Vector3> CalculatedRotationsList = new List<Vector3>();
    public Vector3[] BoneRotations;
    public float timeToSearch = 15.0f;
    float timeleft = 30.0f;
    
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;
    
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };
    
    void Update () 
    {
        if (BodySourceManager == null)
        {
            return;
        }
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }
        
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        
        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
              }
                
            if(body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }

            
            if(body.IsTracked)
            {
                if(!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                
                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }
    }
    
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
       // body.transform.position = Yuna.transform.position;
        //body.tag = "KinectBody";
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);
            
            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            
            jointObj.transform.parent = body.transform;
        }
        
        return body;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        CalculatedRotationsList.Clear();
        timeleft -= Time.deltaTime;
        Yuna.Timer.SetText(timeleft.ToString());
        if(timeleft < 0 && !Yuna.gameEnded)
        {
            Yuna.ShowEndScreen();
            Debug.LogError("STOOOOOOOOOOOOOOOOOOOOOOOOP");
        }
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            
            if(_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            //Debug.Log("elbow left rotation: " + CalculatedRotation);
            float w = body.JointOrientations[jt].Orientation.W;
            float x = body.JointOrientations[jt].Orientation.X;
            float y = body.JointOrientations[jt].Orientation.Y;
            float z = body.JointOrientations[jt].Orientation.Z;
            Vector4 Orientation = new Vector4(x, y, z, w);
          //  Quaternion Q = new Quaternion(x, y, z, w);
            double rotX = Pitch(Orientation);
            double rotY = Yaw(Orientation);
            double rotZ = Roll(Orientation);
           
            Vector3 CalculatedRotation = new Vector3((float)rotX, (float)rotY, (float)rotZ);
            CalculatedRotationsList.Add(CalculatedRotation);
            timeToSearch -= Time.deltaTime;
            if(jt == Kinect.JointType.ShoulderLeft)
            {
               // Debug.Log("elbow left rotation: " + Orientation);
                Vector4 YunaElbowLeft = new Vector4(Yuna.YunaBones[0].transform.rotation.x, Yuna.YunaBones[0].transform.rotation.y, Yuna.YunaBones[0].transform.rotation.z, Yuna.YunaBones[0].transform.rotation.w);
                float distance = Vector4.Distance(Orientation, YunaElbowLeft );
                double kinectInverse = Orientation.x * -1;
                double result = Yuna.YunaBones[0].rotation.x - kinectInverse;

                if(result < 0.2f && timeToSearch < 0 && !Yuna.gameEnded )
                {
                    Yuna.Score++;
                    timeToSearch = 15.0f;
                }
                Debug.Log("Kienct rotation x: " + Orientation.x + " Yuna rotation X: " + Yuna.YunaBones[0].transform.rotation.x + " result: " + result);
                //if (distance <= 1 && timeToSearch < 0 && distance > 0.75f)
                //{
                //    Debug.Log("Distance elbow left = " + distance);
                //    Yuna.Score++;
                //    timeToSearch = 15.0f;
                //}
            }

            if (jt == Kinect.JointType.ShoulderRight)
            {
                // Debug.Log("elbow left rotation: " + Orientation);
                Vector4 YunaElbowRight = new Vector4(Yuna.YunaBones[1].transform.rotation.x, Yuna.YunaBones[1].transform.rotation.y, Yuna.YunaBones[1].transform.rotation.z, Yuna.YunaBones[1].transform.rotation.w);
                float distance = Vector4.Distance(Orientation, YunaElbowRight);

                double kinectInverse = Orientation.x * -1;
                double result = (float)Yuna.YunaBones[1].rotation.x - kinectInverse;

                if (result < 0.2f && timeToSearch < 0 && !Yuna.gameEnded)
                {
                    Yuna.Score++;
                    timeToSearch = 15.0f;
                }
                //if (distance <= 1 && timeToSearch < 0 )
                //{
                //    Debug.Log("Distance elbow right = " + distance);
                //    Yuna.Score++;
                //    timeToSearch = 15.0f;
                //}
            }
            Yuna.DebugYunaScoreText.SetText(Yuna.Score.ToString());
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }
        }
    }

    public static double Pitch(Vector4 quaternion)
    {
        double value1 = 2.0 * (quaternion.w * quaternion.x + quaternion.y * quaternion.z);
        double value2 = 1.0 - 2.0 * (quaternion.x * quaternion.x + quaternion.y * quaternion.y);

        double roll = Math.Atan2(value1, value2);

        return roll * (180.0 / Math.PI);
    }

    /// <summary>
    /// Rotates the specified quaternion around the Y axis.
    public static double Yaw(Vector4 quaternion)
    {
        double value = 2.0 * (quaternion.w * quaternion.y - quaternion.z * quaternion.x);
        value = value > 1.0 ? 1.0 : value;
        value = value < -1.0 ? -1.0 : value;

        double pitch = Math.Asin(value);

        return pitch * (180.0 / Math.PI);
    }

    /// <summary>
    /// Rotates the specified quaternion around the Z axis.
    public static double Roll(Vector4 quaternion)
    {
        double value1 = 2.0 * (quaternion.w * quaternion.z + quaternion.x * quaternion.y);
        double value2 = 1.0 - 2.0 * (quaternion.y * quaternion.y + quaternion.z * quaternion.z);

        double yaw = Math.Atan2(value1, value2);

        return yaw * (180.0 / Math.PI);
    }

    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.green;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
    
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
