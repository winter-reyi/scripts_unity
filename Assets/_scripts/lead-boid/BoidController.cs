using UnityEngine;
using System;
using System.Collections;

public class BoidController : MonoBehaviour
{

    public float minSpeed = 18;//最小移动速度  
    public float turnSpeed = 20;//旋转速度  
    public float randomFreq = 20;//用来确定更新randomPush变量的次数  
    public float randomForce = 18;//这个力产生出一个随机增长和降低的速度，并使得群组的移动看上去更加真实  

    //alignment variables列队变量  
    public float toOriginForce = 50;//用这个来保持所有boid在一个范围内，并与群组的原点保持一定距离  
    public float toOriginRange = 50;//群组扩展的程度  

    public float gravity = 2;

    //seperation variables分离变量  
    public float avoidanceRadius = 24;
    public float avoidanceForce = 14;//这两个变量可以让每个boid个体之间保持最小距离  

    //cohesion variables凝聚变量，这两个变量可用来与群组的领导者或群组的原点保持最小距离。  
    public float followVelocity = 4;
    public float followRadius = 40;

    //这些变量控制了boid的移动
    public Transform origin;//设为父对象，以控制整个群组中的对象。  
    private Vector3 velocity;
    private Vector3 normalizeedVelocity;
    private Vector3 randomPush;//更新基于randomFore的值  
    private Vector3 originPush;
    //以下两个变量存储相邻boid的信息，当前boid需要知道群组中其他boid的信息  
    private Transform[] objects;
    private BoidController[] otherFlocks;
    private Transform transformComponent;

    void Start()
    {
        #region 初始化一些变量
        followRadius = 40;
        minSpeed = 18;
        randomForce = 18;
        avoidanceRadius = 24;
        avoidanceForce = 14;
        #endregion

        randomFreq = 1.0f / randomFreq;

        //将父类指定为origin  
        //origin = transform.parent;

        //Flock transform  
        transformComponent = transform;

        //Temporary components临时  
        Component[] tempFlocks = null;

        //Get all the unity flock omponents from the parent transform in the group  
        if (origin)
        {
            tempFlocks = origin.GetComponent<LeadController>().henchman.ToArray();
        }

        //Assign and store all the flock objects in this group  
        objects = new Transform[tempFlocks.Length];
        otherFlocks = new BoidController[tempFlocks.Length];
        for (int i = 0; i < tempFlocks.Length; i++)
        {
            objects[i] = tempFlocks[i].transform;
            otherFlocks[i] = (BoidController)tempFlocks[i];
        }

        //Null Parent as the flok leader will be UnityFlockController object  
        transform.parent = null;

        //Calculate random push depends on the random frequency provided
        StartCoroutine(DelayDoSomething(2.0f, () =>
        {
            StartCoroutine(UpdateRandom());
        }));
    }

    IEnumerator UpdateRandom()
    {
        while (true)
        {
            randomPush = UnityEngine.Random.insideUnitSphere * randomForce;
            yield return new WaitForSeconds(randomFreq + UnityEngine.Random.Range(-randomFreq / 2.0f, randomFreq / 2.0f));
        }
    }

    void Update()
    {
        //internal variables  
        float speed = velocity.magnitude;//获取速度大小  
        Vector3 avgVelocity = Vector3.zero;
        Vector3 avgPosition = Vector3.zero;
        float count = 0;
        float f = 0.0f;
        float d = 0.0f;
        Vector3 myPosition = transformComponent.position;
        Vector3 forceV;
        Vector3 toAvg;
        Vector3 wantedVel;
        for (int i = 0; i < objects.Length; i++)
        {
            Transform transform = objects[i];
            if (transform != transformComponent)
            {
                Vector3 otherPosition = transform.position;
                //Average position to calculate cohesion  
                avgPosition += otherPosition;
                count++;
                //Directional vector from other flock to this flock  
                forceV = myPosition - otherPosition;
                //Magnitude of that diretional vector(length)  
                d = forceV.magnitude;
                //Add push value if the magnitude,the length of the vetor,is less than followRadius to the leader  
                if (d < followRadius)
                {
                    //calculate the velocity,the speed of the object,based current magnitude is less than the specified avoidance radius  
                    if (d < avoidanceRadius)
                    {
                        f = 1.0f - (d / avoidanceRadius);
                        if (d > 0)
                        {
                            avgVelocity += (forceV / d) * f * avoidanceForce;
                        }
                    }
                    //just keep the current distance with the leader  
                    f = d / followRadius;
                    BoidController otherSealgull = otherFlocks[i];
                    //we normalize the otherSealgull veloity vector to get the direction of movement,then wo set a new veloity  
                    avgVelocity += otherSealgull.normalizeedVelocity * f * followVelocity;
                }
            }
        }
        //上述代码实现了分离规则，首先，检查当前boid与其他boid之间的距离，并相应的更新速度，接下来，用当前速度除以群组中的boid的数目，计算出群组的平均速度  

        if (count > 0)
        {
            //Calculate the aveage flock veloity(Alignment)  
            avgVelocity /= count;
            //Calculate Center value of the flock（Cohesion)  
            toAvg = (avgPosition / count) - myPosition;
        }
        else
        {
            toAvg = Vector3.zero;
        }
        //Directional Vector to the leader  
        forceV = origin.position - myPosition;
        d = forceV.magnitude;
        f = d / toOriginRange;
        //Calculate the velocity of the flok to the leader  
        if (d > 0)
        {
            originPush = (forceV / d) * f * toOriginForce;
        }

        if (speed < minSpeed && speed > 0)
        {
            velocity = (velocity / speed) * minSpeed;
        }

        wantedVel = velocity;
        //Calculate final velocity  
        wantedVel -= wantedVel * Time.deltaTime;
        wantedVel += randomPush * Time.deltaTime;
        wantedVel += originPush * Time.deltaTime;
        wantedVel += avgVelocity * Time.deltaTime;
        wantedVel += toAvg.normalized * gravity * Time.deltaTime;
        //Final Velocity to rotate the flock into  
        velocity = Vector3.RotateTowards(velocity, wantedVel, turnSpeed * Time.deltaTime, 100.0f);

        transformComponent.rotation = Quaternion.LookRotation(velocity);

        transformComponent.Translate(velocity * Time.deltaTime, Space.World);

        normalizeedVelocity = velocity.normalized;
    }

    //延迟办事
    IEnumerator DelayDoSomething(float time, Action act)
    {
        yield return new WaitForSeconds(time);
        if (act != null)
        {
            act();
        }
    }
}