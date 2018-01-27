using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
	public delegate void CollisionEnterHandler(Collision collision);
	public event CollisionEnterHandler CollisionEnter;

	public delegate void CollisionExitHandler(Collision collision);
	public event CollisionExitHandler CollisionExit;

	private void OnCollisionEnter(Collision collision)
	{
		if(CollisionEnter != null)
		{
			CollisionEnter(collision);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (CollisionExit != null)
		{
			CollisionExit(collision);
		}
	}
}
