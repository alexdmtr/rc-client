using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Mono.Data.Sqlite;
using CardWar.Models;
using System.Net;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;

public class Db
{
    public static List<CardModel> Cards = new List<CardModel>();
    public static List<ClassModel> Classes = new List<ClassModel>();

    public static CardModel FindCard(string Type)
    {
        return Cards.Find(card => card.Type.Equals(Type));
    }

    public static List<string> GetCardTextures()
    {
        return Cards.AsQueryable().Select(x => x.CardGFX).ToList();
    }

    public static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }

    static string QueryServer(string table)
    {
        string URL = @"https://card-war-server.herokuapp.com/query?";

        using (WebClient client = new WebClient())
        {
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            return client.DownloadString(URL + "table=" + table);
            
        }
        
        
    }
    public static void UpdateFromServer()
    {
        using (StreamWriter sw = new StreamWriter("cards.json", false, System.Text.Encoding.UTF8))
        {
            string cardsJson = QueryServer("cards");
            sw.Write(cardsJson);
            Cards = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CardModel>>(cardsJson);
        }

        using (StreamWriter sw = new StreamWriter("classes.json", false, System.Text.Encoding.UTF8))
        {
            string classesJson = QueryServer("classes");
            sw.Write(classesJson);
            Classes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ClassModel>>(classesJson);
        }
    }


    //private static SqliteConnection Connection;
    //public static Dictionary<string, DataTable> CachedQueries;

    //public static void Initialize()
    //{
    //    Connection = new SqliteConnection(@"Data Source=file:memory");
    //    Connection.Open();
    //    CachedQueries = new Dictionary<string, DataTable>();

    //    //SQL.Query ("SELECT * FROM Cards");
    //}
    //public static void Quit()
    //{
    //    Connection.Close();
    //}
    //public static void ExecuteFile(string filePath)
    //{
    //    //StreamReader StreamReader = new StreamReader(filePath);
    //    //string text = StreamReader.ReadToEnd();
    //    string text;

    //    TextAsset textAsset = (TextAsset)Resources.Load(filePath, typeof(TextAsset));
    //    text = textAsset.text;
    //    Execute(text);
    //}
    //public static List<string> MySQLtoSQLLite(string commandText)
    //{
    //    string[] separator = new string[1];
    //    separator[0] = ";";

    //    string[] commands = commandText.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
    //    List<string> commandList = new List<string>();

    //    foreach (string command in commands)
    //    {
    //        List<string> currentCommands = new List<string>();
    //        if (command.ToUpper().Contains("INSERT INTO"))
    //        {
    //            int x = command.ToUpper().IndexOf("VALUES");
    //            string repeat = command.Substring(0, x + "VALUES".Length);
    //            string[] separator2 = new string[2];
    //            separator2[0] = "(";
    //            separator2[1] = ")";

    //            string[] rows = command.Substring(x + "VALUES".Length).Split(separator2, System.StringSplitOptions.RemoveEmptyEntries);
    //            foreach (string row in rows)
    //                currentCommands.Add(repeat + "(" + row + ")");
    //        }
    //        else
    //            currentCommands.Add(command);

    //        foreach (string currentCommand in currentCommands)
    //            commandList.Add(currentCommand);
    //    }


    //    return commandList;
    //}
    //private static DataTable _query(string queryText)
    //{
    //    SqliteCommand Command = Connection.CreateCommand();
    //    Command.CommandText = queryText;

    //    SqliteDataReader DataReader = Command.ExecuteReader();


    //    DataTable DataTable = new DataTable();
    //    DataTable.Load(DataReader);
    //    DataReader.Close();

    //    return DataTable;
    //}

    //public static DataTable Query(string queryText)
    //{
    //    if (!CachedQueries.ContainsKey(queryText))
    //        CachedQueries[queryText] = _query(queryText);
    //    return CachedQueries[queryText];
    //}

    //public static void Execute(string commandText)
    //{
    //    CachedQueries.Clear();

    //    SqliteCommand Command = Connection.CreateCommand();

    //    List<string> commands = MySQLtoSQLLite(commandText);
    //    foreach (string command in commands)
    //    {
    //        bool ok = true;
    //        try
    //        {
    //            Command.CommandText = command;
    //            Command.ExecuteNonQuery();
    //        }
    //        catch
    //        {
    //            ok = false;
    //        }

    //        /*if (ok)
    //            Debug.Log ("SQL OK\n" + command);
    //        else
    //            Debug.Log ("SQL ERROR\n" + command);*/
    //    }
    //    //SqlCommand Command = new SqlCommand(commandText, Connection);
    //    //Command.ExecuteNonQuery();
    //}
}