using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cribbage
{
    public class CribbageDeal : Hand
    {
        public override string Error
        {
            get
            {
                if (_cards.Count != 6)
                {
                    return "A cribbage deal must have six cards";
                }
                return "";
            }
        }

        public class CribbageSplit
        {
            public CribbageHand Hand
            {
                get;
                private set;
            }

            public IEnumerable<Card> Crib
            {
                get;
                private set;
            }

            public CribbageSplit(IEnumerable<Card> hand, IEnumerable<Card> crib)
            {
                this.Hand = new CribbageHand(hand);
                if(crib.Count()!=2)
                {
                    throw new ArgumentException("A crib must have two cards");
                }
                this.Crib = crib;
            }
        }

        private CancellationTokenSource cts;

        public void RequestCancel()
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }

        public async Task<IEnumerable<CribbageHandEvaluationResult>> EvaulateHands(Deck deck, bool ownCrib)
        {
            cts = new CancellationTokenSource();
            IEnumerable<CribbageHandEvaluationResult> results = await Task.Run(() => EvaluateHandsInternal(deck, ownCrib, null), cts.Token);
            return results;
        }

        public async Task<IEnumerable<CribbageHandEvaluationResult>> EvaulateHands(Deck deck, bool ownCrib, IProgress<int> progress)
        {
            cts = new CancellationTokenSource();
            IEnumerable<CribbageHandEvaluationResult> results = await Task.Run(() => EvaluateHandsInternal(deck, ownCrib, progress),cts.Token);
            return results;
        }

        private IEnumerable<CribbageHandEvaluationResult> EvaluateHandsInternal(Deck deck, bool ownCrib, IProgress<int> progress)
        {

            ConcurrentHashSet<CribbageHandEvaluationResult> results = new ConcurrentHashSet<CribbageHandEvaluationResult>();
            ParallelOptions po = new ParallelOptions();
            po.CancellationToken = cts.Token;
            po.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
            IEnumerable<CribbageSplit> splits = GetSplits();

            int total = splits.Count();
            int processed = 0;
            int progressPercentage= 0;

            //HashSet<CribbageHandEvaluationResult> partial = new HashSet<CribbageHandEvaluationResult>(); 
            //foreach (CribbageSplit split in splits)
            Parallel.ForEach<CribbageSplit, HashSet<CribbageHandEvaluationResult>>(splits, po, () => new HashSet<CribbageHandEvaluationResult>(), (split, loop, partial) =>
            {
                cts.Token.ThrowIfCancellationRequested();

                partial.Add(new CribbageHandEvaluationResult(split, ownCrib, deck, cts.Token));
                if (progress != null)
                {
                    Interlocked.Increment(ref processed);
                    if (100 * processed / total > progressPercentage)
                    {
                        progressPercentage = 100 * processed / total;
                        progress.Report(progressPercentage);
                    }
                }
                return partial;
            },
            (partialResult) => results.UnionWith(partialResult)
            );
            //}
            //results.UnionWith(partial);
            return results;
        }

        public IEnumerable<CribbageSplit> GetSplits()
        {
            List<CribbageSplit> splits = new List<CribbageSplit>();

            List<Card> cards = new List<Card>(Cards);
            Card[] array = cards.ToArray();
            Card[] crib;
            for (int crib1 = 0; crib1 < array.Length - 1; crib1++)
            {
                for (int crib2 = crib1 + 1; crib2 < array.Length; crib2++)
                {
                    crib = new Card[] { array[crib1], array[crib2] };

                    cards.Remove(array[crib1]);
                    cards.Remove(array[crib2]);
                    
                    splits.Add(new CribbageSplit(cards, crib));

                    cards.AddRange(crib);

                }
            }

            return splits;
        }

    }
}
