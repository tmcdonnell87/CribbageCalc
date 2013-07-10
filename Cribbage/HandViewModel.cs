using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cribbage
{
    public class HandViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        protected Hand _hand;

        public HandViewModel(Hand hand)
        {
            this._hand = hand;
        }


        public IEnumerable<Card> Cards
        {
            get
            {
                return _hand.Cards;
            }
        }

        public void Add(Card card)
        {
            _hand.Add(card);
            OnHandChanged(null, new Card[] {card});

        }

        public bool Remove(Card card)
        {
            bool success = _hand.Remove(card);
            if (success)
            {
                OnHandChanged(new Card[] { card }, null);
            }
            return success;
        }

        public int Count
        {
            get
            {
                return _hand.Count;
            }
        }
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this element has been updated. The specific dependency property that changed is reported in the arguments parameter.
        /// </summary>
        /// <param name="name">The name of the updated DependencyProperty</param>
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public bool IsFlush()
        {
            return _hand.IsFlush();
        }

        public string Error
        {
            get { return _hand.Error; }
        }

        public string this[string columnName]
        {
            get 
            {
                if (String.Equals(columnName, "Cards", StringComparison.OrdinalIgnoreCase))
                {
                    return this.Error;
                }
                return "";
            }
        }

        public class HandChangedEventArgs : EventArgs
        {
            public IList<Card> RemovedCards
            {
                get;
                private set;
            }

            public IList<Card> AddedCards
            {
                get;
                private set;
            }

            public HandChangedEventArgs(IList<Card> removedCards, IList<Card> addedCards)
            {
                this.RemovedCards = removedCards;
                this.AddedCards = addedCards;

            }
        }

        public delegate void HandChangedEventHander(object souce, HandChangedEventArgs e);
        public event HandChangedEventHander HandChanged;

        public void OnHandChanged(IList<Card> removedCards, IList<Card> addedCards)
        {
            if (HandChanged != null)
            {
                HandChanged(this, new HandChangedEventArgs(removedCards, addedCards));
            }
            OnPropertyChanged("Cards");

        }


    }
}
