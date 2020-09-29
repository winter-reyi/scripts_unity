///date: 2020-09-15
///author: winterwan
///purpose: 物体的抛物线运动脚本

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolicMotion : MonoBehaviour
{
	//抛物线运动目标
	public GameObject target;

	//运动速度
	private float speed = 10;

	//运动参数
	private float parameter = 30;

	//与目标距离
	private float distanceToTarget;

	//是否开始运动
	private bool move = true;

	void Start()
	{
		distanceToTarget = Vector3.Distance(this.transform.position, target.transform.position);
		parameter = Random.Range(10f, 35f);
		StartCoroutine(Shoot());
	}

	private IEnumerator Shoot()
	{
		while (move)
		{
			Vector3 targetPos = target.transform.position;
			//朝向目标  (Z轴朝向目标)
			this.transform.LookAt(targetPos);
			//根据距离衰减 角度
			float angle = Mathf.Min(1, Vector3.Distance(this.transform.position, targetPos) / distanceToTarget) *
			              parameter;
			//旋转对应的角度（线性插值一定角度，然后每帧绕X轴旋转）
			this.transform.rotation = this.transform.rotation * Quaternion.Euler(Mathf.Clamp(-angle, -42, 42), 0, 0);
			//当前距离目标点
			float currentDist = Vector3.Distance(this.transform.position, target.transform.position);
			if (currentDist < 0.5f)
			{
				move = false;
			}

			//平移 （朝向Z轴移动）
			this.transform.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, currentDist));
			yield return null;
		}
	}
}