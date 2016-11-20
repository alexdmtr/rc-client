using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardFrontMinion : MonoBehaviour {

    public RawImage Image;
    public Text Attack;
    public Text Health;
    public Text DamageTakenText;

    // Use this for initialization
    void Start()
    {
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void SetTexturePath(string newTexture)
    {
        Image.texture = Global.LoadTexture(newTexture);
    }

    public void SetAttack(string newAttack)
    {
        Attack.text = newAttack;
    }

    public void SetHealth(string newHealth)
    {
        Health.text = newHealth;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
