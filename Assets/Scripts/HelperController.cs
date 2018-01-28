using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelperController : MonoBehaviour
{
	public GameObject cursor;
	public GameObject display;
	public SpriteRenderer map;
	public Sprite[] sprites;

	int optionNumber = 0;
	float[] heights = {-1.1f, -2.5f, -3.9f };
	int scene;
	
	void Start ()
	{
		scene = SceneManager.GetActiveScene().buildIndex;
	}
	
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			optionNumber++;
			optionNumber %= 3;
			if(optionNumber == 1 && (scene == 0 || scene == 2))
			{
				optionNumber = 2;
			}
			if (optionNumber == 2 && (scene == 1 || scene == 3))
			{
				optionNumber = 0;
			}
			cursor.transform.localPosition = new Vector3(-9.1f, heights[optionNumber], -1f);
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			optionNumber--;
			if(optionNumber == -1)
			{
				optionNumber = 2;
			}
			if (optionNumber == 1 && (scene == 0 || scene == 2))
			{
				optionNumber = 0;
			}
			if (optionNumber == 2 && (scene == 1 || scene == 3))
			{
				optionNumber = 1;
			}
			cursor.transform.localPosition = new Vector3(-9.1f, heights[optionNumber], -1f);
		}

		if(Input.GetKeyDown(KeyCode.Space))
		{
			switch(optionNumber)
			{
				case 0:
					SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
					break;
				case 1:
					display.transform.localPosition = new Vector3(0f, 0f, 10f);
					break;
				case 2:
					StartCoroutine(LocatePlayer());
					break;
			}
		}
	}

	IEnumerator LocatePlayer()
	{
		map.sprite = sprites[0];
		yield return new WaitForSeconds(1f);
		map.sprite = sprites[1];
		yield return new WaitForSeconds(1f);

		if (scene == 0) map.sprite = sprites[2];
		if (scene == 2) map.sprite = sprites[3];
	}
}
