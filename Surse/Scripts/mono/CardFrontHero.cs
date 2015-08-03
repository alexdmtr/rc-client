using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardFrontHero : MonoBehaviour {

    public RawImage Image;
    public GameObject Attack;
    public Text AttackText;
    public GameObject Health;
    public Text HealthText;
    public Text DamageTakenText;
    public CanvasGroup CanvasGroup;

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
        CanvasGroup.alpha = newValue;
    }

	// Use this for initialization
	void Start () {
	}

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    public void SetTexturePath(string newTexture)
    {
        Image.texture = Global.LoadTexture(newTexture);
    }

    public void SetAttack(string newAttack, bool isVisible)
    {
        AttackText.text = newAttack;
        Attack.SetActive(isVisible);
    }

    public void SetHealth(string newHealth, bool isVisible)
    {
        HealthText.text = newHealth;
        Health.SetActive(isVisible);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
