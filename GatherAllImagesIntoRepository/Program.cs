using System;
using Generic = System.Collections.Generic;

namespace GatherAllImagesIntoRepository
{
    class Program
    {
        static void Main(string[] args)
        {
            var missingCards = new Generic.List<Dominion.Card>();

            var cardImagesList = new CardImagesList("c:\\trash\\CardImagesList.txt");
            var destinationDir = "C:\\Dropbox\\nathant\\Projects\\Dominion\\Dominulator\\Resources";

            foreach(var card in Dominion.Cards.AllCardsList)
            {
                if (card == Dominion.Cards.Ruins || card == Dominion.Cards.Prize)
                    continue;
                var imagePath = cardImagesList.GetPathForCard(card);
                if (imagePath != null)
                {
                    System.Console.WriteLine(imagePath);
                    var destPath = System.IO.Path.Combine(destinationDir, card.ProgrammaticName + System.IO.Path.GetExtension(imagePath));
                    if (!System.IO.File.Exists(destPath))
                        System.IO.File.Copy(imagePath, destPath, overwrite:false);
                }
                else
                    missingCards.Add(card);
            }

            if (missingCards.Count > 0)
            {
                System.Console.WriteLine("Fail");
                foreach (var card in missingCards)
                    System.Console.WriteLine(card.ProgrammaticName);
            }
            else
            {
                System.Console.WriteLine("Success");
            }
        }
    }

    class CardImagesList
    {
        Generic.Dictionary<string, string> mpProgramaticNameToPath = new Generic.Dictionary<string, string>();

        public CardImagesList(string imagesFileName)
        {
            using (System.IO.TextReader fileStream = new System.IO.StreamReader(imagesFileName))
            {
                while (true)
                {
                    var filePath = fileStream.ReadLine();
                    if (filePath == null)
                        break;
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                    var programmaticName = Dominion.Card.GetProgrammaticName(fileName).ToLower();
                    System.Console.WriteLine("Loaded {0}", programmaticName);
                    this.mpProgramaticNameToPath[programmaticName] = filePath;
                }
            }
        }
        
        public string GetPathForCard(Dominion.Card card)
        {
            string result = null;
            this.mpProgramaticNameToPath.TryGetValue(card.ProgrammaticName.ToLower(), out result);
            return result;
        }
    }
}
