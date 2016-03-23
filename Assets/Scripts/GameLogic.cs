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

public class GameLogic : MonoBehaviour
{
	public int maxBubbles = 10;
	public int activeBubbles = 0;

	public Stack<GameObject> bubbles = new Stack<GameObject>();
	public Stack<GameObject> bombs = new Stack<GameObject>();
	public Stack<GameObject> stars = new Stack<GameObject>();
	public Stack<GameObject> plusses = new Stack<GameObject>();
	public Stack<GameObject> randoms = new Stack<GameObject>();

	float timeLapsed = 0;
	protected float timeLimit = 1;
	public int minSpeed = 15;
	public int maxSpeed = 30;
	public float bombChance = 0.1f;
	public float minSpawnTime = 0.5f;
	public float maxSpawnTime = 1.5f;
	int score = 0;
	float timer = 11;
	Text scoreText;
	Text timerText;

	GameObject gameoverScreen;

	public GameState gameState = GameState.Playing;

	GameObject bubblePrefab;
	GameObject bombPrefab;
	GameObject starPrefab;
	GameObject plusPrefab;
	GameObject randomPrefab;

	GameObject bubblesParent;
	GameObject bombsParent;
	GameObject starsParent;
	GameObject plussesParent;
	GameObject randomsParent;

	public bool paused = false;

	// Use this for initialization
	void Start()
	{
		bubblePrefab = Resources.Load<GameObject>("Prefabs/bubble");
		bombPrefab = Resources.Load<GameObject>("Prefabs/bomb");
		starPrefab = Resources.Load<GameObject>("Prefabs/star");
		plusPrefab = Resources.Load<GameObject>("Prefabs/plus");
		randomPrefab = Resources.Load<GameObject>("Prefabs/question mark");

		bubblesParent = GameObject.Find("Bubbles");
		bombsParent = GameObject.Find("Bombs");
		starsParent = GameObject.Find("Stars");
		plussesParent = GameObject.Find("Plusses");
		randomsParent = GameObject.Find("Randoms");

		gameoverScreen = GameObject.Find("Gameover Screen");
		gameoverScreen.SetActive(false);
		scoreText = GameObject.Find("Score Text").GetComponent<Text>();
		scoreText.text = "" + score;
		timerText = GameObject.Find("Timer Text").GetComponent<Text>();
		timerText.text = "" + score;

		ResetGame();
	}

	// Update is called once per frame
	void Update()
	{
		if (!paused)
		{
			if (gameState == GameState.Playing)
			{
				UpdateTimer();
				CreateBubbles();
				PopBubble();
			}
		}
	}

	void UpdateTimer()
	{
		timer -= Time.deltaTime;
		timerText.text = "" + (int)timer;
		if (timer < 1)
		{
			Gameover();
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
				else if (number > 0.1f && number <= 0.2f)
				{
					SpawnBubble(BubbleTypes.Random);
				}
				else if (number > 0.2f && number <= 0.3f)
				{
					SpawnBubble(BubbleTypes.Plustime);
				}
				else
				{
					SpawnBubble(BubbleTypes.Score);
				}

			}
		}
	}

	public GameObject SpawnBubble(BubbleTypes bubbleType)
	{
		GameObject bubble = bubbles.Pop();
		Vector3 xStart = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
		Vector3 xEnd = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
		float padding = (xEnd.x - xStart.x) * 0.2f;
		float x = Random.Range(xStart.x + padding, xEnd.x - padding);
		float y = (Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y) - 15f;
		bubble.transform.position = new Vector3(x, y, 0);
		int movespeed = Random.Range(minSpeed, maxSpeed);
		bubble.GetComponent<Bubble>().moveSpeed = movespeed;
		bubble.SetActive(true);
		timeLapsed = 0;
		activeBubbles += 1;
		timeLimit = Random.Range(minSpawnTime, maxSpawnTime);

		Bubble bubbleComponent = bubble.GetComponent<Bubble>();

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
				//star.transform.localScale = new Vector3(1, 1, 1);
				bubbleComponent.value = Random.Range(1, 6) * 10;
				switch (bubbleComponent.value)
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

			case BubbleTypes.Plustime:
				GameObject plus = plusses.Pop();
				//plus.transform.localScale = new Vector3(0.8f, 0.8f, 1);
				bubbleComponent.value = Random.Range(1, 6);
				switch (bubbleComponent.value)
				{
					case 1:
						plus.GetComponent<SpriteRenderer>().color = Color.cyan;
						break;
					case 2:
						plus.GetComponent<SpriteRenderer>().color = Color.blue;
						break;
					case 3:
						plus.GetComponent<SpriteRenderer>().color = Color.green;
						break;
					case 4:
						plus.GetComponent<SpriteRenderer>().color = Color.yellow;
						break;
					case 5:
						plus.GetComponent<SpriteRenderer>().color = Color.red;
						break;
				}
				plus.transform.position = bubble.transform.position;
				plus.transform.SetParent(bubble.transform);
				plus.SetActive(true);
				break;

			case BubbleTypes.Random:
				GameObject random = randoms.Pop();
				//random.transform.localScale = new Vector3(0.8f, 0.8f, 1);
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

	public void AddTime(int time)
	{
		timer += time;
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

		foreach (Transform child in plussesParent.transform)
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

			GameObject plus = Instantiate(plusPrefab);
			plus.SetActive(false);
			plus.transform.SetParent(plussesParent.transform);
			plus.name = "plus";
			plusses.Push(plus);

			GameObject random = Instantiate(randomPrefab);
			random.SetActive(false);
			random.transform.SetParent(randomsParent.transform);
			random.name = "random";
			randoms.Push(random);

		}

		gameoverScreen.SetActive(false);
		score = 0;
		timer = 11;
		scoreText.text = "" + this.score;
		gameState = GameState.Playing;
	}
}
