using System;
using System.Collections.Generic;

[Serializable()]
public class ActionData
{
    public BoardAction Action;
    public BoardState FinalState;
    public List<KeyValuePair<BoardAction, BoardState>> History;

    public ActionData()
    {
        this.History = new List<KeyValuePair<BoardAction, BoardState>>();
    }
    public ActionData(BoardAction Action, BoardState FinalState, List<KeyValuePair<BoardAction, BoardState>> History)
    {
        this.Action = Action;
        this.FinalState = FinalState;
        this.History = History;
    }
};