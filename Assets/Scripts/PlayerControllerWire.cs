using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControllerWire : MonoBehaviour
{
	public GameObject controller1Object;
	Material controller1Material;
	public GameObject controller2Object;
	Material controller2Material;
	GameObject headObject;
	SpriteRenderer fader;

	private SteamVR_TrackedObject controller1;
	private SteamVR_TrackedObject controller2;

	public SteamVR_Controller.Device Controller1
	{
		get { return SteamVR_Controller.Input((int)controller1.index); }
	}
	public SteamVR_Controller.Device Controller2
	{
		get { return SteamVR_Controller.Input((int)controller2.index); }
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
		fader = headObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
		head = GetComponent<SphereCollider>();
		rigidbody = GetComponent<Rigidbody>();
		controller1Material = controller1Object.transform.GetChild(0).GetComponent<MeshRenderer>().material;
		controller2Material = controller2Object.transform.GetChild(0).GetComponent<MeshRenderer>().material;

		StartCoroutine(Begin());
	}

	void Update ()
	{
		head.center = headObject.transform.localPosition - new Vector3(0f, 0.2f, 0f);

		if (Controller1.GetHairTriggerDown() && hand1Colliding)
		{
			selectedControllerTransform = controller1Object.transform;
			oldPosition = selectedControllerTransform.position;
			Physics.gravity = Vector3.zero;
			//controller1MeshRenderer.material.color = new Color(0.2f, 0.2f, 0.2f);
		}
		if (Controller2.GetHairTriggerDown() && hand2Colliding)
		{
			selectedControllerTransform = controller2Object.transform;
			oldPosition = selectedControllerTransform.position;
			Physics.gravity = Vector3.zero;
			//controller2MeshRenderer.material.color = new Color(0.2f, 0.2f, 0.2f);
		}
		if (Controller1.GetHairTriggerUp())
		{
			//controller1MeshRenderer.material.color = new Color(1f, 1f, 1f);
			if (selectedControllerTransform == controller1Object.transform)
			{
				selectedControllerTransform = null;
				Physics.gravity = defaultGravity;
				rigidbody.AddForce((oldPosition - controller1Object.transform.position) * 10000);
			}
		}
		if (Controller2.GetHairTriggerUp())
		{
			//controller2MeshRenderer.material.color = new Color(1f, 1f, 1f);
			if (selectedControllerTransform == controller2Object.transform)
			{
				selectedControllerTransform = null;
				Physics.gravity = defaultGravity;
				rigidbody.AddForce((oldPosition - controller2Object.transform.position) * 10000);
			}
		}

		if (selectedControllerTransform)
		{
			rigidbody.velocity = Vector3.zero;

			transform.position = Vector3.Lerp(
				transform.position,
				transform.position + (oldPosition - selectedControllerTransform.position),
				1f
			);
			oldPosition = selectedControllerTransform.position;
		}

		if(transform.position.y < -10)
		{
			StartCoroutine(EndGame());
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == "Tube")
		{
			selectedControllerTransform = null;
			Physics.gravity = defaultGravity;
		}
	}

	void Hand1CollisionEnter(Collision collision)
	{
		hand1Colliding = true;
		controller1Material.SetColor("_EmissionColor", new Color(1f, 1f, 0.8f));
		StartCoroutine(Vibrate(1));

		if(collision.collider.tag == "Goal")
		{
			StartCoroutine(End());
		}
	}

	void Hand2CollisionEnter(Collision collision)
	{
		hand2Colliding = true;
		controller2Material.SetColor("_EmissionColor", new Color(1f, 1f, 0.8f));
		StartCoroutine(Vibrate(2));

		if (collision.collider.tag == "Goal")
		{
			StartCoroutine(End());
		}
	}

	void Hand1CollisionExit(Collision collision)
	{
		hand1Colliding = false;
		controller1Material.SetColor("_EmissionColor", new Color(1f, 1f, 1f));
	}

	void Hand2CollisionExit(Collision collision)
	{
		hand2Colliding = false;
		controller2Material.SetColor("_EmissionColor", new Color(1f, 1f, 1f));
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

	IEnumerator Begin()
	{
		while (fader.color.a > 0.01f)
		{
			fader.color = Color.Lerp(fader.color, new Color(0.4f, 1f, 1f, 0f), Time.deltaTime);
			yield return null;
		}
	}

	IEnumerator End()
	{
		AsyncOperation loadScene = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

		while(fader.color.a < 0.9f && !loadScene.isDone)
		{
			fader.color = Color.Lerp(fader.color, new Color(0.4f, 1f, 1f, 1f), Time.deltaTime);
			yield return null;
		}

		//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	IEnumerator EndGame()
	{
		AsyncOperation loadScene = SceneManager.LoadSceneAsync(5);

		while (!loadScene.isDone)
		{
			yield return null;
		}
	}
}
