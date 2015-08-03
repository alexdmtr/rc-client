using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroName : MonoBehaviour {

    public Text HeroText;
	// Use this for initialization
	void Start () {
	
	}

    void UpdateText(string newText)
    {
        HeroText.text = newText;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
