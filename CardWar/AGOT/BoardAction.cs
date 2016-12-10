using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System;

[Serializable()]
public class BoardAction
{
    public string Type;
    private DataTable ActionData;

    public int? Player;
    public int? Actor;
    public int? Target;
    public int? Parameter;

    public BoardAction(string Type, int? Player, int? Actor, int? Target, int? Parameter)
    {
        this.Type = Type;
        this.Player = Player;
        this.Actor = Actor;
        this.Target = Target;
        this.Parameter = Parameter;

        StringBuilder QueryText = new StringBuilder("SELECT * FROM Actions WHERE Type='");
        QueryText.Append(Type);
        QueryText.Append(@"'");
        ActionData = SQL.Query(QueryText.ToString());
    }

    public static bool operator !=(BoardAction A, BoardAction B)
    {
        if (A.Player != B.Player)
            return true;
        if (A.Actor != B.Actor)
            return true;
        if (A.Target != B.Target)
            return true;
        if (A.Parameter != B.Parameter)
            return true;
        return false;
    }
    public static bool operator ==(BoardAction A, BoardAction B)
    {
        return !(A != B);
    }
}