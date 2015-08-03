using UnityEngine;
using System.Collections;

public class CardMinion : MonoBehaviour {

    public CardFrontMinion Front;

	// Use this for initialization
	void Start () {
	
	}

    public void UpdateCard(string Attack, string Health, string TexturePath)
    {
        Front.SetAttack(Attack);
        Front.SetHealth(Health);
        Front.SetTexturePath(TexturePath);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
