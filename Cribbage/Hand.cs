using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cribbage
{
    public class Hand
    {
        public enum AceValue
        {
            High,
            Low,
            Both
        }

        protected ObservableCollection<Card> _cards;

        public IEnumerable<Card> Cards
        {
            get
            {
                return _cards;
            }
        }

        public int Count
        {
            get
            {
                return _cards.Count;
            }
        }

        private SortedDictionaryImpementsReadOnly<Card.Rank, int> _ranks = new SortedDictionaryImpementsReadOnly<Card.Rank, int>();
        public IReadOnlyDictionary<Card.Rank, int> Ranks
        {
            get
            {
                return _ranks;
            }
        }


        private Dictionary<Card.Suit, int> _suits = new Dictionary<Card.Suit, int>();
        public IReadOnlyDictionary<Card.Suit,int> Suits
        {
            get
            {
                return _suits;
            }
        }

        public virtual string Error
        {
            get
            {
                return "";
            }
        }

        public Hand(ObservableCollection<Card> cards)
        {
            this._cards = cards;
            foreach (Card card in _cards)
            {
                UpdateIndexes(card, 1);
            }
        }

        public Hand():this(new ObservableCollection<Card>())
        {
        }

        public Hand(IEnumerable<Card> cards)
            : this(new ObservableCollection<Card>(cards))
        {

        }

        public void Add(Card card)
        {
            if (!_cards.Contains(card))
            {
                _cards.Add(card);
                UpdateIndexes(card, 1);
            }
        }

        private void UpdateIndexes(Card card, int val)
        {
            int state;
            if (_suits.TryGetValue(card.CardSuit, out state))
            {
                state += val;
                if (state <= 0)
                {
                    _suits.Remove(card.CardSuit);
                }
                else
                {
                    _suits[card.CardSuit] = state;
                }
            }
            else
            {
                if (val > 0)
                {
                    _suits.Add(card.CardSuit, val);
                }
            }
            if (_ranks.TryGetValue(card.CardRank, out state))
            {
                state += val;
                if (state <= 0)
                {
                    _ranks.Remove(card.CardRank);
                }
                else
                {
                    _ranks[card.CardRank] = state;
                }
            }
            else
            {
                if (val > 0)
                {
                    _ranks.Add(card.CardRank, val);
                }
            }
        }

        public bool Remove(Card card)
        {
            bool success = _cards.Remove(card);
            if (success)
            {
                UpdateIndexes(card, -1);
            }
            return success;
        }

        public Dictionary<int,int> GetRunLengths(AceValue aceVal)
        {
            int last = -2;
            int runLength=0;
            int runCount=1;
            Dictionary<int, int> runs = new Dictionary<int, int>();
            
            /*
            //loop through enum since Ranks isn't guaranteed to be ordered 
            foreach(Card.Rank rank in Enum.GetValues(typeof(Card.Rank)))
            {
                //skip low ace if only counting high
                if (aceVal == AceValue.High)
                {
                    continue;
                }
                if (!Ranks.ContainsKey(rank))
                {
                    if (runLength > 0)
                    {
                        if (runs.ContainsKey(runLength))
                        {
                            runs[runLength] += runCount;
                        }
                        else
                        {
                            runs.Add(runLength, runCount);
                        }
                        runLength = 0;
                        runCount = 1;
                    }

                }
                else
                {
                    runLength++;
                    runCount *= Ranks[rank];
                }
                

            }
            */
            int current;
            runLength = 0;
            runCount = 1;
            foreach (Card.Rank rank in _ranks.Keys)
            {
                if (rank==Card.Rank.Ace && aceVal == AceValue.High)
                {
                    continue;
                }
                current = (int)rank;

                //in a run
                if (current == last + 1)
                {
                    runLength++;
                    runCount *= _ranks[rank];
                }
                //not in a run, store last run
                else
                {
                    if (last >= 0)
                    {
                        if (runs.ContainsKey(runLength))
                        {
                            runs[runLength] += runCount;
                        }
                        else
                        {
                            runs.Add(runLength, runCount);
                        }
                    }
                    runLength = 1;
                    runCount = _ranks[rank];
                }
                last = current;

            }
            //tack on high ace if necessary
            if (aceVal != AceValue.Low)
            {
                if (_ranks.ContainsKey(Card.Rank.Ace))
                {
                    if (last == (int)Card.Rank.King)
                    {
                        runLength++;
                        runCount *= _ranks[Card.Rank.Ace];
                    }
                    else
                    {
                        runs.Add(1, _ranks[Card.Rank.Ace]);
                    }
                }
            }
            //store last run
            if (runs.ContainsKey(runLength))
            {
                runs[runLength] += runCount;
            }
            else
            {
                runs.Add(runLength, runCount);
            }

            return runs;
        }

        public bool IsFlush()
        {
            return (Suits.Values.Count() == 1);
        }

        public override string ToString()
        {
            StringBuilder sb = null;
            foreach (Card card in _cards)
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
}
