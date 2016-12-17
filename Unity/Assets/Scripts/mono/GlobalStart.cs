using UnityEngine;
//using uLink;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine.UI;
//using Parse;
//using System.Threading.Tasks;
using CardWar.Models;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class GlobalStart : UnityEngine.MonoBehaviour {

	// Use this for initialization
    //public GameObject GamePrefab;
    public GUIStyle GUIStyle;
    public MeshRenderer LoadingScreen;
    public UnityEngine.UI.Text QuoteSection;
    public MeshRenderer DummyForLoading;
    public AudioSource MusicPlayer;
    public Image ProgressBar;
    public Text LoadingText;
    public GameObject LoadingUI;

    public AudioClip LannisterMusic;

    IEnumerator LoadGameData()
    {
        LoadingText.text = "Connecting to server";

        yield return null;

        Db.UpdateFromServer();

        yield return null;

        Debug.Log(string.Format("{0} cards in DB", Db.Cards.Count));

        foreach (CardModel c in Db.Cards)
        {
            Debug.Log(JsonConvert.SerializeObject(c));
        }

        Debug.Log(string.Format("{0} classes in DB", Db.Classes.Count));

        foreach (ClassModel c in Db.Classes) 
        {
            Debug.Log(JsonConvert.SerializeObject(c));
        }

        //DataTable dt = SQL.Query("SELECT CardGFX FROM Cards");

        LoadingText.text = "Loading textures";

        var cards = Db.Cards;
        for (int i = 0; i < cards.Count; i++)
        {
            StringBuilder ImagePath = new StringBuilder(@"Textures/Cards/");
            //ImagePath.Append(dt.Rows[i]["CardGFX"] as string);
            ImagePath.Append(cards[i].CardGFX);
            DummyForLoading.material.SetTexture(0, Global.LoadTexture(ImagePath.ToString()));
            ProgressBar.fillAmount = (i * 1f/ cards.Count) * 0.25f;
            yield return null;
        }

        /*Global.Map.Terrain.heightmapMaximumLOD = 1;
        ProgressBar.fillAmount = 0.25f;
        LoadingText.text = "Building heightmaps";
        yield return null;
        Global.Map.BuildHeightMap();

        ProgressBar.fillAmount = 0.5f;
        LoadingText.text = "Building alphamaps";
        yield return null;
        Global.Map.BuildAlphaMap();*/

        ProgressBar.fillAmount = 0.75f;
        LoadingText.text = "Loading level";
        //yield return new WaitForSeconds(30);


        //AsyncOperation async = Application.LoadLevelAsync("MapScene");

        AsyncOperation async = SceneManager.LoadSceneAsync("MapScene");
        yield return async;

        LoadingUI.SetActive(false);
        Global.State = GlobalStates.Map;

    }
    void Awake()
    {
    }


	void Start () {
        //Global.Initialize(GamePrefab);
        DontDestroyOnLoad(this);
        ChooseRandomLoadingScreen();
        MusicPlayer.Play();
        
        StartCoroutine("LoadGameData");
	}
	
	// Update is called once per frame
	void Update () {
        if (!MusicPlayer.isPlaying)
        {
            MusicPlayer.clip = LannisterMusic;
            MusicPlayer.Play();
        }
	}

    

    Vector2 scrollPosition = Vector2.zero;

    //[RPC]
    //void SaySomethingNice(string somethingNice)
    //{
    //    Debug.Log(somethingNice);
    //}
    public bool isConnected;
    public string toSend;

    void OnConnectedToServer()
    {
        isConnected = true;
    }

    void OnDisconnectedFromServer()
    {
        isConnected = false;
    }

    public readonly float screenDelay = 5f;
    public float lastRefresh;
    public List<string> LoadingScreens;
    public List<string> Quotes;

    void ChooseRandomLoadingScreen()
    {
        return;
        if (LoadingScreens == null || LoadingScreens.Count == 0)
        {
            LoadingScreens = new List<string>();
            //DataTable dt = SQL.Query("SELECT * FROM LoadingScreens");
            //for (int i = 0; i < dt.Rows.Count; i++)
            //    LoadingScreens.Add(dt.Rows[i]["Image"] as string);
        }


        int indx = UnityEngine.Random.Range(0, LoadingScreens.Count-1);
        LoadingScreen.material.SetTexture(0, Global.LoadTexture(@"Textures/Loading Screens/" + LoadingScreens[indx]));
        LoadingScreens.RemoveAt(indx);


        if (Quotes == null || Quotes.Count == 0)
        {
            Quotes = new List<string>();
            //DataTable dt = SQL.Query("SELECT * FROM Quotes");
            //for (int i = 0; i < dt.Rows.Count; i++)
            //    Quotes.Add((string)dt.Rows[i]["Quote"]);
        }

        indx = UnityEngine.Random.Range(0, Quotes.Count - 1);
        QuoteSection.text = Quotes[indx];
        Quotes.RemoveAt(indx);

        lastRefresh = Time.time;
    }
    
    void OnGUI()
    {

        GUILayout.BeginVertical();
        if (Global.State == GlobalStates.Intro)
        {
            Global.State = GlobalStates.Loading;
        }
        if (Global.State == GlobalStates.Loading)
        {
           
            if (Time.time - lastRefresh > screenDelay)
                ChooseRandomLoadingScreen();
            
        }
        if (Global.State == GlobalStates.Menu)
        {
            
            //GUI.Window(0, new Rect(0, 0, Screen.width, Screen.height), WindowFunction, "In-development Card Game");

            float width = Screen.width * 0.9f;
            scrollPosition = GUI.BeginScrollView(new Rect(0.025f * Screen.width, 0.025f * Screen.height, Screen.width * 0.95f, Screen.height * 0.95f), scrollPosition, new Rect(0, 0, width, 900));
            
            //GUI.Label(new Rect(0, 0, width, 1200), GetIntroText(), GUIStyle);

            GUI.EndScrollView();

            string text = "";
            if (Global.CurrentGame == null)
                text = "Okay, got it!";
            else
                text = "Back to game";
            if (GUI.Button(new Rect(Screen.width / 2 - 0.1f * Screen.width, 0.8f * Screen.height, 0.2f * Screen.width, 0.2f * Screen.height), text))
                Global.SwitchToGame();

        }

        if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 50, 100, 50), "Quit"))
            Application.Quit();

    }

    void OnApplicationQuit()
    {
        //uLink.Network.Disconnect();
      

    }
}
