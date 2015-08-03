using UnityEngine;
using System.Collections;

public class CardHand : MonoBehaviour {

    public CardFrontHand Front;
    public MeshRenderer Cardback;

    float _opacity = 1;

    public float Opacity
    {
        get
        {
            return _opacity;
        }
        set
        {
            _opacity = value;
            SetOpacity(Opacity);
        }
    }

    void SetOpacity(float newValue)
    {
        Front.Opacity = newValue;
        Color c = Cardback.sharedMaterial.color;

        c.a = newValue;
        Cardback.sharedMaterial.color = c;
    }

	// Use this for initialization
	void Start () {
	
	}

    public void UpdateCard(bool isFrontActive, string name, string description, string mana, string attack, string health, string texturePath, string cardBack, string cardFront)
    {
        Front.SetActive(isFrontActive);
        Front.SetName(name);
        Front.SetAttack(attack);
        Front.SetDescription(description);
        Front.SetMana(mana);
        Front.SetAttack(attack);
        Front.SetHealth(health);
        Front.SetTexturePath(texturePath);
        Front.SetType("", false);

        string s = @"Textures/Cardbacks/" + cardBack;
        Cardback.material.SetTexture(0, Global.LoadTexture(s));

        s = @"Textures/Cardfronts/" + cardFront;
        Front.SetFrontImage(s);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
