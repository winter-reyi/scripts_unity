using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeadController : MonoBehaviour
{

    public Vector3 offset;
    public Vector3 bound;
    public float speed = 15;

    private Vector3 initialPosition;
    private Vector3 nextMovementPoint;

    public List<BoidController> henchman;

    void Start()
    {
        offset = new Vector3(0, 0, 0);
        bound = new Vector3(80, 12, 15);
        speed = 15;

        initialPosition = transform.position;
        CalculateNextMovementPoint();
    }


    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextMovementPoint - transform.position), 1.0f * Time.deltaTime);

        if (Vector3.Distance(nextMovementPoint, transform.position) <= 10.0f)
        {
            CalculateNextMovementPoint();
        }
    }
    /* 
     在我们的Update（）方法中，检查控制器对象是否在最终目标位置附近，如果在，使用我们刚刚讨论过的CalculatNextMovementPoint（）方法再次更新nextMovementPoint变量 
         */

    void CalculateNextMovementPoint()
    {
        float posX = Random.Range(initialPosition.x - bound.x, initialPosition.x + bound.x);
        float posY = Random.Range(initialPosition.y - bound.y, initialPosition.y + bound.y);
        float posZ = Random.Range(initialPosition.z - bound.z, initialPosition.z + bound.z);
        nextMovementPoint = initialPosition + new Vector3(posX, posY, posZ);
    }
}