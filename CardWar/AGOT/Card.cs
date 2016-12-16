using UnityEngine;
using System.Collections;
using System.Data;
using System.Text;
using System;
using CardWar.Models;

[Serializable()]
public enum CardStates { Dead, InDeck, InHand, OnBoard, Hero };
[Serializable()]
public class Card
{
    public int ID;
    public string Type;
    public CardStates State;
    public int PlayerID;
    public int? Position;
    public int Health;
    public bool CanAttack = true;
    private CardModel CardData;


    public Card(int ID, string Type, int PlayerID, CardStates State, int? Position)
    {
        this.ID = ID;
        this.Type = Type;
        this.PlayerID = PlayerID;
        this.State = State;
        this.Position = Position;

        StringBuilder QueryText = new StringBuilder(@"SELECT * FROM Cards WHERE TYPE='");
        QueryText.Append(Type);
        QueryText.Append(@"'");
        CardData = Db.FindCard(this.Type);

        if (CardData == null)
            throw new Exception(String.Format("Card {0} not found", this.Type));

        this.Health = GetBaseMaxHealth();
    }

    //private object GetColumn(string columnName)
    //{
    //    return CardData.Rows[0][columnName];
    //}

    public int GetBaseManaCost()
    {
        return CardData.Mana;
        //return (int)(GetColumn("Mana"));
    }

    public int GetBaseAttack()
    {
        return CardData.Attack;
        //return (int)(GetColumn("Attack"));
    }

    public int GetBaseMaxHealth()
    {
        return CardData.Health;
        //return (int)(GetColumn("Health"));
    }

    public string GetCardType()
    {
        return CardData.CardType;
        //return (string)(GetColumn("CardType"));
    }

    public string GetCardGFX()
    {
        return CardData.CardGFX;
        //return (string)(GetColumn("CardGFX"));
    }

    public string GetName()
    {
        return CardData.Name;
        //return (string)(GetColumn("Name"));
    }

    public Card Copy()
    {
        Card newCard = new Card(ID, Type, PlayerID, State, Position);
        newCard.Health = Health;
        newCard.CanAttack = CanAttack;
        return newCard;
    }
}