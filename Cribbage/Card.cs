using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cribbage
{
    public class Card : IComparable<Card>
    {
        public enum Suit
        {
            [Image(@"/Cribbage;component/Resources/Images/cardsuit_club.png",UriKind.Relative)]
            [Abbreviation("C")]
            Club,
            [Image(@"/Cribbage;component/Resources/Images/cardsuit_diamond.png", UriKind.Relative)]
            [Abbreviation("D")]
            Diamond,
            [Image(@"/Cribbage;component/Resources/Images/cardsuit_heart.png", UriKind.Relative)]
            [Abbreviation("H")]
            Heart,
            [Image(@"/Cribbage;component/Resources/Images/cardsuit_spade.png", UriKind.Relative)]
            [Abbreviation("S")]
            Spade,
        }

        public enum Rank
        {
            [Abbreviation("A")]
            [Synonym("1")]
            Ace,
            [Abbreviation("2")]
            Two,
            [Abbreviation("3")]
            Three,
            [Abbreviation("4")]
            Four,
            [Abbreviation("5")]
            Five,
            [Abbreviation("6")]
            Six,
            [Abbreviation("7")]
            Seven,
            [Abbreviation("8")]
            Eight,
            [Abbreviation("9")]
            Nine,
            [Synonym("10")]
            [Abbreviation("T")]
            Ten,
            [Abbreviation("J")]
            Jack,
            [Abbreviation("Q")]
            Queen,
            [Abbreviation("K")]
            King
        }

        private Suit _suit;
        public Suit CardSuit
        {
            private set
            {
                _suit = value;
            }
            get
            {
                return _suit;
            }
        }

        private Rank _rank;
        public Rank CardRank
        {
            private set
            {
                _rank = value;
            }
            get
            {
                return _rank;
            }
        }

        public Card(Suit suit, Rank rank)
            : this(suit, rank, null)
        {

        }

        public Card(Suit suit, Rank rank, Image image)
        {
            this._suit = suit;
            this._rank = rank;
            this.Image = image;
        }

        public override string ToString()
        {
            return CardRank.ToString() + " of " + CardSuit.ToString() + "s";
        }

        public static string GetRankAbbreviation(Card.Rank rank)
        {
            FieldInfo fi = typeof(Card.Rank).GetField(rank.ToString());
            if (fi != null)
            {
                object[] attributes = fi.GetCustomAttributes(typeof(AbbreviationAttribute), true);
                if (attributes != null && attributes.Length > 0)
                {
                    return ((AbbreviationAttribute)attributes[0]).Abbreviation;
                }
            }
            return rank.ToString();

        }

        public string RankAbbreviation
        {
            get
            {
                return GetRankAbbreviation(this.CardRank);
            }
         }

        public Image Image
        {
            private set;
            get;
        }

        public int CompareTo(Card other)
        {
            return ((int)this.CardRank).CompareTo((int)other.CardRank);
        }
    }
}
