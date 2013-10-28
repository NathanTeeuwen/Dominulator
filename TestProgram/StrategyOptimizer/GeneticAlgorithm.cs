using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.GeneticAlgorithm
{
    public struct Parameter
    {
        public int Value;
        public readonly int LowerBound;
        public readonly int UpperBound;

        public Parameter(int value, int lowerBound, int upperBound)
        {
            this.Value = value;
            this.LowerBound = lowerBound;
            this.UpperBound = upperBound;
        }

        public Parameter(int value)
            : this(value, 0, int.MaxValue)
        {
        }

        public bool IsInRange()
        {
            return this.LowerBound <= this.Value && this.Value <= this.UpperBound;
        }
    }

    public class Parameters
    {
        internal readonly Parameter[] parameters;

        public Parameters(params Parameter[] parameters)
        {
            this.parameters = parameters;
        }

        public Parameter[] CloneParameters()
        {
            var result = new Parameter[this.parameters.Length];
            this.parameters.CopyTo(result, 0);
            return result;
        }

        public bool Equals(Parameters other)
        {
            if (other == null)
                return false;

            if (this.parameters.Length != other.parameters.Length)
                return false;

            for (int i = 0; i < this.parameters.Length; ++i)
            {
                if (this.parameters[i].Value != other.parameters[i].Value)
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Parameters);
        }

        public override int GetHashCode()
        {
            int result = 0;
            for (int i = 0; i < this.parameters.Length; ++i)
            {
                result ^= this.parameters[i].Value;
            }
            return result;
        }
    }

    public interface ISpecidesMutator<TSpecies>
    {
        TSpecies Mutate(TSpecies member);
    }

    public interface IScoreSpeciesVsEachOther<TSpecies>
    {
        double Compare(TSpecies left, TSpecies right);
    }

    public interface IScoreSpecies<TSpecies>
    {
        double GetScore(TSpecies member);
    }

    public abstract class GeneticAlgorithm<TSpecies, TSpeciesMutator>
        where TSpeciesMutator : ISpecidesMutator<TSpecies>        
        where TSpecies : class
    {
        public TSpecies[] currentMembers;
        protected int nextMembersCount;
        public SpeciesScorePair[] nextMembers;        
        private UniqueSpeciesGenerator mutator;
        private readonly Random random;

        private const int childMemberCount = 10;
        private const int numberOfGamesToPlayWhenScoring = 33;

        public GeneticAlgorithm(IEnumerable<TSpecies> initialPopulation, TSpeciesMutator mutator, Random random)
        {
            this.mutator = new UniqueSpeciesGenerator(mutator);            
            this.currentMembers = initialPopulation.ToArray();
            this.nextMembers = new SpeciesScorePair[this.currentMembers.Length * (childMemberCount + 1)];  // enough for 10 variations plus the current members
            this.random = random;
        }

        public void RunNGenerations(int generationCount)
        {
            for (int i = 0; i < generationCount; ++i)
            {
                RunOneGeneration();
            }
        }

        public void RunOneGeneration()
        {
            CreateNextGeneration();

            while (this.nextMembersCount > this.currentMembers.Length)
            {
                this.ZeroOutScores();
                this.ScoreCurrentPlayers();
                Array.Sort(this.nextMembers, 0, this.nextMembersCount, new CompareByScoreReverse());
                this.nextMembersCount = Math.Max(this.currentMembers.Length, this.currentMembers.Length * 4 / 5);
            }

            this.KeepBestMembers();
        }

        public struct SpeciesScorePair
        {
            public TSpecies species;
            public double score;

            public SpeciesScorePair(TSpecies species, float score)
            {
                this.species = species;
                this.score = score;
            }
        }

        private struct CompareByScoreReverse
            : IComparer<SpeciesScorePair>
        {
            public int Compare(SpeciesScorePair first, SpeciesScorePair second)
            {
                return -first.score.CompareTo(second.score);
            }
        }

        private void CreateNextGeneration()
        {
            //this.mutator.Reset();

            for (int memberIndex = 0; memberIndex < this.currentMembers.Length; ++memberIndex)
            {
                TSpecies current = this.currentMembers[memberIndex];
                this.mutator.RegisterSpecies(current);
            }

            for (int memberIndex = 0; memberIndex < this.currentMembers.Length; ++memberIndex)
            {
                TSpecies current = this.currentMembers[memberIndex];
                this.nextMembers[memberIndex].species = current;
                for (int i = 0; i < childMemberCount; ++i)
                {
                    var next = current;                    
                    next = this.mutator.GetUniqueSpecies(next);
                    if (next == default(TSpecies))
                    {
                        next = current;
                    }

                    this.nextMembers[memberIndex * childMemberCount + i + this.currentMembers.Length].species = next;
                }
            }

            this.nextMembersCount = this.nextMembers.Length;
        }

        private void ZeroOutScores()
        {
            for (int i = 0; i < this.nextMembersCount; ++i)
            {
                this.nextMembers[i].score = 0;
            }
        }

        private void KeepBestMembers()
        {
            for (int i = 0; i < this.currentMembers.Length; ++i)
            {
                this.currentMembers[i] = this.nextMembers[i].species;
            }
        }

        abstract protected void ScoreCurrentPlayers();        

        private TSpecies PickRandomCompetitor(int excluded)
        {
            int competitorIndex = excluded;
            while (competitorIndex == excluded)
            {
                competitorIndex = this.random.Next(this.nextMembersCount);
            }

            return this.nextMembers[competitorIndex].species;
        }        

        private class UniqueSpeciesGenerator        
        {
            private readonly HashSet<TSpecies> speciesGenerator;
            private TSpeciesMutator mutator;

            public UniqueSpeciesGenerator(TSpeciesMutator mutator)
            {
                this.mutator = mutator;
                this.speciesGenerator = new HashSet<TSpecies>();
            }

            public void Reset()
            {
                this.speciesGenerator.Clear();
            }

            public void RegisterSpecies(TSpecies member)
            {
                this.speciesGenerator.Add(member);
            }

            public TSpecies GetUniqueSpecies(TSpecies member)
            {
                for (int count = 0; count < 100; ++count)
                {
                    TSpecies result = this.mutator.Mutate(member);

                    if (this.speciesGenerator.Contains(result))
                    {                        
                        continue;
                    }

                    this.speciesGenerator.Add(result);
                    return result;
                }

                return default(TSpecies);
            }
        }
    }
    
    public class GeneticAlgorithmPopulationAgainstSelf<TSpecies, TSpeciesMutator, TScoreSpecies>
        : GeneticAlgorithm<TSpecies, TSpeciesMutator>
        where TSpeciesMutator : ISpecidesMutator<TSpecies>
        where TScoreSpecies : IScoreSpeciesVsEachOther<TSpecies>    
        where TSpecies : class
    {                
        private TScoreSpecies comparable;        

        public GeneticAlgorithmPopulationAgainstSelf(IEnumerable<TSpecies> initialPopulation, TSpeciesMutator mutator, TScoreSpecies comparable, Random random)
            : base(initialPopulation, mutator, random)
        {            
            this.comparable = comparable;            
        }        

        override protected void ScoreCurrentPlayers()
        {
            //for (int currentCompetitorIndex = 0; currentCompetitorIndex < this.nextMembersCount; ++currentCompetitorIndex)

            Parallel.ForEach(Enumerable.Range(0, this.nextMembersCount),
                delegate(int currentCompetitorIndex)
                {                    
                    TSpecies current = this.nextMembers[currentCompetitorIndex].species;

                    /*
                    for (int gameCount = 0; gameCount < numberOfGamesToPlayWhenScoring; ++gameCount)
                    {
                        TSpecies other = PickRandomCompetitor(excluded: currentCompetitorIndex);
                        int result = this.comparable.Compare(current, other);
                        if (result > 0)
                            this.nextMembers[currentCompetitorIndex].score += 2;
                        else if (result == 0)
                            this.nextMembers[currentCompetitorIndex].score += 1;
                    }*/

                    foreach (TSpecies other in this.currentMembers)
                    {
                        this.nextMembers[currentCompetitorIndex].score += this.comparable.Compare(current, other);                        
                    }
                }
            );
        }      
    }

    public class GeneticAlgorithmAgainstConstant<TSpecies, TSpeciesMutator, TScoreSpecies>
       : GeneticAlgorithm<TSpecies, TSpeciesMutator>
        where TSpeciesMutator : ISpecidesMutator<TSpecies>
        where TScoreSpecies : IScoreSpecies<TSpecies>
        where TSpecies : class
    {
        private SpeciesScoreCache scorer;

        public GeneticAlgorithmAgainstConstant(IEnumerable<TSpecies> initialPopulation, TSpeciesMutator mutator, TScoreSpecies comparable, Random random)
            : base(initialPopulation, mutator, random)
        {
            this.scorer = new SpeciesScoreCache(comparable);
        }

        override protected void ScoreCurrentPlayers()
        {
            //for (int currentCompetitorIndex = 0; currentCompetitorIndex < this.nextMembersCount; ++currentCompetitorIndex)

            Parallel.ForEach(Enumerable.Range(0, this.nextMembersCount),
                delegate(int currentCompetitorIndex)
                {
                    TSpecies current = this.nextMembers[currentCompetitorIndex].species;
                    this.nextMembers[currentCompetitorIndex].score += this.scorer.GetScore(current);
                }
            );
        }

        private class SpeciesScoreCache
        {
            private TScoreSpecies comparable;

            private Dictionary<TSpecies, double> mapSpeciesToScore = new Dictionary<TSpecies, double>();

            public SpeciesScoreCache(TScoreSpecies comparable)
            {
                this.comparable = comparable;
            }
            
            public double GetScore(TSpecies member)
            {
                double result;
                if (!this.mapSpeciesToScore.TryGetValue(member, out result))
                {
                    result = this.comparable.GetScore(member);
                    this.mapSpeciesToScore[member] = result;
                    return result;
                }
                else
                    return result;                
            }
        }
    }
}
