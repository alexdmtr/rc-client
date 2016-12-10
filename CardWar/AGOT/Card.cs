using UnityEngine;
using System.Collections;
using System.Data;
using System.Text;
using System;

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
    private DataTable CardData;


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
        CardData = SQL.Query(QueryText.ToString());

        this.Health = GetBaseMaxHealth();
    }

    private object GetColumn(string columnName)
    {
        return CardData.Rows[0][columnName];
    }

    public int GetBaseManaCost()
    {
        return (int)(GetColumn("Mana"));
    }

    public int GetBaseAttack()
    {
        return (int)(GetColumn("Attack"));
    }

    public int GetBaseMaxHealth()
    {
        return (int)(GetColumn("Health"));
    }

    public string GetCardType()
    {
        return (string)(GetColumn("CardType"));
    }

    public string GetCardGFX()
    {
        return (string)(GetColumn("CardGFX"));
    }

    public string GetName()
    {
        return (string)(GetColumn("Name"));
    }

    public Card Copy()
    {
        Card newCard = new Card(ID, Type, PlayerID, State, Position);
        newCard.Health = Health;
        newCard.CanAttack = CanAttack;
        return newCard;
    }
}