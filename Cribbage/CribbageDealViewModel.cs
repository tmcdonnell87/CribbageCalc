using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cribbage
{
    public class CribbageDealViewModel : HandViewModel
    {

        public CribbageDealViewModel():base(new CribbageDeal())
        {
            IsEvaluating = false;
        }

        private bool _isEvaluating;
        public bool IsEvaluating
        {
            get
            {
                return _isEvaluating;
            }
            private set
            {
                _isEvaluating = value;
                OnPropertyChanged("IsEvaluating");
            }
        }

        public void RequestCancel()
        {
            ((CribbageDeal)_hand).RequestCancel();
        }

        public async Task<IEnumerable<CribbageHandEvaluationResult>> EvaulateHands(Deck deck, bool ownCrib)
        {
            return await EvaulateHands(deck, ownCrib,null);
        }

        public async Task<IEnumerable<CribbageHandEvaluationResult>> EvaulateHands(Deck deck, bool ownCrib, IProgress<int> progress)
        {
            IsEvaluating = true;
            try
            {
                return await ((CribbageDeal)_hand).EvaulateHands(deck, ownCrib, progress);
            }
            finally
            {
                IsEvaluating = false;
            }
        }

    }
}
