using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Game : MonoBehaviour
{

    public GameObject CardPrefab;
    public GameObject ManaCrystalPrefab;
    public Camera MainCamera;
    public HeroName HeroName0;
    public HeroName HeroName1;

    public void ShuffleDeck(int PlayerID)
    {
        List<Card> Deck = Board.GetPlayerInDeckCards(PlayerID);
        List<int> ordered = new List<int>(Deck.Count);
        for (int i = 0; i < Deck.Count; i++)
            ordered.Add(i);

        List<int> shuffle = new List<int>();
        while (ordered.Count > 0)
        {
            int indx = UnityEngine.Random.Range(0, ordered.Count);
            shuffle.Add(ordered[indx]);
            ordered.RemoveAt(indx);
        }

        for (int i = 0; i < Deck.Count; i++)
            Deck[i].Position = shuffle[i];
    }
    // Use this for initialization
    void Start()
    {
        Class = new string[2];
        Class[0] = "CLASS_LANNISTER";
        Class[1] = "CLASS_STARK";
        CardArray = new Dictionary<int, CardHandler>();
        ManaCrystals = new List<GameObject>();
        Board = new BoardState();
        Board.AddPlayer(0);
        Board.AddPlayer(1);
        Board.AddCard("CARD_ROBB_STARK", 1, CardStates.Hero, null);
        Board.AddCard("CARD_CERSEI_LANNISTER", 0, CardStates.Hero, null);
        for (int i = 0; i < 4; i++)
            Board.AddCard("CARD_STARK_CAVALRY", 1, CardStates.InDeck, i);
        for (int i = 2; i < 10; i++)
            Board.AddCard("CARD_STARK_FOOTMAN", 1, CardStates.InDeck, i);
        for (int i = 0; i < 10; i++)
            Board.AddCard("CARD_LANNISTER_FOOTMAN", 0, CardStates.InDeck, i);
        for (int i = 0; i < 4; i++)
            Board.AddCard("CARD_LANNISTER_CAVALRY", 0, CardStates.InDeck, i);
        //Board.AddCard("CARD_CHELLA", 0, CardStates.InDeck, 7);
        //Board.AddCard("CARD_SHAGGA", 0, CardStates.InDeck, 8);
        //Board.AddCard("CARD_TIMETT", 0, CardStates.InDeck, 9);
        Board.AddCard("CARD_JAIME_LANNISTER", 0, CardStates.InDeck, 0);
        Board.AddCard("CARD_THEON_GREYJOY", 1, CardStates.OnBoard, 0);
        var History = new List<KeyValuePair<BoardAction, BoardState>>();
        PreppedBoardStates = new List<ActionData>();

        ShuffleDeck(0);
        ShuffleDeck(1);
        Board = BoardLogic.DoActionComplex(Board, new List<BoardAction> { BoardLogic.GetStartGameAction(Board) }, ref History);
        //QueueAction(BoardLogic.GetStartGameAction(Board));
        NeedsFullUpdate = true;
        AwesomenessCounter = 0;

        /*finalBoardState = Board;
        StartWork();*/
    }
    bool NeedsFullUpdate;



    public KeyValuePair<Vector3, Quaternion> GetCardCoords(BoardState Board, Card card)
    {
        int CardID = card.ID;
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        if (card.State == CardStates.InDeck)
        {
            float cardX, cardY, cardZ;
            float distanceY = 5f;

            cardX = 900;
            cardZ = card.PlayerID == 0 ? -800f : 800f;
            if (card.Position == null)
                Debug.Log("Error. Card " + CardID.ToString() + " has no position.");
            else
            {
                cardY = 5f;
                cardY += distanceY * (int)card.Position;


                position = new Vector3(cardX, cardY, cardZ);
                rotation = Quaternion.Euler(new Vector3(270, 180, 0));
            }
        }
        if (card.State == CardStates.InHand)
        {
            float cardX, cardY, cardZ;
            float centerX = 0f, centerY = 50f, centerZ = card.PlayerID == 0 ? -1200f : 1200f;
            float radiusX = 1200f, radiusZ = 300f;
            float radiusY = 30f;
            float angleBetweenCards = 9;
            float angle;

            List<Card> PlayerCards = Board.GetPlayerInHandCards(card.PlayerID);
            if (card.Position == null)
                Debug.Log("Error. Card " + CardID.ToString() + " has no position.");
            else
            {
                angle = 90 + angleBetweenCards * (PlayerCards.Count / 2.0f);
                angle -= angleBetweenCards * (int)card.Position;

                cardX = centerX + (card.PlayerID == 0 ? 1 : -1) * radiusX * Mathf.Cos(angle / 180f * Mathf.PI);
                cardZ = centerZ + (card.PlayerID == 0 ? 1 : -1) * radiusZ * Mathf.Sin(angle / 180f * Mathf.PI);
                cardY = centerY + radiusY * (int)card.Position;
                //cardY = Mathf.Sin (angle / 180f * Mathf.PI) * 5;

                position = new Vector3(cardX, cardY, cardZ);
                rotation = Quaternion.Euler(new Vector3((card.PlayerID == 0?90:270), 90-angle + (card.PlayerID == 0? 0 : 180), 0));
            }
        }
        if (card.State == CardStates.OnBoard)
        {
            float minionX, minionY, minionZ;
            float distanceBetweenMinions = 250f;
            minionY = 50f;
            if (card.PlayerID == 0)
                minionZ = -200f;
            else
                minionZ = 200f;

            List<Card> PlayerMinions = Board.GetPlayerOnBoardMinions(card.PlayerID);

            if (card.Position == null)
                Debug.Log("Error. Minion " + CardID.ToString() + " has no position.");
            else
            {
                minionX = -(PlayerMinions.Count-1) * distanceBetweenMinions / 2;
                minionX += ((int)card.Position) * distanceBetweenMinions;
                
                position = new Vector3(minionX, minionY, minionZ);
                rotation = Quaternion.Euler(90, 0, 0);
            }
        }
        if (card.State == CardStates.Hero)
        {
            float heroX, heroY, heroZ;
            heroY = 50f;
            heroX = 0f;
            if (card.PlayerID == 0)
                heroZ = -550f;
            else
                heroZ = 550f;

            position = new Vector3(heroX, heroY, heroZ);
            rotation = Quaternion.Euler(90, 0, 0);
        }

        return new KeyValuePair<Vector3, Quaternion>(position, rotation);
    }

    public Dictionary<int, CardHandler> CardArray;
    public List<GameObject> ManaCrystals;
    public AnimationChain[] AnimationChainArray;
    public BoardState Board;
    public int HumanPlayer;
    public string[] Class;


    private CardHandler CreateCard(BoardState board, Card card)
    {
        KeyValuePair<Vector3, Quaternion> kvp = GetCardCoords(board, card);
        //Debug.Log(card.ID);
        CardHandler CardHandler = ((GameObject)GameObject.Instantiate(CardPrefab, kvp.Key, kvp.Value)).GetComponent<CardHandler>();
        //Debug.Log(card.ID);
        CardHandler.CardID = card.ID;

        CardHandler.UpdateCardTexture(false, board, card, card.State, Class[card.PlayerID]);
        CardHandler.transform.parent = this.transform;
        return CardHandler;
    }


    public void UpdateManaCrystals()
    {
        foreach (GameObject crystal in ManaCrystals)
            GameObject.Destroy(crystal);
        ManaCrystals.Clear();

        for (int i = 0; i < Board.Players.Count; i++)
        {
            float crystalX, crystalY, crystalZ;
            float crystalDistance = 100f;
            crystalY = 50f;
            crystalX = +500;
            if (i == 0)
                crystalZ = -475f;
            else
                crystalZ = +475f;

            for (int j = 0; j < Board.Players[i].CurrentMana; j++)
            {
                ManaCrystals.Add((GameObject)GameObject.Instantiate(ManaCrystalPrefab, new Vector3(crystalX + crystalDistance * j, crystalY, crystalZ), Quaternion.identity));
                ManaCrystals[j].transform.parent = this.transform;
            }
        }
    }

    IEnumerator UpdateStatePartial()
    {
        foreach (Card card in Board.Cards)
        {
            int cardID = card.ID;
            if (CardArray.ContainsKey(cardID))
            {
                CardArray[cardID].UpdateCardTexture(false, Board, card, card.State, Class[card.PlayerID]);
            }
            else
                CardArray[cardID] = CreateCard(Board, card);
            //yield return null;
        }
        UpdateManaCrystals();
        yield return null;
    }

    IEnumerator UpdateStateFull()
    {
        foreach (Card card in Board.Cards)
        {
            int cardID = card.ID;
            if (CardArray.ContainsKey(cardID))
            {
                CardArray[cardID].UpdateCardTexture(false, Board, card, card.State, Class[card.PlayerID]);
                ReturnCard(Board, card, CardArray[cardID]);
            }
            else
                CardArray[cardID] = CreateCard(Board, card);
            //yield return null;
        }
        UpdateManaCrystals();
        yield return null;
    }

    public Game(GameObject CardPrefab, GameObject ManaCrystalPrefab)
    {
        previousMousePosition = currentMousePosition = Vector3.zero;

        this.CardPrefab = CardPrefab;
        this.ManaCrystalPrefab = ManaCrystalPrefab;

        Board = new BoardState();
        UpdateStateFull();
    }

    CardHandler selectedCard;
    CardHandler hoveredCard;
    Vector3 previousMousePosition;
    Vector3 currentMousePosition;

    void ReturnCard(BoardState Board, Card card, CardHandler CardHandler)
    {
        CardHandler.CurrentAnimation = GetCardReturnAnimation(Board, card, CardHandler);
    }

    void ReturnCardIfNotNullAndInState(CardHandler CardHandler, CardStates State)
    {
        if (CardHandler != null)
        {
            Card c = Board.GetCard(CardHandler.CardID);
            if (c.State == State)
                ReturnCard(Board, Board.GetCard(CardHandler.CardID), CardHandler);
        }
    }

    CardHandler DetailedCard;
    float timeHovered;
    void UpdateInput()
    {
        previousMousePosition = currentMousePosition;
        currentMousePosition = Input.mousePosition;
        Vector3 cardHighlightPosition = MainCamera.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y, MainCamera.transform.position.y - 5));

        RaycastHit hit = new RaycastHit();
        Ray ray = MainCamera.ScreenPointToRay(currentMousePosition);
        

        if (Physics.Raycast(ray, out hit))
        {
            CardHandler CardHandler = hit.collider.gameObject.GetComponent<CardHandler>();
            if (CardHandler != null)
            {
                //ReturnCardIfNotNullAndInState(hoveredCard, CardStates.InHand);
                
                if (CardHandler != hoveredCard)
                    timeHovered = 0;
                else if (selectedCard == null)
                    timeHovered += Time.deltaTime;
                hoveredCard = CardHandler;
                
                if (Input.GetMouseButton(0))
                {
                    if (selectedCard == null)
                        selectedCard = CardHandler;
                }
            }
            else
            {
                hoveredCard = null;
                //ReturnCardIfNotNullAndInState(hoveredCard, CardStates.InHand);
            }

        }
        else
        {
            hoveredCard = null;
            //ReturnCardIfNotNullAndInState(hoveredCard, CardStates.InHand);
        }
       
        if (Input.GetMouseButtonUp(0))
        {
            if (selectedCard != null)
            {
                Card card = Board.GetCard((int)selectedCard.CardID);
                if (card.PlayerID == HumanPlayer)
                {
                    BoardAction PlayCard = BoardLogic.GetPlayCardAction(Board, card.ID);
                    if (card.State == CardStates.InHand && BoardLogic.CanDoAction(Board, PlayCard))
                    {
                        QueueAction(PlayCard);
                        //StartWork(BoardLogic.GetPlayCardAction(Board, card.ID));
                        //Board = BoardLogic.DoPlayCard(Board, card.ID);
                        //UpdateState();
                    }
                    else if (hoveredCard != null
                                && (card.State == CardStates.OnBoard || card.State == CardStates.Hero))
                               
                    {
                        BoardAction Attack = BoardLogic.GetAttackAction(Board, (int)selectedCard.CardID, (int)hoveredCard.CardID);
                        if (BoardLogic.CanDoAction(Board, Attack))
                            QueueAction(Attack);
                        //StartWork(BoardLogic.GetAttackAction(Board, (int)selectedCard.CardID, (int)targetedCard.CardID));
                        //Board = BoardLogic.DoAttack(Board, (int)selectedCard.CardID, (int)targetedCard.CardID);
                        //UpdateState();
                    }
                    else
                        ReturnCard(Board, card, selectedCard);
                }

            }
            selectedCard = null;
        }
        
        if (selectedCard != null)
        {
            Card card = Board.GetCard(selectedCard.CardID);
            Vector3 curr = MainCamera.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y, MainCamera.transform.position.y-50));

            if (card.PlayerID == HumanPlayer && card.State == CardStates.InHand)
                selectedCard.CurrentAnimation = new GameAnimation(selectedCard.transform, AnimationTypes.Simple, new Vector3(curr.x, 500, curr.z), Quaternion.Euler(90, 0, 0), 0.1f);
        }
        else if (hoveredCard != null)
        {
            Card card = Board.GetCard(hoveredCard.CardID);
            if (DetailedCard == null)
            {
                DetailedCard = ((GameObject)GameObject.Instantiate(CardPrefab, new Vector3(-450, 850, -130), Quaternion.Euler(90, 0, 0))).GetComponent<CardHandler>();
                
            }
            DetailedCard.UpdateCardTexture(true, Board, card, CardStates.InHand, Class[card.PlayerID]);
            /*(Card card = Board.GetCard(hoveredCard.CardID);
            KeyValuePair<Vector3, Quaternion> pos = GetCardCoords(Board, card);
            Vector3 forcedMousePosition = MainCamera.WorldToScreenPoint(new Vector3(pos.Key.x, pos.Key.y, pos.Key.z+500));
            Vector3 curr = MainCamera.ScreenToWorldPoint(new Vector3(forcedMousePosition.x, forcedMousePosition.y, MainCamera.transform.position.y - 5));

           
            if (card.PlayerID == HumanPlayer && card.State == CardStates.InHand)
                hoveredCard.CurrentAnimation = new GameAnimation(hoveredCard.transform, AnimationTypes.Simple, new Vector3(curr.x, 750, curr.z), Quaternion.Euler(90, 0, 0), 0.0f);
            */
        }

        if (DetailedCard != null && hoveredCard != null)
        {
            Card x = (Board.GetCard(hoveredCard.CardID));
            DetailedCard.gameObject.SetActive(x.State != CardStates.InDeck && !(x.State == CardStates.InHand && x.PlayerID == 1) && hoveredCard != null && selectedCard == null && ((timeHovered > 0.5) || x.State == CardStates.InHand));
        }
        else if (hoveredCard == null)
            if (DetailedCard != null)
                DetailedCard.gameObject.SetActive(false);

        //if (Input.GetKeyUp(KeyCode.Space))
        //{

        //StartWork(BoardLogic.GetEndTurnAction(Board));
        // Board = BoardLogic.DoEndTurn(Board);
        //UpdateState();
        //}
    }

    UnityThreading.ActionThread BoardLogicThread;


    public bool Working;
    public bool NeedsUpdate;
    public bool Pending;

    GameAnimation GetCardReturnAnimation(BoardState Board, Card card, CardHandler CardHandler, float time = 0.5f)
    {
        return new GameAnimation(CardHandler.transform, AnimationTypes.Simple, GetCardCoords(Board, card).Key, GetCardCoords(Board, card).Value, time);
    }

    public AnimationChain CurrentAnimationChain;



    public void QueueAction(BoardAction Action)
    {
        if (Board.Players[0].Status == GameStatus.Loss || Board.Players[1].Status == GameStatus.Loss)
            return;
        if (CurrentAnimationChain != null && CurrentAnimationChain.IsRunning())
            return;
        if (!Pending)
        {
            Pending = true;
            ToDoAction = Action;
        }
    }

    int AwesomenessCounter;
    bool needsWorkDone;

    public BoardAction ToDoAction;
    public ActionData CurrentActionData;
    int currentActionIndex;
    public BoardState finalBoardState;

    public void UpdateAnimations()
    {
        bool isRunning = false;
        if (CurrentAnimationChain != null && CurrentAnimationChain.IsRunning())
        {
            CurrentAnimationChain.Continue();
            if (CurrentAnimationChain.IsRunning())
                isRunning = true;
        }
        if (!isRunning)
        {
            if (CurrentActionData != null && CurrentActionData.History != null && currentActionIndex >= 0 && currentActionIndex < CurrentActionData.History.Count)
            {
                CurrentAnimationChain = new AnimationChain();

                BoardAction Action = CurrentActionData.History[currentActionIndex].Key;
                BoardState State = CurrentActionData.History[currentActionIndex].Value;

                //Debug.Log(currentActionIndex.ToString() + Action.Type);



                if (Action.Type == "ACTION_ATTACK")
                {
                    Card Actor = State.GetCard((int)Action.Actor);
                    Card Target = State.GetCard((int)Action.Target);

                    CardHandler ActorCardHandler = CardArray[Actor.ID];
                    CardHandler TargetCardHandler = CardArray[Target.ID];

                    GameAnimation AttackAnimation = new GameAnimation(ActorCardHandler.transform, AnimationTypes.Simple, ActorCardHandler.transform.position + new Vector3(0, 150f, 0), ActorCardHandler.transform.rotation, 0.1f);
                    CurrentAnimationChain.Enqueue(AttackAnimation);

                    Vector3 AttackPosition = new Vector3(TargetCardHandler.transform.position.x, TargetCardHandler.transform.position.y + 50f, TargetCardHandler.transform.position.z);
                    
                    AttackAnimation = new GameAnimation(ActorCardHandler.transform, AnimationTypes.Simple, AttackPosition, TargetCardHandler.transform.rotation, 0.2f);
                    CurrentAnimationChain.Enqueue(AttackAnimation);
                }

                List<GameAnimation> SimultaneousAnimations = new List<GameAnimation>();

                List<Card> AffectedCards = new List<Card>();
                if (Action.Type != "ACTION_ATTACK")
                {
                    if (Action.Actor != null)
                        AffectedCards.Add(State.GetCard((int)Action.Actor));
                    if (Action.Target != null)
                        AffectedCards.Add(State.GetCard((int)Action.Target));
                }


                switch (Action.Type)
                {
                    case "ACTION_DRAW_CARDS":
                        AffectedCards.AddRange(State.GetPlayerInHandCards((int)Action.Player));
                        break;
                    case "ACTION_PLAY_CARD":
                        AffectedCards.AddRange(State.GetPlayerInHandCards((int)Action.Player));
                        AffectedCards.AddRange(State.GetPlayerOnBoardMinions((int)Action.Player));
                        break;
                    case "ACTION_KILL":
                        AffectedCards.AddRange(State.GetPlayerOnBoardMinions(0));
                        AffectedCards.AddRange(State.GetPlayerOnBoardMinions(1));
                        break;

                }

                float time = 0.25f;
                if (Action.Type == "ACTION_DRAW_CARDS")
                    time = 0.5f;
                //if (Action.Type == "ACTION_DEAL_DAMAGE" || Action.Type == "ACTION_DIE")
                  //  time = 0f;

                
                foreach (Card Card in AffectedCards)
                    if (CardArray.ContainsKey(Card.ID))
                        SimultaneousAnimations.Add(GetCardReturnAnimation(State, Card, CardArray[Card.ID], time));

                CurrentAnimationChain.Enqueue(SimultaneousAnimations);


                isRunning = true;
                currentActionIndex++;

                Board = State;
                NeedsUpdate = true;
            }
        }
        else if (CurrentActionData.History != null && currentActionIndex == CurrentActionData.History.Count)
        {
            NeedsFullUpdate = true;
            currentActionIndex++;
        }
    }

    public void WindowFunction(int WindowID)
    {
        string text = "";

        Vector2 size = new Vector2(Screen.width * 0.2f, Screen.height * 0.2f);
        if (WindowID == 0)
            text = "Congratulations, you have won!\nClick here to return to the mapp.";
        if (WindowID == 1)
            text = "You have lost. Better luck next time!\nClick here to return to the map.";

        if (GUI.Button(new Rect(0, 0.1f * size.y, size.x, size.y * 0.9f), text))
        {
            Application.LoadLevel("MapScene");
            Global.State = GlobalStates.Map;
            Destroy(Global.CurrentGame.gameObject);
            Global.CurrentGame = null;
        }
    }

    public void OnGUI()
    {
        int VictoryStatus = 0;
        if (Board.Players[0].Status == GameStatus.Loss)
            VictoryStatus = -1;
        if (Board.Players[1].Status == GameStatus.Loss)
            VictoryStatus = +1;

        Vector2 size = new Vector2(Screen.width * 0.2f, Screen.height * 0.2f);
        if (VictoryStatus == 1)
        {
            GUI.Window(0, new Rect(Screen.width / 2 - size.x / 2, Screen.height / 2 - size.y / 2, size.x, size.y), WindowFunction, "<color=YELLOW>Victory!</color>");
        }
        else if (VictoryStatus == -1)
        {
            GUI.Window(1, new Rect(Screen.width / 2 - size.x / 2, Screen.height / 2 - size.y / 2, size.x, size.y), WindowFunction, "<color=RED>Defeat...</color>");

        }
        else
        {
            string s = Board.ActivePlayer == 0 ? "End Turn" : "Opponent Turn";
            if (GUI.Button(new Rect(Screen.width-150, Screen.height / 2 - 25, 150, 50), s))
            {
                if (Board.ActivePlayer == 0)
                    QueueAction(BoardLogic.GetEndTurnAction(Board));
            }
            /*if (GUI.Button(new Rect(Screen.width-75, Screen.height - 50, 100, 50), "Help"))
            {
                Global.SwitchToMenu();
            }*/
        }
    }
    public void DoAction()
    {
        //Debug.Log(Action.Type + ", Active Player: " + Board.ActivePlayer);
        //BoardState newBoard = Board.Copy();
        bool ok = false;
        //Debug.Log(PreppedBoardStates.Count);

        if (!Working && !needsWorkDone)
        {
            for (int i = 0; i < PreppedBoardStates.Count; i++)
            {
                if (PreppedBoardStates[i].Action == ToDoAction)
                {
                    CurrentActionData = PreppedBoardStates[i];
                    ok = true;
                    break;
                }
            }
        }

        if (!ok)
        {
            //Debug.Log("Couldn't find action.");
            var History = new List<KeyValuePair<BoardAction, BoardState>>();
            CurrentActionData = new ActionData();
            CurrentActionData.Action = ToDoAction;
            Debug.Log(ToDoAction.Type);
            CurrentActionData.FinalState = BoardLogic.DoActionComplex(Board, new List<BoardAction> { ToDoAction }, ref History);
            CurrentActionData.History = History;

            //CurrentAnimationChain = GetAnimationFromHistory(History);
        }

        //Debug.Log(CurrentAnimationChain != null && CurrentAnimationChain.IsRunning());
        currentActionIndex = 0;
        //Board = CurrentActionData.FinalState;
        Pending = false;
        //NeedsFullUpdate = true;
        //CurrentAnimationChain = GetAnimationFromHistory(ActionData.History);
        //NeedsUpdate = true;

        //Debug.Log(AwesomenessCounter);
        //AwesomenessCounter++;
        //Debug.Log("Depth: " + depth + " , Active Player: " + Board.ActivePlayer);

        /*object o = GameNetworking.SendAndReceive(CurrentActionData);
        ActionData A = (ActionData)o;
        if (A == null)
            Debug.Log("nope, it's null");
        else
        {
            Debug.Log("It worked!!!");
            CurrentActionData = A;
        }*/

        //Debug.Log(((Socket_Helper.SocketHelper.MessagesForServer)GameNetworking.SendAndReceive( Socket_Helper.SocketHelper.MessagesForServer.HelloWorld)).ToString());
        finalBoardState = CurrentActionData.FinalState;
        needsWorkDone = true;
        //StartWork();

    }



    public void StartWork()
    {
        if (!Working)
        {
            //Debug.Log("Working on board logic.");

            BoardLogicThread = UnityThreadHelper.CreateThread((System.Action)Work);
        }
    }

    public List<ActionData> PreppedBoardStates;

    public void Work()
    {
        Working = true;
        if (PreppedBoardStates != null)
            PreppedBoardStates.Clear();
        else
            PreppedBoardStates = new List<ActionData>();
        List<BoardAction> PossibleActions = BoardLogic.GetAllPossibleActions(finalBoardState);
        foreach (BoardAction Action in PossibleActions)
        {
            List<KeyValuePair<BoardAction, BoardState>> History = new List<KeyValuePair<BoardAction, BoardState>>();
            BoardState FinalState = BoardLogic.DoActionComplex(finalBoardState, new List<BoardAction> { Action }, ref History);
            PreppedBoardStates.Add(new ActionData(Action, FinalState, History));
        }
        //PreppedBoardStates.Add(new KeyValuePair<BoardAction,BoardState>(Action, BoardLogic.DoActionComplex(Board, new List<BoardAction> { Action }, ref History)));
        //BoardState newBoard = BoardLogic.DoActionComplex(Board, new List<BoardAction> { MyWork });
        Working = false;
        /*Board = newBoard;
        NeedsUpdate = true;*/
    }
    public void Update()
    {
        int VictoryStatus = 0;
        if (Board.Players[0].Status == GameStatus.Loss)
            VictoryStatus = -1;
        if (Board.Players[1].Status == GameStatus.Loss)
            VictoryStatus = +1;
        if (needsWorkDone && !Working)
        {
            needsWorkDone = false;
            StartWork();
        }
        if (Pending)
        {
            DoAction();
        }
        if (NeedsFullUpdate)
        {
            StartCoroutine("UpdateStateFull");
            NeedsFullUpdate = false;
            NeedsUpdate = false;
        }
        if (NeedsUpdate)
        {
            //Debug.Log("Updating visuals.");
            StartCoroutine("UpdateStatePartial");
            NeedsUpdate = false;
        }
        UpdateAnimations();

        if (VictoryStatus == 0)
            UpdateInput();

        /*foreach (AnimationChain ac in AnimationChainArray)
            if (ac != null && ac.IsRunning())
                ac.Continue();*/
    }

    /*public static void DoAnimationChain(AnimationChain Chain)
    {

    }*/
}
