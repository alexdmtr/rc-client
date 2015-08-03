
using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.UI;

public class GameCard : MonoBehaviour {

	public GameObject HandObject;
	public RawImage HandImage;
    public MeshRenderer HandCardback;

	/*public GameObject MinionObject;
	public MeshRenderer MinionImage;*/

	public GameObject HeroObject;
    public RawImage HeroImage;

	public GameAnimation CurrentAnimation;
	public int? CardID;

	string oldImagePath = "";
    string oldCardbackPath = "";

	// Use this for initialization
	void Start () {
		//UpdateCard ("Daenerys Targaryen", (Texture2D)HandImage.material.GetTexture(0), 0, 0, 30, CardStates.Hero);
	}

    public void UpdateCardTexture(BoardState board, Card card)
    {
        /*StringBuilder ImagePath = new StringBuilder(@"Textures/Cards/");
        ImagePath.Append(card.GetCardGFX());
        StringBuilder CardbackPath = new StringBuilder(@"Textures/");
        if (card.PlayerID == 0)
            CardbackPath.Append("Lannister cardback");
        else
            CardbackPath.Append("Stark cardback");
       UpdateCard(@"<b>" + card.GetName() + @"</b>", ImagePath.ToString(), CardbackPath.ToString(), board.GetCardManaCost(card), board.GetCardAttack(card), board.GetCardHealth(card), card.State);
        */
         }

	public void UpdateCard(string Name, string ImagePath, string CardbackPath, int Mana, int Attack, int Health, CardStates State)
	{
        
		if (State == CardStates.InDeck || State == CardStates.Dead)
		{
			//MinionObject.SetActive(false);
			
			HeroObject.SetActive(false);

			HandObject.SetActive(true);
		}
		if (State == CardStates.InDeck || State == CardStates.InHand)
		{
			//MinionObject.SetActive(false);

			HeroObject.SetActive(false);

            if (oldImagePath != ImagePath && HandImage != null)
            {
                HandImage.material.SetTexture(0, Global.LoadTexture(ImagePath));
                //HandImage.texture = Global.LoadTexture(ImagePath);
                ImagePath = oldImagePath;
            }
            if (oldCardbackPath != CardbackPath)
            {
                HandCardback.material.SetTexture(0, Global.LoadTexture(CardbackPath));
                CardbackPath = oldCardbackPath;
            }
			/*HandName.text = Name;
			HandMana.text = Mana.ToString();
			HandAttack.text = Attack.ToString();
			HandHealth.text = Health.ToString();*/

			HandObject.SetActive(true);
		}
		if (State == CardStates.OnBoard)
		{
			HandObject.SetActive(false);

			HeroObject.SetActive(false);

			/*if (oldImagePath != ImagePath)
				MinionImage.material.SetTexture(0, Global.LoadTexture(ImagePath));
			MinionAttack.text = Attack.ToString();
			MinionHealth.text = Health.ToString();

			MinionObject.SetActive(true);
		*/}
		if (State == CardStates.Hero)
		{
			HandObject.SetActive(false);	
			
			//MinionObject.SetActive(false);

            if (oldImagePath != ImagePath && HeroImage != null)
                HeroImage.material.SetTexture(0, Global.LoadTexture(ImagePath));
                //HeroImage.texture = Global.LoadTexture(ImagePath);
			//HeroAttack.text = Attack.ToString();
			//HeroHealth.text = Health.ToString();

			HeroObject.SetActive(true);
		}

	}

	// Update is called once per frame
	void Update () {
		if (CurrentAnimation != null && CurrentAnimation.IsRunning())
			CurrentAnimation.Continue();
	}
}
