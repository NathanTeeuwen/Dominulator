namespace Dominulator
{
    public class Expansion
    {
        public string Name { get; private set; }
        public ExpansionIndex Index { get; private set; }
        private DependencyObjectDecl<bool, DefaultTrue> isEnabled;

        public Dominion.Expansion DominionExpansion 
        { 
            get
            {
                return DominionCard.GetExpansionForIndex(this.Index);
            }
        }

        public Expansion(string name, ExpansionIndex index)
        {
            this.Name = name;
            this.Index = index;
            this.isEnabled = new DependencyObjectDeclWithSettings<bool, DefaultTrue>(this, SettingsString());
        }

        public DependencyObjectDecl<bool, DefaultTrue> IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
        }

        private string SettingsString()
        {
            return "Is Expansion Enabled" + this.Name;
        }
    }

    public enum ExpansionIndex
    {
        Adventures = 0,
        Alchemy = 1,
        Base = 2,
        Cornucopia = 3,
        DarkAges = 4,
        Guilds = 5,
        Hinterlands = 6,
        Intrigue = 7,
        Promo = 8,
        Prosperity = 9,
        Seaside = 10,
        Empires = 11,
        _Unknown = 11,
        _Count = 12
    }
}
