using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public interface ISpecidesMutator<TSpecies>
    {
        TSpecies Mutate(TSpecies member);
    }

    public interface IScoreSpecies<TSpecies>
    {
        double Compare(TSpecies left, TSpecies right);
    }
    
    public class GeneticAlgorithm<TSpecies, TSpeciesMutator, TScoreSpecies>
        where TSpeciesMutator : ISpecidesMutator<TSpecies>
        where TScoreSpecies : IScoreSpecies<TSpecies>
    {        
        public TSpecies[] currentMembers;
        private int nextMembersCount;
        public SpeciesScorePair[] nextMembers;
        private TScoreSpecies comparable;
        private UniqueSpeciesGenerator mutator;
        private readonly Random random;

        private const int childMemberCount = 10;
        private const int numberOfGamesToPlayWhenScoring = 33;

        public GeneticAlgorithm(IEnumerable<TSpecies> initialPopulation, TSpeciesMutator mutator, TScoreSpecies comparable, Random random)
        {
            this.mutator = new UniqueSpeciesGenerator(mutator);
            this.comparable = comparable;
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
            this.mutator.Reset();

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
                    for (int j = 0; j < this.random.Next(5); ++j)
                        next = this.mutator.GetUniqueSpecies(next);
                    
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

        private void ScoreCurrentPlayers()
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

        private TSpecies PickRandomCompetitor(int excluded)
        {
            int competitorIndex = excluded;
            while (competitorIndex == excluded)
            {
                competitorIndex = this.random.Next(this.nextMembersCount);
            }

            return this.nextMembers[competitorIndex].species;
        }

        class UniqueSpeciesGenerator
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
                while (true)
                {
                    TSpecies result = this.mutator.Mutate(member);

                    
                    if (this.speciesGenerator.Contains(result))
                    {
                        continue;
                    }

                    this.speciesGenerator.Add(result);
                    return result;
                }
            }
        }
    }    
}
