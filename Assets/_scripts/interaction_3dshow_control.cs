using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interaction_3dshow_control : MonoBehaviour
{
    private bool flag = true;
    private float temp = 0f;
    
    //观察目标  
    [SerializeField] private Transform target;
    private Vector3 mPosition = new Vector3(0, 0, 0);

    //观察距离  
    private float Distance;

    //旋转速度  
    private float SpeedX = 240;
    private float SpeedY = 120;

    //角度限制  
    private float MinLimitY = 10;
    private float MaxLimitY = 80;

    //旋转角度  
    private float mX = 0.0F;
    private float mY = 0.0F;

    //鼠标缩放距离最值  
    private float MaxDistance = 10f;
    private float MinDistance = 2f;

    //鼠标缩放速率  
    private float ZoomSpeed = 3F;

    //存储角度的四元数  
    private Quaternion mRotation;

    //定义鼠标按键枚举  
    private enum MouseButton
    {
        //鼠标右键  
        MouseButton_Right = 1,
    }

    void OnEnable()
    {
        //初始化将中心物体的距离和本物体的距离设为Distance
        Distance = Vector3.Distance(Camera.main.transform.position, target.position);
    }

    void Start()
    {
        //初始化旋转角度
        mX = 270f;
        mY = 30f;
        mRotation = Quaternion.Euler(mY, mX, 0);
        Camera.main.transform.rotation = mRotation;
        Distance = 5f;

        //重新计算位置  
        mPosition = mRotation * new Vector3(0.0F, 0.0F, -Distance) + target.position;
    }

    void Update()
    {
        //鼠标右键旋转和滚轮旋转缩放
        if (target != null && Input.GetMouseButton((int) MouseButton.MouseButton_Right))
        {
            //获取鼠标输入  
            mX += Input.GetAxis("Mouse X") * SpeedX * 0.02F;
            mY -= Input.GetAxis("Mouse Y") * SpeedY * 0.02F;
            //范围限制  
            mY = ClampAngle(mY, MinLimitY, MaxLimitY);
            //计算旋转  
            mRotation = Quaternion.Euler(mY, mX, 0);

            Camera.main.transform.rotation = mRotation;
            flag = true;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            temp = Distance - ZoomSpeed * Input.GetAxis("Mouse ScrollWheel");
            Distance = Mathf.Clamp(temp, MinDistance, MaxDistance);
            flag = true;
        }

        //重新计算位置
        refreshPosition();

        if (flag)
        {
            Camera.main.transform.position = mPosition;
            flag = false;
        }
    }

    //角度限制  
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    //重新计算位置
    void refreshPosition()
    {
        mPosition = mRotation * new Vector3(0.0F, 0.0F, -Distance) + target.position;
    }

    void GetAsset()
    {
        AssetBundle ab = AssetBundle.LoadFromFile(Application.dataPath + "_assetbundles/logos");
    }
}