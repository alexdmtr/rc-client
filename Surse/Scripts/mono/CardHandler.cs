using UnityEngine;
using System.Collections;
using System.Text;

public class CardHandler : MonoBehaviour {

    public CardHand Hand;
    public CardHero Hero;
    public CardMinion Minion;
    public int CardID;
    public GameAnimation CurrentAnimation;

	// Use this for initialization
	void Start () {
	
	}

    public void UpdateCardTexture(bool alwaysShow, BoardState board, Card card, CardStates State, string classType)
    {
        StringBuilder ImagePath = new StringBuilder(@"Textures/Cards/");
        ImagePath.Append(card.GetCardGFX());
        StringBuilder CardbackPath = new StringBuilder(@"Textures/");
        if (card.PlayerID == 0)
            CardbackPath.Append("Lannister cardback");
        else
            CardbackPath.Append("Stark cardback");

        int attack = board.GetCardAttack(card);
        string manaString = "<b>" + board.GetCardManaCost(card).ToString() + "</b>";
        string attackString = "<b>" + attack.ToString() + "</b>";
        string healthString = "<b>" + board.GetCardHealth(card).ToString() + "</b>";
        if (board.GetCardMaxHealth(card) > board.GetCardHealth(card))
            healthString = "<color=MAROON>" + healthString + "</color>";
       UpdateCard(alwaysShow || State == CardStates.InHand && card.PlayerID == 0,@"<b>" + card.GetName() + @"</b>", ImagePath.ToString(), CardbackPath.ToString(), manaString, attackString, attack > 0, healthString, State, classType);
        
    }

    public void UpdateCard(bool showFront, string Name, string ImagePath, string CardbackPath, string Mana, string Attack, bool showAttack, string Health, CardStates State, string classType)
    {
        Hand.gameObject.SetActive(State == CardStates.InDeck || State == CardStates.InHand);
        Hero.gameObject.SetActive(State == CardStates.Hero);
        Minion.gameObject.SetActive(State == CardStates.OnBoard);

        string cardBack = (string)SQL.Query("SELECT Cardback FROM Classes WHERE Type='" + classType + "'").Rows[0][0];
        string cardFront = (string)SQL.Query("SELECT Cardfront FROM Classes WHERE Type='" + classType + "'").Rows[0][0];
        Hand.UpdateCard(showFront, Name, "", Mana.ToString(), Attack.ToString(), Health, ImagePath, cardBack, cardFront);
        Hero.UpdateCard(Attack.ToString(), showAttack, Health, true, ImagePath);
        Minion.UpdateCard(Attack.ToString(), Health, ImagePath);
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentAnimation != null && CurrentAnimation.IsRunning())
            CurrentAnimation.Continue();
    }
}
