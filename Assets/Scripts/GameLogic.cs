using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum GameState
{
	Menu,
	Playing,
	Gameover
}

public enum BubbleTypes
{
	Score,
	Bomb,
	Random
}

public class GameLogic : MonoBehaviour
{
	public int maxBubbles = 10;
	public int activeBubbles = 0;

	public Stack<GameObject> bubbles = new Stack<GameObject>();
	public Stack<GameObject> bombs = new Stack<GameObject>();
	public Stack<GameObject> stars = new Stack<GameObject>();
	public Stack<GameObject> randoms = new Stack<GameObject>();

	float timeLapsed = 0;
	protected float timeLimit = 1;
	public float maxSpeed = 0.3f;
	public float bombChance = 0.1f;
	public float minSpawnTime = 0.5f;
	public float maxSpawnTime = 1.5f;
	int score = 0;
	Text scoreText;

	GameObject gameoverScreen;

	public GameState gameState = GameState.Playing;

	GameObject bubblePrefab;
	GameObject bombPrefab;
	GameObject starPrefab;
	GameObject randomPrefab;

	GameObject bubblesParent;
	GameObject bombsParent;
	GameObject starsParent;
	GameObject randomsParent;

	// Use this for initialization
	void Start()
	{
		bubblePrefab = Resources.Load<GameObject>("Prefabs/bubble");
		bombPrefab = Resources.Load<GameObject>("Prefabs/bomb");
		starPrefab = Resources.Load<GameObject>("Prefabs/star");
		randomPrefab = Resources.Load<GameObject>("Prefabs/question mark");

		bubblesParent = GameObject.Find("Bubbles");
		bombsParent = GameObject.Find("Bombs");
		starsParent = GameObject.Find("Stars");
		randomsParent = GameObject.Find("Randoms");

		gameoverScreen = GameObject.Find("Gameover Screen");
		gameoverScreen.SetActive(false);
		scoreText = GameObject.Find("Score Text").GetComponent<Text>();
		scoreText.text = "" + score;

		ResetGame();
	}

	// Update is called once per frame
	void Update()
	{
		if (gameState == GameState.Playing)
		{
			CreateBubbles();
			PopBubble();
		}
	}

	void CreateBubbles()
	{
		timeLapsed += Time.deltaTime;
		if (activeBubbles < maxBubbles)
		{
			if (timeLapsed > timeLimit)
			{
				float number = Random.Range(0, 1f);

				if (number <= 0.1f)
				{
					SpawnBubble(BubbleTypes.Bomb);
				}
				if (number > 0.1f && number <= 0.2f)
				{
					SpawnBubble(BubbleTypes.Random);
				}
				else
				{
					SpawnBubble(BubbleTypes.Score);
				}

			}
		}
	}

	GameObject SpawnBubble(BubbleTypes bubbleType)
	{
		GameObject bubble = bubbles.Pop();
		Vector3 xStart = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
		Vector3 xEnd = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
		float padding = (xEnd.x - xStart.x) * 0.2f;
		float x = Random.Range(xStart.x + padding, xEnd.x - padding);
		float y = (Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y) - 15f;
		bubble.transform.position = new Vector3(x, y, 0);
		bubble.GetComponent<Bubble>().moveSpeed = Random.Range(0.1f, maxSpeed);
		bubble.SetActive(true);
		timeLapsed = 0;
		activeBubbles += 1;
		timeLimit = Random.Range(minSpawnTime, maxSpawnTime);

		switch (bubbleType)
		{
			case BubbleTypes.Bomb:
				GameObject bomb = bombs.Pop();
				bomb.transform.localScale = new Vector3(0.8f, 0.8f, 1);
				bomb.transform.position = bubble.transform.position;
				bomb.transform.SetParent(bubble.transform);
				bomb.SetActive(true);
				break;

			case BubbleTypes.Score:
				GameObject star = stars.Pop();
				star.transform.localScale = new Vector3(1, 1, 1);
				Bubble bubbleComp = bubble.GetComponent<Bubble>();
				bubbleComp.value = Random.Range(1, 6) * 10;
				switch (bubbleComp.value)
				{
					case 10:
						star.GetComponent<SpriteRenderer>().color = Color.cyan;
						break;
					case 20:
						star.GetComponent<SpriteRenderer>().color = Color.blue;
						break;
					case 30:
						star.GetComponent<SpriteRenderer>().color = Color.green;
						break;
					case 40:
						star.GetComponent<SpriteRenderer>().color = Color.yellow;
						break;
					case 50:
						star.GetComponent<SpriteRenderer>().color = Color.red;
						break;
				}
				star.transform.position = bubble.transform.position;
				star.transform.SetParent(bubble.transform);
				star.SetActive(true);
				break;

			case BubbleTypes.Random:
				GameObject random = randoms.Pop();
				random.transform.localScale = new Vector3(0.8f, 0.8f, 1);
				random.transform.position = bubble.transform.position;
				random.transform.SetParent(bubble.transform);
				random.SetActive(true);
				break;
		}
		return bubble;
	}

	void PopBubble()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100);
			if (hit.collider != null)
			{
				hit.collider.gameObject.GetComponent<Bubble>().PopBubble();
			}
		}
	}

	public void PushBubble(GameObject bubble)
	{
		bubbles.Push(bubble);
	}

	public void AddScore(int score)
	{
		this.score += score;
		scoreText.text = "" + this.score;
	}

	public void Gameover()
	{
		gameState = GameState.Gameover;
		gameoverScreen.SetActive(true);
	}

	public void ResetGame()
	{
		foreach (Transform child in bubblesParent.transform)
		{
			Destroy(child.gameObject);
		}

		foreach (Transform child in bombsParent.transform)
		{
			Destroy(child.gameObject);
		}

		foreach (Transform child in starsParent.transform)
		{
			Destroy(child.gameObject);
		}

		for (int i = 0; i < maxBubbles + 5; i++)
		{
			GameObject bub = Instantiate(bubblePrefab);
			bub.transform.SetParent(bubblesParent.transform);
			bub.SetActive(false);
			bub.name = "bubble";
			bubbles.Push(bub);

			GameObject bomb = Instantiate(bombPrefab);
			bomb.SetActive(false);
			bomb.transform.SetParent(bombsParent.transform);
			bomb.name = "bomb";
			bombs.Push(bomb);

			GameObject star = Instantiate(starPrefab);
			star.SetActive(false);
			star.transform.SetParent(starsParent.transform);
			star.name = "star";
			stars.Push(star);

			GameObject random = Instantiate(randomPrefab);
			random.SetActive(false);
			random.transform.SetParent(randomsParent.transform);
			random.name = "random";
			randoms.Push(random);

		}

		gameoverScreen.SetActive(false);
		score = 0;
		scoreText.text = "" + this.score;
		gameState = GameState.Playing;
	}
}
