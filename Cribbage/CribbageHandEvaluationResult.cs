using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cribbage
{
    public class CribbageHandEvaluationResult
    {
        public double ExpectedHandPoints
        {
            get;
            private set;
        }

        public double ExpectedTotalPoints
        {
            get;
            private set;
        }

        public double ExpectedCribPoints
        {
            get;
            private set;
        }

        public int GuaranteedPoints
        {
            get;
            private set;
        }

        public int MaximumPoints
        {
            get;
            private set;
        }

        public int GuaranteedHandPoints
        {
            get;
            private set;
        }

        public int MaximumHandPoints
        {
            get;
            private set;
        }

        public int MaximumCribPoints
        {
            get;
            private set;
        }

        public int MinimumCribPoints
        {
            get;
            private set;
        }

        public List<Card> SelectedCards
        {
            get;
            private set;
        }

        public string Description
        {
            get
            {
                StringBuilder sb = null;
                foreach (Card card in SelectedCards)
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder(card.ToString());
                    }
                    else
                    {
                        sb.Append(", " + card.ToString());
                    }
                }
                return sb.ToString();
            }
        }

        public CribbageHandEvaluationResult(CribbageDeal.CribbageSplit split, bool ownCrib, Deck deck, CancellationToken cancellationToken)
        {

            SelectedCards = new List<Card>(split.Hand.Cards);
            SelectedCards.Sort();
            
            Card[] cards = deck.ToArray();
            
            int expectedCrib=0;
            int expectedHand=0;
            this.GuaranteedPoints = int.MaxValue;
            this.MaximumPoints = int.MinValue;
            this.GuaranteedHandPoints = int.MaxValue;
            this.MaximumHandPoints = int.MinValue;

            int handPoints;
            int cribPoints;
            this.MaximumCribPoints = int.MinValue;
            this.MinimumCribPoints = int.MaxValue;
            for(int turn=0; turn<cards.Length; turn++)
            {

                handPoints = split.Hand.CountPoints(cards[turn]);
                expectedHand += handPoints;
                if (handPoints < this.GuaranteedHandPoints)
                {
                    this.GuaranteedHandPoints = handPoints;
                }
                if (handPoints > this.MaximumHandPoints)
                {
                    this.MaximumHandPoints = handPoints;
                }
                
                for (int crib1 = 0; crib1 < cards.Length-1; crib1++)
                {
                    if (crib1 != turn)
                    {
                        for (int crib2 = crib1 + 1; crib2 < cards.Length; crib2++)
                        {
                            if (cancellationToken != null)
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                            }
                            if (crib2 != turn)
                            {
                                List<Card> crib = new List<Card>(split.Crib);
                                crib.Add(cards[crib1]);
                                crib.Add(cards[crib2]);
                                cribPoints=(new CribbageHand(crib)).CountPoints(cards[turn]);
                                expectedCrib += cribPoints;
                                if (cribPoints < this.MinimumCribPoints)
                                {
                                    this.MinimumCribPoints = cribPoints;
                                }
                                if (cribPoints > this.MaximumCribPoints)
                                {
                                    this.MaximumCribPoints = cribPoints;
                                }
                 
                            }
                        }

                    }
                }
                  

            }
            this.ExpectedHandPoints = (double)expectedHand / deck.Count();
            this.ExpectedCribPoints = ((double)expectedCrib / (deck.Count * (deck.Count - 1) * (deck.Count - 2)) * 2);
            this.ExpectedTotalPoints = ownCrib ? this.ExpectedHandPoints + this.ExpectedCribPoints : this.ExpectedHandPoints - this.ExpectedCribPoints;
            this.GuaranteedPoints = ownCrib ? this.GuaranteedHandPoints + this.MinimumCribPoints : this.GuaranteedHandPoints;
            this.MaximumPoints = ownCrib ? this.MaximumHandPoints + this.MaximumCribPoints : this.MaximumHandPoints;
        }
    
    }
}
