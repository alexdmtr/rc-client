using UnityEngine;
using uLink;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine.UI;
using Parse;
using System.Threading.Tasks;

public class GlobalStart : UnityEngine.MonoBehaviour {

	// Use this for initialization
    public GameObject GamePrefab;
    public GUIStyle GUIStyle;
    public MeshRenderer LoadingScreen;
    public UnityEngine.UI.Text QuoteSection;
    public MeshRenderer DummyForLoading;
    public AudioSource MusicPlayer;
    public Image ProgressBar;
    public Text LoadingText;

    public AudioClip LannisterMusic;

    IEnumerator LoadGameData()
    {
        LoadingText.text = "Loading textures";
        DataTable dt = SQL.Query("SELECT CardGFX FROM Cards");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            StringBuilder ImagePath = new StringBuilder(@"Textures/Cards/");
            ImagePath.Append(dt.Rows[i]["CardGFX"] as string);
            DummyForLoading.material.SetTexture(0, Global.LoadTexture(ImagePath.ToString()));
            ProgressBar.fillAmount = (i * 1f/ dt.Rows.Count) * 0.25f;
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

        
        AsyncOperation async = Application.LoadLevelAsync("MapScene");
        yield return async;
        Global.State = GlobalStates.Map;

    }
    void Awake()
    {
    }


	void Start () {
        Global.Initialize(GamePrefab);
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

    string GetIntroText()
    {
        #region longText
        string text = @"<b>In-development Card Game</b>

<i>-Demonstrative build #1–</i>
                           
Hello there and welcome! Thank you for taking your time to review this project. My name is Alex Dumitru and I’d like to present to you the first demonstrative version of my upcoming card game.

<b><i>GAMEPLAY</i></b>
The game is easy to learn. There are 2 players; each of them starts out with a deck of cards (in this case, 10 cards for each player). Cards can be many things, but, for the purposes of this demonstration, there is only one type of card: the character. A character is defined by two values: its <b><color=YELLOW>ATTACK VALUE</color></b>, shown by the number on its bottom left side, and its <b><color=RED>HEALTH VALUE</color></b>, shown by the number on its bottom right side.

Each player starts in control of a <b>HERO</b> character. This hero must be protected at all costs; when a player's hero dies, that player <b><color=RED>LOSES</color></b> the game. Thus the game's <b><color=GREEN>WINNING CONDITION:</color></b> kill the enemy hero.

The game is played in turns, each consecutive turn belonging to another player (the active player changes). Each turn, the active player draws a card from the top of their deck, their maximum amount of mana crystals is increased (up to 10) and that amount is completely replenished. Once a player ends his turn, the other player's next turn begins.
If you are the active player and seek to end your turn, simply press the end turn button at the right of the game board. 
NOTE: The game is meant for online multiplayer vs human opponents. For the purposes of this demonstration, the opponent has been outfitted with a basic Artificial Intelligence.

The game starts with each player drawing 4 cards, after which the turn starts for the first player (in this case, the human player).

At the bottom of the screen lies your current hand. In your hand you find all the cards that you have drawn from your deck. If it is your turn and you have sufficient mana crystals (denoted by the number of mana crystals that appears in the bottom-right corner of the screen), you can play a card, substracting its <b><color=BLUE>MANA COST</color></b> (shown in its upper left corner whilst the card is in your hand) from your current mana.To play a card, simply click and drag it onto the board.
In this instance, playing a character card means putting it in on your side of the board, represented by the space between the middle line and your hero.

Once a character is on the board and once a turn has passed, it can attack, once per turn, an enemy character. An attack is simple - each of the two participants take damage (their health is lowered) by the attack value of the other. If a character's health reaches 0, they die.
In order to attack with a ready character, simply click and drag your character, releasing it over the suitable enemy character you wish to attack.

<b><i>LORE</i></b>
For this demonstration, I have chosen one scenario: the Battle of the Green Fork, as presented in Game of Thrones (the TV series), in which Tywin Lannister, leading a numerous Lannister host, aided by Vale wildlings - which were recruited by his son, Tyrion - makes battle with an unexpectedly small Stark host. The tiny Stark host of about 2,000 men ultimately turned out to be a diversion, as most of Robb Stark's army had been focused on the capture of Jaime Lannister at the Battle of the Whispering Wood.
You (the human player) are in control of Tywin Lannister and his army. March and destroy whatever forces Stark throws in your way.
NOTE: Robb Stark does not actually participate in this battle; the choice of using him as the opposing Hero is symbolic. 

Your forces consist of, primarily, a large number of Lannister Infantry and Cavalry - efficient and plentiful. You also are in possession of a wildling army, as represented by three tribal chieftains - Shagga, Timett and Chella. 

The Stark army is formed mostly of reserve footmen and riders, of little value. Victory is imminent.

<b><color=YELLOW>HEAR ME ROAR!</color></b>



<i>(NOTE: This debriefing can also be read in-game, by clicking the 'Help' button in the bottom-right corner of the board.)</i>
";
        #endregion
        return text;
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
        if (LoadingScreens == null || LoadingScreens.Count == 0)
        {
            LoadingScreens = new List<string>();
            DataTable dt = SQL.Query("SELECT * FROM LoadingScreens");
            for (int i = 0; i < dt.Rows.Count; i++)
                LoadingScreens.Add(dt.Rows[i]["Image"] as string);
        }


        int indx = UnityEngine.Random.Range(0, LoadingScreens.Count-1);
        LoadingScreen.material.SetTexture(0, Global.LoadTexture(@"Textures/Loading Screens/" + LoadingScreens[indx]));
        LoadingScreens.RemoveAt(indx);


        if (Quotes == null || Quotes.Count == 0)
        {
            Quotes = new List<string>();
            DataTable dt = SQL.Query("SELECT * FROM Quotes");
            for (int i = 0; i < dt.Rows.Count; i++)
                Quotes.Add((string)dt.Rows[i]["Quote"]);
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
            
            GUI.Label(new Rect(0, 0, width, 1200), GetIntroText(), GUIStyle);

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
