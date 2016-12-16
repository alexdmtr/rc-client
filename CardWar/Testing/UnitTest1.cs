using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestServerDbConnection()
        {
            Db.UpdateFromServer();

            Assert.IsTrue(Db.Cards.Count > 0);
            Assert.IsTrue(Db.Classes.Count > 0);

            System.Diagnostics.Debug.WriteLine(String.Format("{0} Cards\n{1} Card classes", Db.Cards.Count, Db.Classes.Count));
        }

        [TestMethod]
        public void TestBoard()
        {
            Db.UpdateFromServer();

            var Board = new BoardState();
            Board.AddPlayer(0);
            Board.AddPlayer(1);
            Board.AddCard("CARD_ROBB_STARK", 1, CardStates.Hero, null);
            Board.AddCard("CARD_CERSEI_LANNISTER", 0, CardStates.Hero, null);

            //throw new Exception(Board.Cards.Find(c => c.Type.Equals("CARD_ROBB_STARK")).GetBaseMaxHealth().ToString());
            Assert.IsTrue(Board.Cards.Find(c => c.Type.Equals("CARD_ROBB_STARK")).GetBaseMaxHealth().Equals(30));
        }
    }
}
