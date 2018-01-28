using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
	NavMeshAgent agent;
	GameObject player;
	
	void Start ()
	{
		agent = GetComponent<NavMeshAgent>();
		player = GameObject.FindGameObjectWithTag("MainCamera");
	}

	private void Update()
	{
		if(Vector3.Distance(transform.position, player.transform.position) < 1)
		{
			StartCoroutine(EndGame());
		}
	}

	void FixedUpdate ()
	{
		agent.destination = new Vector3(player.transform.position.x, 0.75f, player.transform.position.z);
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
