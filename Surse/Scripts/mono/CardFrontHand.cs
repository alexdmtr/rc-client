using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardFrontHand : MonoBehaviour {


    public RawImage Image;
    public RawImage FrontImage;
    public Text Name;
    public Text Mana;
    public Text Attack;
    public Text Health;
    public Text Description;
    public CanvasGroup CanvasGroup;

    public GameObject Type;
    public Text TypeText;


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
 
	// Use this for initialization
	void Start () {
	}

    void SetOpacity(float newValue)
    {
        CanvasGroup.alpha = newValue;
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    public void SetTexturePath(string newTexture)
    {
        Image.texture = Global.LoadTexture(newTexture);
    }

    public void SetFrontImage(string newImage)
    {
        FrontImage.texture = Global.LoadTexture(newImage);
    }

    public void SetName(string newName)
    {
        Name.text = newName;
    }

    public void SetMana(string newMana)
    {
        Mana.text = newMana;
    }

    public void SetAttack(string newAttack)
    {
        Attack.text = newAttack;
    }

    public void SetHealth(string newHealth)
    {
        Health.text = newHealth;
    }

    public void SetDescription(string newDescription)
    {
        Description.text = newDescription;  
    }

    public void SetType(string newType, bool isVisible)
    {
        TypeText.text = newType;
        Type.SetActive(isVisible);
    }

   
 
	
	// Update is called once per frame
	void Update () {
	}
}
