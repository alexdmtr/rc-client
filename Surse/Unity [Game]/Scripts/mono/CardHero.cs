using UnityEngine;
using System.Collections;

public class CardHero : MonoBehaviour {

    public CardFrontHero Front;


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
    }

	// Use this for initialization
	void Start () {
	
	}

    public void UpdateCard(string Attack, bool isAttackVisible, string Health, bool isHealthVisible, string TexturePath)
    {
        Front.SetAttack(Attack, isAttackVisible);
        Front.SetHealth(Health, isHealthVisible);
        Front.SetTexturePath(TexturePath);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
