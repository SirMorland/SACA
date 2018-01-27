using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerWire : MonoBehaviour
{
	public GameObject controller1Object;
	SphereCollider controller1Collider;
	public GameObject controller2Object;
	SphereCollider controller2Collider;
	GameObject headObject;

	private SteamVR_TrackedObject controller1;
	private SteamVR_TrackedObject controller2;

	public SteamVR_Controller.Device Controller1
	{
		get {
			if (controller1) return SteamVR_Controller.Input((int)controller1.index);
			else return null;
		}
	}
	public SteamVR_Controller.Device Controller2
	{
		get
		{
			if (controller1) return SteamVR_Controller.Input((int)controller2.index);
			else return null;
		}
	}

	public Transform selectedControllerTransform;
	public bool hand1Colliding;
	public bool hand2Colliding;
	Vector3 oldPosition;
	SphereCollider head;
	Rigidbody rigidbody;

	Vector3 defaultGravity = new Vector3(0, -9.81f, 0f);

	void Awake()
	{
		controller1 = controller1Object.GetComponent<SteamVR_TrackedObject>();
		controller2 = controller2Object.GetComponent<SteamVR_TrackedObject>();
	}

	void Start ()
	{
		controller1Object.GetComponent<HandController>().CollisionEnter +=
			Hand1CollisionEnter;
		controller2Object.GetComponent<HandController>().CollisionEnter +=
			Hand2CollisionEnter;

		controller1Object.GetComponent<HandController>().CollisionExit +=
			Hand1CollisionExit;
		controller2Object.GetComponent<HandController>().CollisionExit +=
			Hand2CollisionExit;

		headObject = GameObject.FindGameObjectWithTag("MainCamera");
		head = GetComponent<SphereCollider>();
		rigidbody = GetComponent<Rigidbody>();
		controller1Collider = controller1Object.GetComponent<SphereCollider>();
		controller2Collider = controller2Object.GetComponent<SphereCollider>();
	}

	void Update ()
	{
		head.center = headObject.transform.localPosition - new Vector3(0f, 0.2f, 0f);

		if (Controller1.GetHairTriggerDown() && hand1Colliding)
		{
			selectedControllerTransform = controller1Object.transform;
			oldPosition = selectedControllerTransform.position;
			Physics.gravity = Vector3.zero;
			controller1Collider.enabled = false;
		}
		if (Controller2.GetHairTriggerDown() && hand2Colliding)
		{
			selectedControllerTransform = controller2Object.transform;
			oldPosition = selectedControllerTransform.position;
			Physics.gravity = Vector3.zero;
			controller2Collider.enabled = false;
		}
		if (Controller1.GetHairTriggerUp())
		{
			controller1Collider.enabled = true;
			hand1Colliding = false;
			if (selectedControllerTransform == controller1Object.transform)
			{
				selectedControllerTransform = null;
				Physics.gravity = defaultGravity;
				rigidbody.AddForce((oldPosition - controller1Object.transform.position) * 10000);
			}
		}
		if (Controller2.GetHairTriggerUp())
		{
			controller2Collider.enabled = true;
			hand2Colliding = false;
			if (selectedControllerTransform == controller2Object.transform)
			{
				selectedControllerTransform = null;
				Physics.gravity = defaultGravity;
				rigidbody.AddForce((oldPosition - controller2Object.transform.position) * 10000);
			}
		}

		if (selectedControllerTransform)
		{
			transform.position = Vector3.Lerp(
				transform.position,
				transform.position + (oldPosition - selectedControllerTransform.position),
				1f
			);
			oldPosition = selectedControllerTransform.position;
		}
	}

	void Hand1CollisionEnter(Collision collision)
	{
		if(collision.collider.tag == "Tube")
		{
			hand1Colliding = true;
			StartCoroutine(Vibrate(1));
		}
	}

	void Hand2CollisionEnter(Collision collision)
	{
		if (collision.collider.tag == "Tube")
		{
			hand2Colliding = true;
			StartCoroutine(Vibrate(2));
		}
	}

	void Hand1CollisionExit(Collision collision)
	{
		if (collision.collider.tag == "Tube")
		{
			hand1Colliding = false;
		}
	}

	void Hand2CollisionExit(Collision collision)
	{
		if (collision.collider.tag == "Tube")
		{
			hand2Colliding = false;
		}
	}

	IEnumerator Vibrate(int i)
	{
		float timer = 0.1f;
		while(timer >= 0)
		{
			timer -= Time.deltaTime;
			if(i == 1) Controller1.TriggerHapticPulse(2048);
			if(i == 2) Controller2.TriggerHapticPulse(2048);
			yield return null;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if(collision.collider.tag == "Tube")
		{
			selectedControllerTransform = null;
			Physics.gravity = defaultGravity;
			controller1Collider.enabled = true;
			controller2Collider.enabled = true;
		}
	}
}
