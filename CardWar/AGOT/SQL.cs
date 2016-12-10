using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Mono.Data.Sqlite;

public class SQL
{
    private static SqliteConnection Connection;
    public static Dictionary<string, DataTable> CachedQueries;

    public static void Initialize()
    {
        Connection = new SqliteConnection(@"Data Source=file:memory");
        Connection.Open();
        CachedQueries = new Dictionary<string, DataTable>();

        //SQL.Query ("SELECT * FROM Cards");
    }
    public static void Quit()
    {
        Connection.Close();
    }
    public static void ExecuteFile(string filePath)
    {
        //StreamReader StreamReader = new StreamReader(filePath);
        //string text = StreamReader.ReadToEnd();
        string text;

        TextAsset textAsset = (TextAsset)Resources.Load(filePath, typeof(TextAsset));
        text = textAsset.text;
        Execute(text);
    }
    public static List<string> MySQLtoSQLLite(string commandText)
    {
        string[] separator = new string[1];
        separator[0] = ";";

        string[] commands = commandText.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
        List<string> commandList = new List<string>();

        foreach (string command in commands)
        {
            List<string> currentCommands = new List<string>();
            if (command.ToUpper().Contains("INSERT INTO"))
            {
                int x = command.ToUpper().IndexOf("VALUES");
                string repeat = command.Substring(0, x + "VALUES".Length);
                string[] separator2 = new string[2];
                separator2[0] = "(";
                separator2[1] = ")";

                string[] rows = command.Substring(x + "VALUES".Length).Split(separator2, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string row in rows)
                    currentCommands.Add(repeat + "(" + row + ")");
            }
            else
                currentCommands.Add(command);

            foreach (string currentCommand in currentCommands)
                commandList.Add(currentCommand);
        }


        return commandList;
    }
    private static DataTable _query(string queryText)
    {
        SqliteCommand Command = Connection.CreateCommand();
        Command.CommandText = queryText;

        SqliteDataReader DataReader = Command.ExecuteReader();


        DataTable DataTable = new DataTable();
        DataTable.Load(DataReader);
        DataReader.Close();

        return DataTable;
    }

    public static DataTable Query(string queryText)
    {
        if (!CachedQueries.ContainsKey(queryText))
            CachedQueries[queryText] = _query(queryText);
        return CachedQueries[queryText];
    }

    public static void Execute(string commandText)
    {
        CachedQueries.Clear();

        SqliteCommand Command = Connection.CreateCommand();

        List<string> commands = MySQLtoSQLLite(commandText);
        foreach (string command in commands)
        {
            bool ok = true;
            try
            {
                Command.CommandText = command;
                Command.ExecuteNonQuery();
            }
            catch
            {
                ok = false;
            }

            /*if (ok)
                Debug.Log ("SQL OK\n" + command);
            else
                Debug.Log ("SQL ERROR\n" + command);*/
        }
        //SqlCommand Command = new SqlCommand(commandText, Connection);
        //Command.ExecuteNonQuery();
    }
}