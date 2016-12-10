using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

[Serializable()]
public enum GameStatus { InPlay, Loss };

[Serializable()]
public class BoardState
{
    [Serializable()]
    public class Player
    {
        public int ID;
        public int CurrentMana;
        public int MaximumMana;
        public string Cardback;
        public GameStatus Status;

        public Player(int ID)
        {
            this.ID = ID;

            CurrentMana = 0;
            MaximumMana = 0;
            Status = GameStatus.InPlay;
        }

        public Player Copy()
        {
            Player newPlayer = new Player(ID);
            newPlayer.CurrentMana = CurrentMana;
            newPlayer.MaximumMana = MaximumMana;
            newPlayer.Status = Status;
            return newPlayer;
        }
    }

    public List<Player> Players;
    public List<Card> Cards;
    public int ActivePlayer;
    public int CurrentTurn;
    public int NewCardUID;

    public BoardState()
    {
        Players = new List<Player>();
        Cards = new List<Card>();
        ActivePlayer = 0;

        CurrentTurn = 0;
        NewCardUID = 0;
    }

    public BoardState Copy()
    {
        BoardState newBoardState = new BoardState();

        foreach (Player player in Players)
            newBoardState.Players.Add(player.Copy());
        foreach (Card card in Cards)
            newBoardState.Cards.Add(card.Copy());
        newBoardState.ActivePlayer = ActivePlayer;
        newBoardState.CurrentTurn = CurrentTurn;

        return newBoardState;
    }

    public Card AddCard(string Type, int PlayerID, CardStates State, int? Position)
    {
        Card Card = new Card(NewCardUID, Type, PlayerID, State, Position);

        Cards.Add(Card);

        NewCardUID++;

        return Card;
    }

    public void AddPlayer(int PlayerID)
    {
        Player Player = new Player(PlayerID);
        Players.Add(Player);
    }

    public Card GetCard(int CardID)
    {
        Card Card = Cards[CardID];//= Cards.Find(x => x.ID == CardID);
        return Card;
    }

    public List<Card> GetPlayerOnBoardMinions(int PlayerID)
    {
        List<Card> minions = Cards.FindAll(x => x.PlayerID == PlayerID && x.State == CardStates.OnBoard);
        return minions;
    }

    public List<Card> GetPlayerInHandCards(int PlayerID)
    {
        List<Card> cards = Cards.FindAll(x => x.PlayerID == PlayerID && x.State == CardStates.InHand);
        return cards;
    }

    public List<Card> GetPlayerInDeckCards(int PlayerID)
    {
        List<Card> cards = Cards.FindAll(x => x.PlayerID == PlayerID && x.State == CardStates.InDeck);
        return cards;
    }

    public int GetHeroID(int PlayerID)
    {
        Card Card = Cards.Find(x => x.PlayerID == PlayerID && x.State == CardStates.Hero);
        return Card.ID;
    }

    /* STATUSES */
    public int GetCardManaCost(Card card)
    {
        return card.GetBaseManaCost();
    }

    public int GetCardAttack(Card card)
    {
        return card.GetBaseAttack();
    }

    public int GetCardMaxHealth(Card card)
    {
        return card.GetBaseMaxHealth();
    }

    public int GetCardHealth(Card card)
    {
        return card.Health;
    }

    public int? GetTopDeck(int PlayerID)
    {
        List<Card> cards = GetPlayerInDeckCards(PlayerID).OrderBy(x => (int)x.Position).ToList();
        if (cards.Count == 0)
        {
            Debug.Log("Cannot get topdeck, deck empty.");
            return null;
        }
        return cards[cards.Count - 1].ID;
    }



    /*private void DoActionDirectly(Action Action)
    {
        if (Action.Type == "ACTION_DRAW_CARDS")
        {
            int numberOfCards;
            if (Action.Parameter != null)
                numberOfCards = (int)Action.Parameter;
            else
                numberOfCards = 1;

            for (int i = 1; i <= numberOfCards; i++)
            {
                int ? topDeckID = GetTopDeck((int)Action.Player);
                if (topDeckID != null)
                {
                    Card topDeckCard = GetCard((int)topDeckID);
                    int numCardsInHand = GetPlayerInHandCards((int)Action.Player).Count;
                    topDeckCard.Position = numCardsInHand;
                    topDeckCard.State = CardStates.InHand;
                }
            }
        }
        if (Action.Type == "ACTION_DEAL_DAMAGE")
        {
            Card ActorCard = GetCard((int) Action.Actor);
            Card TargetCard = GetCard ((int) Action.Target);
            int Damage = (int)Action.Parameter;

            TargetCard.Health -= Damage;
        }
        if (Action.Type == "ACTION_START_TURN")
        {
            ActivePlayer = (int)Action.Player;
            DoDrawCards(ActivePlayer, 1);

            Players[ActivePlayer].MaximumMana++;
            Players[ActivePlayer].CurrentMana = Players[ActivePlayer].MaximumMana;
            List<Card> minions = GetPlayerOnBoardMinions(ActivePlayer);
            minions.Add(GetCard(GetHeroID(ActivePlayer)));

            foreach(Card minion in minions)
                minion.CanAttack = true;
        }
        if (Action.Type == "ACTION_END_TURN")
        {
            if (Action.Player == 0)
                ActivePlayer = 1;
            else
                ActivePlayer = 0;

            DoStartTurn();
            if (ActivePlayer == 1)
                DoAITurn();

        }
        if (Action.Type == "ACTION_PLAY_CARD")
        {
            Card ActorCard = GetCard((int) Action.Actor);
            if (ActorCard.GetCardType() == "CARDTYPE_CHARACTER")
            {
                Players[(int)Action.Player].CurrentMana -= GetCardManaCost(ActorCard);

                int numCardsOnBoard = GetPlayerOnBoardMinions((int)Action.Player).Count;
                List<Card> cards = GetPlayerInHandCards((int)Action.Player);
                foreach(Card card in cards)
                    if (card.Position > ActorCard.Position)
                        card.Position--;

                ActorCard.Position = numCardsOnBoard;
                ActorCard.State = CardStates.OnBoard;
                ActorCard.CanAttack = false;


            }
        }
        if (Action.Type == "ACTION_ATTACK")
        {
            Card ActorCard = GetCard((int) Action.Actor);
            Card TargetCard = GetCard ((int) Action.Target);

            int toDamage = GetCardAttack(ActorCard);
            int fromDamage = GetCardAttack(TargetCard);

            DoDealDamage(ActorCard.ID, TargetCard.ID, toDamage);
            DoDealDamage(TargetCard.ID, ActorCard.ID, fromDamage);

            ActorCard.CanAttack = false;
        }
        if (Action.Type == "ACTION_DIE")
        {
            Card ActorCard = GetCard((int) Action.Actor);

            if (ActorCard.State == CardStates.OnBoard)
            {
                List<Card> cards = GetPlayerOnBoardMinions((int)Action.Player);
                foreach(Card card in cards)
                    if (card.Position > ActorCard.Position)
                        card.Position--;

                ActorCard.Position = null;
                ActorCard.State = CardStates.Dead;
            }
        }
        UpdateBoard();
        
    }*/

}
