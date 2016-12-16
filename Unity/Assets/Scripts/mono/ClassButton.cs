using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClassButton : MonoBehaviour {

    public ClassInfo ClassInfo;
    public string ClassType;
    public Text Text;

    // Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateText()
    {
        string text = Db.Classes.Find(c => c.Type.Equals(ClassType)).Name;
        //string text = (string)SQL.Query("SELECT Name FROM Classes WHERE Type='" + ClassType + "'").Rows[0][0];
        Text.text = text;
    }

    public void ButtonPress()
    {
        ClassInfo.gameObject.SetActive(true);
        ClassInfo.ClassType = ClassType;
        ClassInfo.UpdateInfo();
    }
}
