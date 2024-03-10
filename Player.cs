using System;
using System.Collections.Generic;

namespace RaceTo21
{
	public class Player
	{
		public string name;
		public List<Card> cards = new List<Card>();
		public PlayerStatus status = PlayerStatus.active;
		public int score;
		public int TotalScore;
        public FlexLayout HandLayout { get; set; }
        public Label TotalValueLabel { get; set; }
        public Button HitButton { get; set; }
        public Button StayButton { get; set; }
        public Label StatusLabel { get; set; }
        public bool HasActedThisRound { get; set; }

        public Player(string n)
		{
			name = n;
        	}

		/* Introduces player by name
		 * Called by CardTable object
		 */
		public void Introduce(int playerNum)
		{
			Console.WriteLine("Hello, my name is " + name + " and I am player #" + playerNum);
		}
		public void ShowScore()
        	{
            		Console.WriteLine(name + "'s current score: " + score +", Total score: " + TotalScore);
        	}

        	public void Reset()
        	{
            		cards.Clear();
            		status = PlayerStatus.active;
        	}

        	public int CardsTotalValue()
        	{
            		int totalValue = 0;
            		foreach (var card in cards)
            		{
                		string rank = card.ID.Substring(0, card.ID.Length - 1);
                		int cardValue = 0;
                		switch (rank)
                		{
                    			case "A":
                        			cardValue = 1;
                        			break;
                    			case "J":
                        			cardValue = 10;
                        			break;
                    			case "Q":
                        			cardValue = 10;
                        			break;
                    			case "K":
                        			cardValue = 10;
                        			break;
                    			default:
                        			cardValue = int.Parse(rank);
                        			break;
                		}
                		totalValue += cardValue;
            		}
            		return totalValue;
        	}
    	}
}

