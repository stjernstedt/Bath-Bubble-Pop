using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BubbleTypes
{
	Score,
	Plustime,
	Bomb,
	Random
}

public class Bubble : MonoBehaviour
{
	GameLogic gameLogic;
	public float moveSpeed;
	public float acceleration = 30f;
	public float curveLimit = 10f;
	//public float frequency = 1;
	//public float amplitude = 1;
	float timeElapsed = 0;
	public int value = 0;
	bool killZoned = false;
	public bool moveLeft = true;

	GameObject bombsParent;
	GameObject starsParent;
	GameObject randomsParent;
	GameObject plussesParent;

	Rigidbody2D rigidbody;

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
		plussesParent = GameObject.Find("Plusses");
		randomsParent = GameObject.Find("Randoms");
		rigidbody = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (!gameLogic.paused)
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
	}

	void Move()
	{
		if (rigidbody.velocity.x < -curveLimit)
		{
			moveLeft = false;
		}
		else if (rigidbody.velocity.x > curveLimit)
		{
			moveLeft = true;
		}

		if (moveLeft)
		{
			rigidbody.AddForce(new Vector2(-acceleration, 0));
		}
		else
		{
			rigidbody.AddForce(new Vector2(acceleration, 0));
		}

		if (rigidbody.velocity.y < moveSpeed)
		{
			rigidbody.AddForce(new Vector2(0, acceleration));
		}

	}

	public void PopBubble()
	{
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
						case "plus":
							gameLogic.AddTime(value);
							gameLogic.plusses.Push(child.gameObject);
							child.transform.SetParent(plussesParent.transform);
							break;
						case "random":
							PopRandom();
							gameLogic.randoms.Push(child.gameObject);
							child.transform.SetParent(randomsParent.transform);
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
						case "plus":
							gameLogic.plusses.Push(child.gameObject);
							child.transform.SetParent(plussesParent.transform);
							break;
						case "random":
							gameLogic.randoms.Push(child.gameObject);
							child.transform.SetParent(randomsParent.transform);
							break;
					}
				}
				gameObject.SetActive(false);
				gameLogic.bubbles.Push(gameObject);
				gameLogic.activeBubbles -= 1;

				child.gameObject.SetActive(false);
			}
		}
	}

	private void PopRandom()
	{
		int bubbleAmount = Random.Range(3, 5);

		int bubbleTypesCount = System.Enum.GetNames(typeof(BubbleTypes)).Length;

		for (int i = 0; i < bubbleAmount; i++)
		{
			BubbleTypes bubbleType = (BubbleTypes)Random.Range(0, bubbleTypesCount-1);
			GameObject bubble = gameLogic.SpawnBubble(bubbleType);
			bubble.transform.position = transform.position;
			Vector2 forceVector = new Vector2(Random.Range(-1500, 1500), Random.Range(-1500, -500));
			bubble.GetComponent<Rigidbody2D>().AddForce(forceVector);
		}
		//gameLogic.paused = true;
	}
}