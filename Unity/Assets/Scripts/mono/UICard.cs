using UnityEngine;
using System.Collections;
using System.Text;

public class UICard : MonoBehaviour {

    CardFrontHand Hand;
    CardFrontHero Hero;
    CardFrontMinion Minion;

    public int ? cardID;

    void UpdateGraphics(BoardState Board, Card Card)
    {
        this.cardID = Card.ID;

        Hand.SetActive(false);
        Hero.SetActive(false);
        Minion.SetActive(false);

        #region build mana string
        StringBuilder manaString = new StringBuilder("<b>");
        int mana = Board.GetCardManaCost(Card);
        manaString.Append(mana);
        manaString.Append("</b>");
        #endregion
        #region build attack string
        StringBuilder attackString = new StringBuilder("<b>");
        int attack = Board.GetCardAttack(Card);
        attackString.Append(attack);
        attackString.Append("</b>");
        #endregion
        #region build health string
        StringBuilder healthString = new StringBuilder("<b>");
        int health = Board.GetCardHealth(Card);
        bool isDamaged = false;

        if (health < Card.GetBaseMaxHealth())
            isDamaged = true;

        if (isDamaged)
            healthString.Append("<color=RED>");

        healthString.Append(health);

        if (isDamaged)
            healthString.Append("</color>");

        healthString.Append("</b>");
        #endregion
        #region build name string
        StringBuilder nameString = new StringBuilder();
        nameString.Append(Card.GetName());
        #endregion
        #region build texture path
        StringBuilder texturePath = new StringBuilder(@"Textures/Cards");
        texturePath.Append(Card.GetCardGFX());
        #endregion 

        if (Card.State == CardStates.Hero)
        {
            Hero.SetTexturePath(texturePath.ToString());
            Hero.SetAttack(attackString.ToString(), attack != 0);
            Hero.SetHealth(healthString.ToString(), true);
            Hero.SetActive(true);
        }
        else if (Card.State == CardStates.InHand || Card.State == CardStates.InDeck)
        {
            Hand.SetTexturePath(texturePath.ToString());
            Hand.SetMana(manaString.ToString());
            Hand.SetAttack(attackString.ToString());
            Hand.SetHealth(healthString.ToString());
            Hand.SetName(nameString.ToString());
            Hand.SetActive(true);
        }
        else if (Card.State == CardStates.OnBoard)
        {
            Minion.SetTexturePath(texturePath.ToString());
            Minion.SetTexturePath(Card.GetCardGFX());
            Minion.SetAttack(attackString.ToString());
            Minion.SetHealth(healthString.ToString());
        }

    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
