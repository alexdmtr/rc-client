/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game
{
	public static Vector3 InterpolatePosition(Vector3 initial, Vector3 target, float part)
	{
		return Vector3.Lerp(initial, target, part);
	}
	
	public static Quaternion InterpolateRotation(Quaternion initial, Quaternion target, float part)
	{
		return Quaternion.Slerp(initial, target, part);
	}

	public KeyValuePair<Vector3, Quaternion> GetCardCoords(Card card)
	{
		int CardID = card.ID;
		Vector3 position = Vector3.zero;
		Quaternion rotation = Quaternion.identity;

		if (card.State == CardStates.InDeck)
		{
			float cardX, cardY, cardZ;
			float distanceY = 0.015f;

			cardX = -4f;
			cardZ = card.PlayerID == 0 ? 3: -3;
			if (card.Position == null)
				Debug.Log ("Error. Card " + CardID.ToString() + " has no position.");
			else
			{
				cardY = 0.5f;
				cardY += distanceY * (int)card.Position;


				position = new Vector3(cardX, cardY, cardZ);
				rotation = Quaternion.Euler(new Vector3(180, card.PlayerID == 0? 180: 0, 0));
			}
		}
		if (card.State == CardStates.InHand)
		{
			float cardX, cardY, cardZ;
			float centerX = 0f, centerY = 0.5f, centerZ = card.PlayerID == 0 ? 7.25f: -7.25f;
			float radiusX = 3f, radiusZ = 3f;
			float radiusY = 0.03f;
			float angleBetweenCards = 9;
			float angle;

			List<Card> PlayerCards = Board.GetPlayerInHandCards(card.PlayerID);
			if (card.Position == null)
				Debug.Log ("Error. Card " + CardID.ToString() + " has no position.");
			else
			{
				angle = 90 + angleBetweenCards * (PlayerCards.Count / 2.0f);
				angle -= angleBetweenCards * (int)card.Position;

				cardX = centerX + (card.PlayerID == 0 ? -1 : 1) * radiusX * Mathf.Cos(angle / 180f * Mathf.PI);
				cardZ = centerZ + (card.PlayerID == 0 ? -1 : 1) * radiusZ * Mathf.Sin(angle / 180f * Mathf.PI); 
				cardY = centerY + radiusY * (int)card.Position;
				//cardY = Mathf.Sin (angle / 180f * Mathf.PI) * 5;

				position = new Vector3(cardX, cardY, cardZ);
				rotation = Quaternion.Euler(new Vector3(card.PlayerID == 0 ? 0 : 180, -(angle-90), 0));
			}
		}
		if (card.State == CardStates.OnBoard)
		{
			float minionX, minionY, minionZ;
			float distanceBetweenMinions = 1.1f;
			minionY = 0.5f;
			if (card.PlayerID == 0)
				minionZ = 1f;
			else
				minionZ = -1f;

			List<Card> PlayerMinions = Board.GetPlayerOnBoardMinions(card.PlayerID);

			if (card.Position == null)
				Debug.Log ("Error. Minion " + CardID.ToString() + " has no position.");
			else
			{
				minionX = 0 + (PlayerMinions.Count / 2) * distanceBetweenMinions;
				minionX -= ((int)card.Position) * distanceBetweenMinions;
				if (PlayerMinions.Count % 2 == 0)
					minionX -= distanceBetweenMinions / 2;

				position = new Vector3(minionX, minionY, minionZ);
				rotation = Quaternion.identity;
			}
		}
		if (card.State == CardStates.Hero)
		{
			float heroX, heroY, heroZ;
			heroY = 0.5f;
			heroX = 0f;
			if (card.PlayerID == 0)
				heroZ = 2.75f;
			else
				heroZ = -2.75f;

			position = new Vector3(heroX, heroY, heroZ);
			rotation = Quaternion.identity;
		}

		return new KeyValuePair<Vector3, Quaternion>(position, rotation);
	}
	
	public Dictionary<int, GameCard> CardArray = new Dictionary<int, GameCard>();
	public List<GameObject> ManaCrystals = new List<GameObject>();
	public GameObject CardPrefab;
	public GameObject ManaCrystalPrefab;
	public AnimationChain[] AnimationChainArray = new AnimationChain[2];
	public BoardState Board;
	public int HumanPlayer;


	private GameCard CreateCard(BoardState board, Card card)
	{
		KeyValuePair<Vector3, Quaternion> kvp = GetCardCoords(card);
		GameCard gameCard = ((GameObject)GameObject.Instantiate(CardPrefab, kvp.Key, kvp.Value)).GetComponent<GameCard>();
		gameCard.CardID = card.ID;
		UpdateCardTexture (gameCard, board, card);
		return gameCard;
	}
	private void UpdateCardTexture(GameCard gameCard, BoardState board, Card card)
	{
		gameCard.UpdateCard(@"<b>" + card.GetName() + @"</b>", @"Textures/Cards/" + card.GetCardGFX(), board.GetCardManaCost(card), board.GetCardAttack(card), board.GetCardHealth(card), card.State);
	}

	public void UpdateManaCrystals()
	{
		foreach(GameObject crystal in ManaCrystals)
			GameObject.Destroy(crystal);
		ManaCrystals.Clear ();

		for (int i = 0; i < Board.Players.Count; i++)
		{
			float crystalX, crystalY, crystalZ;
			float crystalDistance = 0.25f;
			crystalY = 0.5f;
			crystalX = -2.5f;
			if (i == 0)
				crystalZ = 4.75f;
			else
				crystalZ = -4.75f;

			for (int j = 0; j < Board.Players[i].CurrentMana; j++)
			{
				ManaCrystals.Add((GameObject)GameObject.Instantiate(ManaCrystalPrefab, new Vector3(crystalX - crystalDistance *j, crystalY, crystalZ), Quaternion.identity));
			}
		}
	}

	
	public void UpdateState()
	{
		foreach (Card card in Board.Cards)
		{
			int cardID = card.ID;
			if (CardArray.ContainsKey(cardID))
			{
				UpdateCardTexture(CardArray[cardID], Board, card);
				ReturnCard(card, CardArray[cardID]);
			}
			else
				CardArray[cardID] = CreateCard(Board, card);

		}
		UpdateManaCrystals();
	}

	public Game(GameObject CardPrefab, GameObject ManaCrystalPrefab)
	{
		previousMousePosition = currentMousePosition = Vector3.zero;

		this.CardPrefab = CardPrefab;
		this.ManaCrystalPrefab = ManaCrystalPrefab;

		Board = new BoardState();
		UpdateState ();
	}

	GameCard selectedCard;
	GameCard targetedCard;
	Vector3 previousMousePosition;
	Vector3 currentMousePosition;

	void ReturnCard(Card card, GameCard gameCard)
	{
		gameCard.CurrentAnimation = new GameAnimation(gameCard.transform, AnimationTypes.Simple, GetCardCoords(card).Key, GetCardCoords(card).Value, 0.5f);
	}

	void UpdateInput()
	{
		previousMousePosition = currentMousePosition;
		currentMousePosition = Input.mousePosition;
		Vector3 cardHighlightPosition = Camera.main.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y, Camera.main.transform.position.y-5));

		if (Input.GetMouseButton(0))
		{
			RaycastHit hit = new RaycastHit();
			Ray ray = Camera.main.ScreenPointToRay(currentMousePosition);

			if (Physics.Raycast(ray, out hit))
			{
				GameCard gameCard = hit.collider.gameObject.GetComponent<GameCard>();
				if (gameCard != null)
				{
					if (selectedCard == null)
						selectedCard = gameCard;
					else
						targetedCard = gameCard;
				}
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			if (selectedCard != null && selectedCard.CardID != null)
			{
				Card card = Board.GetCard((int)selectedCard.CardID);
				if (card.PlayerID == HumanPlayer)
				{

					if (card.State == CardStates.InHand && Board.CanPlayCard(card.ID))
					{
						Board = Board.DoPlayCard(card.ID);
						UpdateState();
					}
					else if (targetedCard != null 
					         && (card.State == CardStates.OnBoard || card.State == CardStates.Hero)
					         && Board.CanAttack((int)selectedCard.CardID, (int)targetedCard.CardID))
					{
						Board = Board.DoAttack((int)selectedCard.CardID, (int)targetedCard.CardID);
						UpdateState ();
					}
					else
						ReturnCard(card, selectedCard);
				}

			}
			selectedCard = null;
		}
		if (selectedCard != null)
		{
			Card card = Board.GetCard ((int)selectedCard.CardID);
			Vector3 curr = Camera.main.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y, Camera.main.transform.position.y-5));

			if (card.PlayerID == HumanPlayer && card.State == CardStates.InHand)
				selectedCard.CurrentAnimation = new GameAnimation(selectedCard.transform, AnimationTypes.Simple, new Vector3(curr.x, 5, curr.z), Quaternion.identity, 0.1f);
		}

		if (Input.GetKeyUp(KeyCode.Space))
		{
			Board = Board.DoEndTurn();
			UpdateState();
		}
	}
	
	public void Update()
	{
		UpdateInput();
		foreach (AnimationChain ac in AnimationChainArray)
			if (ac != null && ac.IsRunning())
				ac.Continue();
	}
	
	public static void DoAnimationChain(AnimationChain Chain)
	{

	}
	

}*/
