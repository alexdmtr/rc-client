using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardLogic
{

    /* ACTIONS */
    public static BoardAction GetStartTurnAction(BoardState InitialState)
    {
        return new BoardAction("ACTION_START_TURN", null, null, null, null);
    }

    public static BoardAction GetEndTurnAction(BoardState InitialState)
    {
        return new BoardAction("ACTION_END_TURN", null, null, null, null);
    }

    public static BoardAction GetPlayCardAction(BoardState InitialState, int cardID)
    {
        return new BoardAction("ACTION_PLAY_CARD", InitialState.ActivePlayer, cardID, null, null);
    }

    public static BoardAction GetDrawCardsAction(BoardState InitialState, int count)
    {
        return new BoardAction("ACTION_TOPDECK", InitialState.ActivePlayer, null, null, count);
    }

    public static BoardAction GetAttackAction(BoardState InitialState, int actorID, int targetID)
    {
        Card ActorCard = InitialState.GetCard(actorID);
        return new BoardAction("ACTION_ATTACK", ActorCard.PlayerID, actorID, targetID, null);
    }

    public static BoardAction GetAITurnAction(BoardState InitialState)
    {
        return new BoardAction("ACTION_AI_TURN", null, null, null, null);
    }

    public static BoardAction GetStartGameAction(BoardState InitialState)
    {
        return new BoardAction("ACTION_START_GAME", null, null, null, null);
    }
    /*public static BoardState DoStartTurn(BoardState InitialState)
    {
        //DoActionDirectly(new Action("ACTION_START_TURN", ActivePlayer, null, null, null));
        return DoActionComplex(InitialState, new List<BoardAction> { new BoardAction("ACTION_START_TURN", null, null, null, null) });
    }

    public static BoardState DoEndTurn(BoardState InitialState)
    {
        //DoActionDirectly(new Action("ACTION_END_TURN", ActivePlayer, null, null, null));
        return DoActionComplex(InitialState, new List<BoardAction> { new BoardAction("ACTION_END_TURN", null, null, null, null) });
    }*/

    /*public static bool CanPlayCard(BoardState InitialState, int cardID)
    {
        return CanDoAction(InitialState, new BoardAction("ACTION_PLAY_CARD", InitialState.ActivePlayer, cardID, null, null));
    }*/

    /*public static BoardState DoPlayCard(BoardState InitialState, int cardID)
    {
        //DoActionDirectly(new Action("ACTION_PLAY_CARD", ActivePlayer, cardID, null, null));
        return DoActionComplex(InitialState, new List<BoardAction> { new BoardAction("ACTION_PLAY_CARD", InitialState.ActivePlayer, cardID, null, null) });
    }

    public static BoardState DoDrawCards(BoardState InitialState, int playerID, int count)
    {
        //DoActionDirectly(new Action("ACTION_DRAW_CARDS", ActivePlayer, null, null, 1));
        return DoActionComplex(InitialState, new List<BoardAction> { new BoardAction("ACTION_TOPDECK", InitialState.ActivePlayer, null, null, count) });
    }*/

    /*public void DoDealDamage(int actorID, int targetID, int damage)
    {
        Card ActorCard = GetCard(actorID);
        //DoActionDirectly(new Action("ACTION_DEAL_DAMAGE", ActorCard.PlayerID, actorID, targetID, damage));
    }*/

    /*public static bool CanAttack(BoardState InitialState, int actorID, int targetID)
    {
        Card ActorCard = InitialState.GetCard(actorID);
        return CanDoAction(InitialState, new BoardAction("ACTION_ATTACK", ActorCard.PlayerID, actorID, new List<int> { targetID }, null));
    }

    public static BoardState DoAttack(BoardState InitialState, int actorID, int targetID)
    {
        Card ActorCard = InitialState.GetCard(actorID);
        //DoActionDirectly(new Action("ACTION_ATTACK", ActorCard.PlayerID, actorID, targetID, null));
        return DoActionComplex(InitialState, new List<BoardAction> { new BoardAction("ACTION_ATTACK", InitialState.ActivePlayer, actorID, new List<int> { targetID } , null) });
    }*/

    /*public void DoDie(int actorID)
    {
        Card ActorCard = GetCard(actorID);
        //DoActionDirectly(new Action("ACTION_DIE", ActorCard.PlayerID, actorID, null, null));
    }*/

    public static List<BoardAction> GetAllPossibleActions(BoardState InitialState)
    {
        List<BoardAction> Actions = new List<BoardAction>();
        BoardState CurrentState = InitialState.Copy();
        int opponentID;
        if (CurrentState.ActivePlayer == 0)
            opponentID = 1;
        else
            opponentID = 0;

        List<Card> handCards = CurrentState.GetPlayerInHandCards(CurrentState.ActivePlayer);
        foreach (Card card in handCards)
        {
            BoardAction PlayCard = GetPlayCardAction(CurrentState, card.ID);
            if (CanDoAction(CurrentState, PlayCard))
                Actions.Add(PlayCard);
        }
        List<Card> boardCards = CurrentState.GetPlayerOnBoardMinions(CurrentState.ActivePlayer);
        boardCards.Add(CurrentState.GetCard(CurrentState.GetHeroID(CurrentState.ActivePlayer)));
        List<Card> opponentCards = CurrentState.GetPlayerOnBoardMinions(opponentID);
        opponentCards.Add(CurrentState.GetCard(CurrentState.GetHeroID(opponentID)));

        foreach (Card card in boardCards)
        {
            foreach (Card opponentCard in opponentCards)
            {
                BoardAction AttackAction = GetAttackAction(CurrentState, card.ID, opponentCard.ID);
                if (CanDoAction(CurrentState, AttackAction))
                    Actions.Add(AttackAction);
            }
        }

        //Actions.Add(GetEndTurnAction(CurrentState));
        return Actions;
    }

    private static double AI_GetPlayerScore(BoardState State, int playerID)
    {
        double score = 0;
        int opponentID;

        int heroHealth = State.GetCardHealth(State.GetCard(State.GetHeroID(playerID)));
        score += heroHealth * 0.5;

        if (playerID == 0)
            opponentID = 1;
        else
            opponentID = 0;

        int enemyHeroHealth = State.GetCardHealth(State.GetCard(State.GetHeroID(opponentID)));

        score = score - 0.65 * enemyHeroHealth;

        List<Card> boardCards = State.GetPlayerOnBoardMinions(playerID);
        foreach (Card card in boardCards)
        {
            int cardHealth = State.GetCardHealth(card);
            int cardAttack = State.GetCardAttack(card);

            score += 0.8 * cardHealth + 0.7 * cardAttack;
        }

        List<Card> enemyBoardCards = State.GetPlayerOnBoardMinions(opponentID);
        foreach (Card card in enemyBoardCards)
        {
            int cardHealth = State.GetCardHealth(card);
            int cardAttack = State.GetCardAttack(card);

            score = score - 2.0 * (cardAttack + cardHealth);
        }

        if (heroHealth < 1)
            score = Mathf.NegativeInfinity;
        if (enemyHeroHealth < 1)
            score = Mathf.Infinity;

        return score;
    }

    class ActionAfermath
    {
        public BoardAction Action;
        public BoardState State;
        public List<KeyValuePair<BoardAction, BoardState>> History;
        public double Score;
    }
    private static BoardState DoAITurn(BoardState InitialState, ref List<KeyValuePair<BoardAction, BoardState>> History)
    {
        BoardState CurrentState = InitialState.Copy();
        int opponentID;
        if (CurrentState.ActivePlayer == 0)
            opponentID = 1;
        else
            opponentID = 0;

        while (true)
        {
            List<BoardAction> possibleActions = GetAllPossibleActions(CurrentState);
            if (possibleActions.Count == 0)
                break;
            List<ActionAfermath> aftermaths = new List<ActionAfermath>();
            foreach (BoardAction action in possibleActions)
                if (CanDoAction(CurrentState, action))
                {
                    var newHistory = new List<KeyValuePair<BoardAction, BoardState>>(History);

                    BoardState state = DoActionComplex(CurrentState, new List<BoardAction> { action }, ref newHistory).Copy();

                    ActionAfermath aftermath = new ActionAfermath() { Action = action, History = newHistory, Score = AI_GetPlayerScore(state, CurrentState.ActivePlayer), State = state };
                    aftermaths.Add(aftermath);
                }

            aftermaths.Sort((x, y) => x.Score.CompareTo(y.Score));
            aftermaths.Reverse();


            CurrentState = aftermaths[0].State;
            History = aftermaths[0].History;

        }
        //Debug.Log("Number of aftermaths: " + aftermaths.Count);
        //foreach (var aftermath in aftermaths)
        //    Debug.Log(aftermath.Action.Type + " => " + aftermath.Score.ToString());
        //Debug.Log("Best aftermath: " + aftermaths[0].Score);
        //Debug.Log("Worst aftermath: " + aftermaths[aftermaths.Count - 1].Score);


        //List<Card> handCards = CurrentState.GetPlayerInHandCards(CurrentState.ActivePlayer);
        //foreach (Card card in handCards)
        //{
        //    BoardAction PlayCard = GetPlayCardAction(CurrentState, card.ID);
        //    if (CanDoAction(CurrentState, PlayCard))
        //        CurrentState = DoActionComplex(CurrentState, new List<BoardAction> { PlayCard }, ref History);
        //}
        //List<Card> boardCards = CurrentState.GetPlayerOnBoardMinions(CurrentState.ActivePlayer);
        //boardCards.Add(CurrentState.GetCard(CurrentState.GetHeroID(CurrentState.ActivePlayer)));

        //foreach (Card card in boardCards)
        //{
        //    BoardAction Attack = BoardLogic.GetAttackAction(CurrentState, card.ID, CurrentState.GetHeroID(opponentID));
        //    if (CanDoAction(CurrentState, Attack))
        //        CurrentState = DoActionComplex(CurrentState, new List<BoardAction> { Attack }, ref History);
        //}

        BoardAction EndTurn = GetEndTurnAction(CurrentState);
        CurrentState = DoActionComplex(CurrentState, new List<BoardAction> { EndTurn }, ref History);
        return CurrentState;
    }



    /*private void UpdateBoard()
    {
        foreach(Card card in Cards)
            if (card.State == CardStates.OnBoard && card.Health <= 0)
                DoDie (card.ID);
    }*/

    public static bool CanDoAction(BoardState InitialState, BoardAction Action)
    {
        if (Action.Type == "ACTION_DRAW_CARDS")
            return true;
        if (Action.Type == "ACTION_DEAL_DAMAGE")
            return true;
        if (Action.Type == "ACTION_START_TURN")
            return true;
        if (Action.Type == "ACTION_END_TURN")
            return true;
        if (Action.Type == "ACTION_PLAY_CARD")
        {
            Card ActorCard = InitialState.GetCard((int)Action.Actor);
            BoardState.Player Player = InitialState.Players[(int)Action.Player];
            return Player.CurrentMana >= InitialState.GetCardManaCost(ActorCard) && Action.Player == InitialState.ActivePlayer && ActorCard.State == CardStates.InHand;
        }
        if (Action.Type == "ACTION_ATTACK")
        {
            Card ActorCard = InitialState.GetCard((int)Action.Actor);
            int TargetID = (int)Action.Target;
            Card TargetCard = InitialState.GetCard(TargetID);

            if (ActorCard.CanAttack == false)
                return false;
            if (Action.Player != InitialState.ActivePlayer)
                return false;
            if (ActorCard.PlayerID == TargetCard.PlayerID)
                return false;
            if (InitialState.GetCardAttack(ActorCard) > 0 && (ActorCard.State == CardStates.Hero || ActorCard.State == CardStates.OnBoard) && (TargetCard.State == CardStates.Hero || TargetCard.State == CardStates.OnBoard))
                return true;

            return false;
        }
        if (Action.Type == "ACTION_DIE")
            return true;
        return true;
    }

    private static BoardState UpdateBoard(BoardState InitialState)
    {
        return InitialState;
    }

    public static BoardState DoActionComplex(BoardState InitialState, List<BoardAction> InitialActions, ref List<KeyValuePair<BoardAction, BoardState>> History)
    {

        List<List<BoardAction>> Actions = new List<List<BoardAction>>();
        Actions.Add(InitialActions);


        BoardState CurrentState = InitialState.Copy();

        List<BoardAction> CurrentActions = InitialActions;

        while (CurrentActions != null && CurrentActions.Count > 0)
        {
            List<BoardAction> TriggeredActions = new List<BoardAction>();
            foreach (BoardAction Action in CurrentActions)
            {
                CurrentState = CurrentState.Copy();


                /*List<Action> aux = new List<global::Action>();
                aux.Add(Action);
                History.Add(new KeyValuePair<List<Action>, BoardState>(aux, CurrentState));
                */
                int PlayerID;

                switch (Action.Type)
                {

                    case "ACTION_DRAW_CARDS":
                        /*
                         * Draws cards
                         * Player: Player who draws the cards
                         * Actor : Optional, where does this draw come from?
                         * TargetList: Cards drawn
                         * Parameter : Not used.
                         */
                        {
                            PlayerID = (int)Action.Player;
                            Card Card = CurrentState.GetCard((int)Action.Target);
                            if (Card.State == CardStates.InDeck)
                            {
                                List<Card> Deck = CurrentState.GetPlayerInDeckCards(Card.PlayerID);
                                foreach (Card CardInDeck in Deck)
                                    if (CardInDeck.Position > Card.Position)
                                        CardInDeck.Position--;
                            }
                            int numCardsInHand = CurrentState.GetPlayerInHandCards(PlayerID).Count;
                            Card.Position = numCardsInHand;
                            Card.State = CardStates.InHand;
                        }
                        break;
                    case "ACTION_TOPDECK":
                        /*
                         * Sequentally draws card from the top of the player's deck
                         * Player: Player who draws the cards
                         * Actor: Optional, where does this draw come from?
                         * TargetList: Not used.
                         * Parameter: Number of cards drawn.
                         */
                        {
                            PlayerID = (int)Action.Player;
                            int numberOfCardsDrawn = (int)Action.Parameter;

                            for (int i = 0; i < numberOfCardsDrawn; i++)
                            {
                                int? CardID = CurrentState.GetTopDeck(PlayerID);
                                if (CardID != null)
                                    CurrentState = DoActionComplex(CurrentState, new List<BoardAction> { new BoardAction("ACTION_DRAW_CARDS", PlayerID, Action.Actor, (int)CardID, null) }, ref History);
                                // if (CardID != null)
                                //  CurrentState = CurrentState.DoActionSingle(new Action("ACTION_DRAW_CARDS", PlayerID, Action.Actor, new List<int> { (int)CardID }, null), ref TriggeredActions, ref History);
                            }
                        }
                        break;
                    case "ACTION_DEAL_DAMAGE":
                        /*
                         * Simultaneously deals damage to multiple targets
                         * Player: Not used.
                         * Actor: Optional, where does this damage come from?
                         * TargetList: Cards dealt damage to
                         * Parameter: How much damage are we talking here?
                         */
                        {
                            int Damage = (int)Action.Parameter;
                            Card TargetCard = CurrentState.GetCard((int)Action.Target);
                            TargetCard.Health -= Damage;

                            //List<int> KillList = new List<int>();
                            TargetCard = CurrentState.GetCard((int)Action.Target);
                            if (TargetCard.Health <= 0)
                            {
                                // KillList.Add(TargetCard.ID);
                                if (TargetCard.State == CardStates.OnBoard)
                                    TriggeredActions.Add(new BoardAction("ACTION_KILL", Action.Player, Action.Actor, TargetCard.ID, null));
                                else if (TargetCard.State == CardStates.Hero)
                                {
                                    //Debug.Log("thus, death");
                                    CurrentState.Players[TargetCard.PlayerID].Status = GameStatus.Loss;
                                }
                            }
                            //if (KillList.Count > 0 && TargetCard.State == CardStates.OnBoard)
                            //  CurrentState = DoActionComplex(CurrentState, new List<BoardAction> { new BoardAction("ACTION_KILL", Action.Player, Action.Actor, TargetCard.ID, null) }, ref History);
                        }
                        break;
                    case "ACTION_START_TURN":
                        /*
                         * Starts the current run.
                         * Player: Not used.
                         * Actor: Not used.
                         * TargetList: Not used.
                         * Parameter: Not used.
                         */
                        {
                            if (CurrentState.Players[CurrentState.ActivePlayer].MaximumMana < 10)
                                CurrentState.Players[CurrentState.ActivePlayer].MaximumMana++;
                            CurrentState.Players[CurrentState.ActivePlayer].CurrentMana = CurrentState.Players[CurrentState.ActivePlayer].MaximumMana;
                            List<Card> minions = CurrentState.GetPlayerOnBoardMinions(CurrentState.ActivePlayer);
                            minions.Add(CurrentState.GetCard(CurrentState.GetHeroID(CurrentState.ActivePlayer)));

                            foreach (Card minion in minions)
                                minion.CanAttack = true;
                            CurrentState = DoActionComplex(CurrentState, new List<BoardAction> { new BoardAction("ACTION_TOPDECK", CurrentState.ActivePlayer, null, null, 1) }, ref History);
                            //  CurrentState = CurrentState.DoActionSingle(new Action("ACTION_TOPDECK", PlayerID, null, null, 1), ref TriggeredActions, ref History);

                            if (CurrentState.ActivePlayer == 1)
                                CurrentState = DoAITurn(CurrentState, ref History);
                        }
                        break;
                    case "ACTION_END_TURN":
                        /*
                         * Ends the current turn.
                         * Player: Not used.
                         * Actor: Not used.
                         * TargetList: Not used.
                         * Parameter: Not used.
                         */
                        {
                            if (CurrentState.ActivePlayer == 0)
                                CurrentState.ActivePlayer = 1;
                            else
                                CurrentState.ActivePlayer = 0;

                            CurrentState = DoActionComplex(CurrentState, new List<BoardAction> { new BoardAction("ACTION_START_TURN", null, null, null, null) }, ref History);
                        }
                        break;
                    case "ACTION_PLAY_CARD":
                        /*
                         * Plays a card.
                         * Player: The player whose card is being played.
                         * Actor: The card that is played.
                         * TargetList: Optional, whosoever this card targets. Passed down as paramater to triggers.
                         * Parameter: Not used.
                         */
                        {
                            PlayerID = (int)Action.Player;

                            Card ActorCard = CurrentState.GetCard((int)Action.Actor);
                            CurrentState.Players[PlayerID].CurrentMana -= CurrentState.GetCardManaCost(ActorCard);
                            List<Card> inHandCards = CurrentState.GetPlayerInHandCards(ActorCard.PlayerID);

                            foreach (Card Card in inHandCards)
                                if (Card.Position > ActorCard.Position)
                                    Card.Position--;

                            int numMinionsOnBoard = CurrentState.GetPlayerOnBoardMinions(ActorCard.PlayerID).Count;
                            ActorCard.Position = numMinionsOnBoard;
                            ActorCard.State = CardStates.OnBoard;
                            ActorCard.CanAttack = false;

                            Card HeroCard = CurrentState.GetCard(CurrentState.GetHeroID(PlayerID));
                            //HeroCard.Health -= 3;
                            //CurrentState = BoardLogic.DoActionComplex(CurrentState, new List<BoardAction>() { new BoardAction("ACTION_DEAL_DAMAGE", null, HeroCard.ID, HeroCard.ID, 3) }, ref History );
                        }
                        break;
                    case "ACTION_ATTACK":
                        /*
                         * Executes an attack, from character to character.
                         * Player: Optional.
                         * Actor: The attacker.
                         * TargetList: The defenders.
                         * Parameter: not used.
                         */
                        {
                            Card ActorCard = CurrentState.GetCard((int)Action.Actor);
                            List<BoardAction> SimultaneousDamage = new List<BoardAction>();
                            SimultaneousDamage.Add(new BoardAction("ACTION_DEAL_DAMAGE", null, Action.Actor, Action.Target, CurrentState.GetCardAttack(ActorCard)));
                            Card TargetCard = CurrentState.GetCard((int)Action.Target);
                            SimultaneousDamage.Add(new BoardAction("ACTION_DEAL_DAMAGE", null, TargetCard.ID, ActorCard.ID, CurrentState.GetCardAttack(TargetCard)));
                            CurrentState.GetCard((int)Action.Actor).CanAttack = false;

                            //CurrentState = DoActionComplex(CurrentState, SimultaneousDamage, ref History);
                            TriggeredActions.AddRange(SimultaneousDamage);

                        }
                        break;
                    case "ACTION_KILL":
                        /*
                         * Kills cards.
                         * Player: Optional.
                         * Actor: Optional, the killer.
                         * TargetList: The targets.
                         * Parameter: not used.
                         */
                        {
                            Card ActorCard = CurrentState.GetCard((int)Action.Actor);
                            Card TargetCard = CurrentState.GetCard((int)Action.Target);

                            if (TargetCard.State == CardStates.OnBoard)
                            {
                                List<Card> OnBoard = CurrentState.GetPlayerOnBoardMinions(TargetCard.PlayerID);

                                foreach (Card minion in OnBoard)
                                    if (minion.Position > TargetCard.Position)
                                        minion.Position--;
                            }

                            TargetCard.Position = null;
                            TargetCard.State = CardStates.Dead;
                        }
                        break;
                    case "ACTION_AI_TURN":
                        CurrentState = DoAITurn(CurrentState, ref History);
                        break;
                    case "ACTION_START_GAME":
                        {
                            CurrentState.ActivePlayer = 0;
                            List<BoardAction> doActions = new List<BoardAction>();

                            doActions.Add(new BoardAction("ACTION_TOPDECK", 0, null, null, 4));
                            doActions.Add(new BoardAction("ACTION_TOPDECK", 1, null, null, 4));

                            CurrentState = BoardLogic.DoActionComplex(CurrentState, doActions, ref History);
                            CurrentState = BoardLogic.DoActionComplex(CurrentState, new List<BoardAction>() { BoardLogic.GetStartTurnAction(CurrentState) }, ref History);
                            break;
                        }
                }
                History.Add(new KeyValuePair<BoardAction, BoardState>(Action, CurrentState));

            }

            CurrentActions = TriggeredActions;
        }

        return CurrentState;
    }
}
