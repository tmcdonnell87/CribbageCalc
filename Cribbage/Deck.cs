using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cribbage
{
    public class Deck : Stack<Card>
    {
        private Card[,] cards;

        public Deck(ResourceDictionary cardImageSource):base()
        {
            cards = new Card[Enum.GetValues(typeof(Card.Suit)).Length,Enum.GetValues(typeof(Card.Rank)).Length];
            Image cardImage;
            foreach (Card.Rank rank in Enum.GetValues(typeof(Card.Rank)))
            {
                foreach (Card.Suit suit in Enum.GetValues(typeof(Card.Suit)))
                {

                    if (cardImageSource != null)
                    {
                        try
                        {
                            cardImage = new Image();
                            string key = String.Format("CARD_{0}{1}", Card.GetRankAbbreviation(rank), suit.ToString()[0]);
                            var source = cardImageSource[key];
                            cardImage.Source = (ImageSource)source;
                        }
                        catch (Exception)
                        {
                            cardImage = null;
                        }
                    }
                    else
                    {
                        cardImage = null;
                    }
                    Card card= new Card(suit, rank,cardImage);
                    cards[(int)suit,(int)rank] = card;
                    
                    base.Push(card);
                }
               
            }

        }

        public Deck():this(null)
        {
            
        }

        public void Shuffle()
        {
            Random rand = new Random();
            this.OrderBy(x => rand.Next());
        }

        public new void Push(Card card)
        {
            if (!FromDeck(card))
            {
                throw new ArgumentException("A card must be from a deck to be added");
            }

            base.Push(card);
        }

        
        public void Collect()
        {
            this.Clear();
            foreach (Card card in cards)
            {
                base.Push(card);
            }
        }

        public Card FindInDeck(Card.Suit suit, Card.Rank rank)
        {
            foreach (Card card in this)
            {
                if (card.CardSuit == suit && card.CardRank == rank)
                {
                    return card;
                }
            }
            return null;
        }

        public Card Remove(Card.Suit suit, Card.Rank rank)
        {
           return Remove(cards[(int)suit,(int)rank]);
        }

        public Card Remove(Card pullCard)
        {
            if (!FromDeck(pullCard))
            {
                return null;
            }
            Card card = null;
            Card loopCard = null;
            Stack<Card> tempStack = new Stack<Card>();
            while (this.Count>0)
            {
                loopCard = this.Pop();
                if (loopCard == pullCard)
                {
                    card = loopCard;
                    break;
                }
                tempStack.Push(loopCard);
            }
            while (tempStack.Count > 0)
            {
                this.Push(tempStack.Pop());
            }
            return card;
        }

        public bool FromDeck(Card card)
        {
            return (card==cards[(int)card.CardSuit,(int)card.CardRank]);
        }


    }
}
