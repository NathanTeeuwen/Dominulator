using System;

namespace Dominion.CardTypes
{
    public class Harbinger
       : Card
    {
        public static Harbinger card = new Harbinger();

        private Harbinger()
            : base("Harbinger", Expansion.Base, edition:Edition.Second, coinCost: 3, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Merchant
       : Card
    {
        public static Merchant card = new Merchant();

        private Merchant()
            : base("Merchant", Expansion.Base, edition: Edition.Second, coinCost: 3, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Vassal
       : Card
    {
        public static Vassal card = new Vassal();

        private Vassal()
            : base("Vassal", Expansion.Base, edition: Edition.Second, coinCost: 3, isAction: true, plusCoins:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Poacher
       : Card
    {
        public static Poacher card = new Poacher();

        private Poacher()
            : base("Poacher", Expansion.Base, edition: Edition.Second, coinCost: 4, isAction: true, plusCards:1, plusActions:1, plusCoins: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Bandit
       : Card
    {
        public static Bandit card = new Bandit();

        private Bandit()
            : base("Bandit", Expansion.Base, edition: Edition.Second, coinCost: 5, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Sentry
       : Card
    {
        public static Sentry card = new Sentry();

        private Sentry()
            : base("Sentry", Expansion.Base, edition: Edition.Second, coinCost: 5, isAction: true, plusCards:1, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Artisan
       : Card
    {
        public static Artisan card = new Artisan();

        private Artisan()
            : base("Artisan", Expansion.Base, edition: Edition.Second, coinCost: 6, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Lurker
       : Card
    {
        public static Lurker card = new Lurker();

        private Lurker()
            : base("Lurker", Expansion.Intrigue, edition: Edition.Second, coinCost: 2, isAction: true, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Diplomat
       : Card
    {
        public static Diplomat card = new Diplomat();

        private Diplomat()
            : base("Diplomat", Expansion.Intrigue, edition: Edition.Second, coinCost: 4, isAction: true, plusCards:2, isReaction:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Mill
       : Card
    {
        public static Mill card = new Mill();

        private Mill()
            : base("Mill", Expansion.Intrigue, edition: Edition.Second, coinCost: 4, isAction: true, plusCards: 1, plusActions: 1, victoryPoints: player => 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class SecretPassage
       : Card
    {
        public static SecretPassage card = new SecretPassage();

        private SecretPassage()
            : base("SecretPassage", Expansion.Intrigue, edition: Edition.Second, coinCost: 4, isAction: true, plusCards: 2, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Courtier
       : Card
    {
        public static Courtier card = new Courtier();

        private Courtier()
            : base("Courtier", Expansion.Intrigue, edition: Edition.Second, coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Patrol
       : Card
    {
        public static Patrol card = new Patrol();

        private Patrol()
            : base("Patrol", Expansion.Intrigue, edition: Edition.Second, coinCost: 5, isAction: true, plusCards:3)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Replace
       : Card
    {
        public static Replace card = new Replace();

        private Replace()
            : base("Replace", Expansion.Intrigue, edition: Edition.Second, coinCost: 5, isAction: true, isAttack:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }
}
