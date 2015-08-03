using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class ClassInfo : MonoBehaviour {

	// Use this for initialization
    public RawImage Image;
    public Text Name;
    public Text Quote;

    public string ClassType;
    void Start () {
	
	}

    public void UpdateInfo()
    {
        string heroType = (string)SQL.Query("SELECT Hero FROM Classes WHERE Type='" + ClassType + "'").Rows[0][0];
        string heroName =  (string)SQL.Query("SELECT Name FROM Cards WHERE Type='" + heroType + "'").Rows[0][0];
        string heroImage = (string)SQL.Query("SELECT CardGFX From Cards WHERE Type='" + heroType + "'").Rows[0][0];
        string heroQuote = (string)SQL.Query("SELECT Quote From Cards WHERE Type='" + heroType + "'").Rows[0][0];

        Name.text = heroName;
        Quote.text = "<i>" + heroQuote + "</i>";
        StringBuilder ImagePath = new StringBuilder(@"Textures/Cards/");
        ImagePath.Append(heroImage);

        Image.texture = Global.LoadTexture(ImagePath.ToString());
    }
	// Update is called once per frame
	void Update () {
	
	}
}
