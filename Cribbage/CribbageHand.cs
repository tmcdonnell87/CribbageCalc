using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cribbage
{
    public class CribbageHand : Hand
    {

        public CribbageHand(ObservableCollection<Card> cards) : base(cards)
        {
            if (cards.Count() != 4)
            {
                throw new ArgumentException("A cribbage hand must have four cards");
            }
        }

        public CribbageHand(IEnumerable<Card> cards)
            : base(cards)
        {
            if (cards.Count() != 4)
            {
                throw new ArgumentException("A cribbage hand must have four cards");
            }
        }

        private int CountHandPoints()
        {
            int total = 0;
            //fifteens
            total += 2 * NumFifteensIterative();

            //pairs
            foreach (int numRank in Ranks.Values)
            {
                if (numRank >= 2)
                {
                    total += (int)(2 * Utilities.Combination(numRank, 2));
                }
            }

            //runs
            Dictionary<int, int> runs = GetRunLengths(Hand.AceValue.Low);
            foreach (int len in runs.Keys)
            {
                if (len >= 3)
                {
                    total += len * runs[len];
                }
            }

            return total;
        }

        public int CountPoints(Card turn)
        {

            int total = 0;
            Add(turn);
            total += CountHandPoints();
            Remove(turn);

            //flush
            if (IsFlush())
            {
                total += 4;
                if (_cards.First().CardSuit == turn.CardSuit)
                {
                    total++;
                }
            }

            //jack of suit
            IEnumerable<Card> jacksOfSuit = Cards.Where<Card>(card => card.CardRank == Card.Rank.Jack && card.CardSuit == turn.CardSuit);
            total += jacksOfSuit.Count();

            return total;
        }
        

        private int CardWorth(Card card)
        {
            if ((int)card.CardRank < 10)
            {
                return (int)(card.CardRank)+1;
            }
            else
            {
                return 10;
            }

        }

        private int NumFifteensIterative()
        {
            List<int> values = new List<int>(5);
            foreach(Card card in Cards)
            {
                values.Add(CardWorth(card));
            }
            values.Sort();
            int[] sortedArray = values.ToArray();
            return FindComponentSumsRecursive(15,sortedArray,0);
        }

        private int FindComponentSumsRecursive(int target, int[] components, int startIndex)
        {
            int sums = 0;
            int val;
            for (int idx = startIndex; idx < components.Length; idx++)
            {
                val=components[idx];
                if (val > target)
                {
                    return sums;
                }
                else if (val == target)
                {
                    sums++;
                }
                else
                {
                    sums += FindComponentSumsRecursive(target - val, components, idx + 1);
                }

            }
            return sums;
        }

        public override string Error
        {
            get
            {
                if (_cards.Count != 4)
                {
                    return "A hand must have four cards";
                }
                return "";
            }
        }
   }
}
