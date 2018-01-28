using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			StartCoroutine(StartGame());
		}
	}

	IEnumerator StartGame()
	{
		AsyncOperation loadScene = SceneManager.LoadSceneAsync(0);

		while (!loadScene.isDone)
		{
			yield return null;
		}
	}
}
