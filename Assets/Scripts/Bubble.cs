using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bubble : MonoBehaviour
{
	GameLogic gameLogic;
	public float moveSpeed;
	public float frequency = 1;
	public float amplitude = 1;
	float timeElapsed = 0;
	public int value = 0;
	bool killZoned = false;

	GameObject bombsParent;
	GameObject starsParent;
	GameObject randomsParent;

	void Awake()
	{
		timeElapsed = 0;
	}

	// Use this for initialization
	void Start()
	{
		gameLogic = GameObject.Find("Logic").GetComponent<GameLogic>();
		bombsParent = GameObject.Find("Bombs");
		starsParent = GameObject.Find("Stars");
		randomsParent = GameObject.Find("Randoms");
	}

	// Update is called once per frame
	void Update()
	{
		if (gameLogic.gameState == GameState.Playing)
		{
			Move();
		}

		if (transform.position.y > (Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y + 10))
		{
			killZoned = true;
			PopBubble();
			killZoned = false;
		}
		timeElapsed += Time.deltaTime;
	}

	void Move()
	{
		float x = Mathf.Sin(timeElapsed * frequency);
		x *= amplitude;
		transform.Translate(x, moveSpeed, 0);
	}

	public void PopBubble()
	{
		gameObject.SetActive(false);
		gameLogic.bubbles.Push(gameObject);
		gameLogic.activeBubbles -= 1;

		if (transform.childCount > 0)
		{
			foreach (Transform child in transform)
			{
				if (!killZoned)
				{
					switch (child.gameObject.name)
					{
						case "bomb":
							gameLogic.Gameover();
							break;
						case "star":
							gameLogic.AddScore(value);
							gameLogic.stars.Push(child.gameObject);
							child.transform.SetParent(starsParent.transform);
							break;
						case "random":

							break;
					}
				}

				if (killZoned)
				{
					switch (child.gameObject.name)
					{
						case "bomb":
							gameLogic.bombs.Push(child.gameObject);
							child.transform.SetParent(bombsParent.transform);
							break;
						case "star":
							gameLogic.stars.Push(child.gameObject);
							child.transform.SetParent(starsParent.transform);
							break;
						case "random":
							gameLogic.randoms.Push(child.gameObject);
							child.transform.SetParent(randomsParent.transform);
							break;
					}
				}

				child.gameObject.SetActive(false);
			}
		}
	}
}