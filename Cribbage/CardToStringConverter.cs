using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cribbage
{
    public class CardToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");
            Card card = value as Card;
            if (card == null)
            {
                return "";
            }
            return card.CardRank.ToString().Substring(1, 1) + card.CardSuit.ToString().Substring(1, 1);

        }

        public static Card ConvertStringToCard(string input)
        {

            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            /*
            char[] text = input.ToUpper().ToCharArray();
            if (text.Length > 2)
            {
                throw new ArgumentException("Invalid card description - length should not be more than two characters");
            }
            else if (text.Length == 0)
            {
                return null;
            }
            */

            /* Match hierarchy:
             * - Check for longest valid match on start
             * - If there is a valid match, evaluate tail
             * -- If tail match, take it
             * -- If tail partial, no error
             * - If front partial, no error
             * - Error
             */

            //default values for compiler since it's non-nullable
            Card.Rank rank = Card.Rank.Ace;
            Card.Suit suit = Card.Suit.Club;
            bool partial = false;

            SortedList<int, Enum> matches;
            IList<string> keys;

            matches = new SortedList<int, Enum>(Comparer<int>.Create((x, y) => y.CompareTo(x)));

            foreach (Card.Rank loopRank in Enum.GetValues(typeof(Card.Rank)))
            {
                keys = Utilities.GetMatchesForEnum(loopRank);

                foreach (string key in keys)
                {
                    if (input.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                    {
                        matches.Add(key.Length, loopRank);
                    }
                    if (!partial && key.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                    {
                        partial = true;
                    }
                    
                }
            }

            if (matches.Count > 0)
            {
                rank = (Card.Rank)matches.Values[0];
                input = input.Substring(matches.Keys[0]);
            }
            else if (!partial)
            {
                throw new ArgumentException("Invalid rank");
            }


            matches = new SortedList<int, Enum>(Comparer<int>.Create((x, y) => y.CompareTo(x)));

            foreach (Card.Suit loopSuit in Enum.GetValues(typeof(Card.Suit)))
            {
                keys = Utilities.GetMatchesForEnum(loopSuit);

                foreach (string key in keys)
                {
                    if (input.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                    {
                        matches.Add(key.Length, loopSuit);
                    }
                    if (!partial && key.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                    {
                        partial = true;
                    }

                }
            }

            if (matches.Count > 0)
            {
                suit = (Card.Suit)matches.Values[0];
            }
            else if (partial)
            {
                return null;
            }

            else
            {
                throw new ArgumentException("Invalid suit");
            }


            /*
            //parse rank
            switch (text[0])
            {
                case 'A':
                case '1':
                    rank = Card.Rank.Ace;
                    break;
                case '2':
                    rank = Card.Rank.Two;
                    break;
                case '3':
                    rank = Card.Rank.Three;
                    break;
                case '4':
                    rank = Card.Rank.Four;
                    break;
                case '5':
                    rank = Card.Rank.Five;
                    break;
                case '6':
                    rank = Card.Rank.Six;
                    break;
                case '7':
                    rank = Card.Rank.Seven;
                    break;
                case '8':
                    rank = Card.Rank.Eight;
                    break;
                case '9':
                    rank = Card.Rank.Nine;
                    break;
                case 'T':
                    rank = Card.Rank.Ten;
                    break;
                case 'J':
                    rank = Card.Rank.Jack;
                    break;
                case 'Q':
                    rank = Card.Rank.Queen;
                    break;
                case 'K':
                    rank = Card.Rank.King;
                    break;
                default:
                    throw new System.ArgumentException("Invalid rank");
                //exception
            }
            if (text.Length == 1)
            {
                return null;
            }
             
            //parse suit
            switch (text[1])
            {
                case 'C':
                    suit = Card.Suit.Club;
                    break;
                case 'S':
                    suit = Card.Suit.Spade;
                    break;
                case 'D':
                    suit = Card.Suit.Diamond;
                    break;
                case 'H':
                    suit = Card.Suit.Heart;
                    break;
                default:
                    throw new System.ArgumentException("Invalid suit");
            }
             */
            return new Card(suit, rank);
        }

    

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {

            string input = value as string;
            try
            {
                return ConvertStringToCard(input);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
