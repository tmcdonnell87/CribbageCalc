using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cribbage
{

    /* QUESTIONS:
     * 
     * 
     */
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private ICollectionView _resultCV;

        private Card _selectedCard;
        public Card SelectedCard
        {
            get
            {
                return _selectedCard;
            }
            set
            {
                _selectedCard = value;
                OnPropertyChanged("SelectedCard");
            }

        }

        private CribbageDealViewModel _hand;

        public CribbageDealViewModel Hand
        {
            get { return _hand; }
            set { _hand = value;
            OnPropertyChanged("Cards");
            }
        }

        private Deck _deck;
        public Deck Deck
        {
            get
            {
                return _deck;
            }
            private set
            {
                _deck = value;
                OnPropertyChanged("Deck");
            }
        }

        private ObservableCollection<Card> _spades;

        public ObservableCollection<Card> Spades
        {
            get { return _spades; }
            set { _spades = value;
            OnPropertyChanged("Spades");
            }
        }

        private Dictionary<Card.Suit, ObservableCollection<Card>> _suitsOut;

        public Dictionary<Card.Suit, ObservableCollection<Card>> SuitsOut
        {
            get { return _suitsOut; }
            set { _suitsOut = value;
            OnPropertyChanged("SuitsOut");
            }
        }

        private ObservableCollection<CribbageHandEvaluationResult> _results = new ObservableCollection<CribbageHandEvaluationResult>();
        public ObservableCollection<CribbageHandEvaluationResult> Results
        {
            get { return _results; }
            set
            {
                _results = value;
                OnPropertyChanged("Results");
            }
        }

        private int _progressPercentage;
        public int ProgressPercentage
        {
            get
            {
                return _progressPercentage;
            }
            set
            {
                _progressPercentage = value;
                OnPropertyChanged("ProgressPercentage");
            }
        }

        public static RoutedCommand RemoveCardCommand = new RoutedCommand();

        public MainWindow()
        {
            Uri resourceLocater = new Uri("/Cribbage;component/Resources/Cards.xaml", System.UriKind.Relative);
            ResourceDictionary cardResources = (ResourceDictionary)Application.LoadComponent(resourceLocater);
            Deck = new Deck(cardResources);

            //TestHand();

            Hand = new CribbageDealViewModel();
            Hand.HandChanged += SelectedHandCardsChanged;

            SuitsOut = new Dictionary<Card.Suit, ObservableCollection<Card>>();
            foreach(Card.Suit suit in Enum.GetValues(typeof(Card.Suit)))
            {
                SuitsOut.Add(suit, new ObservableCollection<Card>());
            }
            foreach (Card card in Deck)
            {
                ObservableCollection<Card> suitList;
                SuitsOut.TryGetValue(card.CardSuit, out suitList);
                if (suitList != null)
                {
                    suitList.Add(card);
                }
            }

            InitializeComponent();
            ctlCardTextBox.Focus();
        }

        private void TestHand()
        {
            StringBuilder sb = new StringBuilder("Hand test: \n");

            List<Card> cards = new List<Card>();
            cards.Add(new Card(Card.Suit.Spade, Card.Rank.King));
            cards.Add(new Card(Card.Suit.Spade, Card.Rank.Queen));
            cards.Add(new Card(Card.Suit.Spade, Card.Rank.King));
            cards.Add(new Card(Card.Suit.Club, Card.Rank.Five));
            CribbageHand cribHand = new CribbageHand(cards);
            sb.AppendLine("Points: "+cribHand.CountPoints(new Card(Card.Suit.Spade, Card.Rank.Jack)));

            cards.Add(new Card(Card.Suit.Spade, Card.Rank.Five));
            cards.Add(new Card(Card.Suit.Diamond, Card.Rank.Seven));
            cards.Add(new Card(Card.Suit.Diamond, Card.Rank.Six));
            cards.Add(new Card(Card.Suit.Spade, Card.Rank.Ace));

            Hand hand = new Hand(cards);
            Dictionary<int, int> runs;
            runs = hand.GetRunLengths(Cribbage.Hand.AceValue.Low);
            runs = hand.GetRunLengths(Cribbage.Hand.AceValue.High);
            runs = hand.GetRunLengths(Cribbage.Hand.AceValue.Both);

            MessageBox.Show(sb.ToString());


        }

        private void ctlCardTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ctlCardTextBox == null)
            {
                return;
            }

            if (SelectedCard != null)
            {
                AddCardToHand(SelectedCard.CardSuit, SelectedCard.CardRank);
                ctlCardTextBox.Clear();
            }
            OnPropertyChanged("Hand");    
        }

        private void AddCardToHand(Card.Suit suit, Card.Rank rank)
        {
            Card card = Deck.Remove(suit,rank);
            if (card != null)
            {
                ObservableCollection<Card> suitList;
                SuitsOut.TryGetValue(card.CardSuit, out suitList);
                if (suitList != null)
                {
                    suitList.Remove(card);
                } 
                Hand.Add(card);
                OnPropertyChanged("Hand");
            }
        }

        private void AddCardToHand(Card card)
        {
            if (card!=Deck.Remove(card))
            {
                return;
            }
            ObservableCollection<Card> suitList;
            SuitsOut.TryGetValue(card.CardSuit, out suitList);

            if (suitList != null)
            {
                suitList.Remove(card);
            } 
            Hand.Add(card);
            OnPropertyChanged("Hand");
        }

        private void RemoveCardFromHand(Card card)
        {
            Deck.Push(card);
            ObservableCollection<Card> suitList;
            SuitsOut.TryGetValue(card.CardSuit, out suitList);
            if (suitList != null)
            {
                suitList.Add(card);
            } 
            Hand.Remove(card);
            OnPropertyChanged("Hand");

        }

        private void ctlClearSelectedCardsButton_Click(object sender, RoutedEventArgs e)
        {
            //cache card before we start modifying collection
            List<Card> cards = new List<Card>(Hand.Cards);
            foreach (Card card in cards)
            {
                RemoveCardFromHand(card);
            }
        }

        private void SuitCards_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Card card = null;

            if (e.OriginalSource is FrameworkElement)
            {
                card = ((FrameworkElement)(e.OriginalSource)).DataContext as Card;
            }
            if (card == null)
            {
                return;
            }
            AddCardToHand(card);
        }

        private void ctlHandListView_MouseDown(object sender, RoutedEventArgs e)
        {
            Card card = null;

            if (e.OriginalSource is FrameworkElement)
            {
                card = ((FrameworkElement)(e.OriginalSource)).DataContext as Card;
            }
            if (card == null)
            {
                return;
            }
            RemoveCardFromHand(card);
        }



        private void SelectedHandCardsChanged(object sender, HandViewModel.HandChangedEventArgs e)
        {
            //clear previous results
            if (Results.Count > 0)
            {
                Results.Clear();
            }
        }

        private async void ctlSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            Progress<int> progress = new Progress<int>((pct) => this.ProgressPercentage = pct);
            bool ownCrib = (ctlOwnCribRadioButton.IsChecked.HasValue && (bool)ctlOwnCribRadioButton.IsChecked);
            if (ctlCardSelectionExpander.IsExpanded)
            {
                ctlCardSelectionExpander.IsExpanded = false;
            }
            ProgressPercentage = 0;
            Results.Clear();
            try
            {
                IEnumerable<CribbageHandEvaluationResult> results = await Hand.EvaulateHands(Deck, ownCrib, progress);
                foreach (CribbageHandEvaluationResult result in results)
                {
                    Results.Add(result);
                }
                if (_resultCV == null)
                {
                    _resultCV = CollectionViewSource.GetDefaultView(ctlResultDataGrid.ItemsSource);
                }
            }
            catch (OperationCanceledException)
            { 
                //do nothing
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while processing: " + ex.Message, "Processing Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ctlCancelEvaluationButton_Click(object sender, RoutedEventArgs e)
        {
            if (Hand.IsEvaluating)
            {
                Hand.RequestCancel();
            }
        }

        private void HelpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
            sb.AppendLine("Terry McDonnell - 2013");
            sb.AppendFormat("Version: {0}", Assembly.GetExecutingAssembly().GetName().Version);
            sb.AppendLine();
            MessageBox.Show(sb.ToString(), "About Cribbage Hand Evaluator", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
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




    }
}
