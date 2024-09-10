using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratingRandomObject
{
    public class GeneratingService
    {
        private List<string> words;
        private Random random;
        public GeneratingService(string filePath)
        {
            words = new List<string>();
            random = new Random();
            LoadWordsFromFile(filePath);
        }
        private void LoadWordsFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] fileWords = File.ReadAllText(filePath).Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                words.AddRange(fileWords);
            }
            else
            {
                throw new FileNotFoundException($"The file at {filePath} does not exist.");
            }
        }
        private int GenerateRandomNumber()
        {
            return random.Next(1000, 10000);
        }
        private string GenerateRandomText()
        {
            if (words.Count < 10)
            {
                throw new InvalidOperationException("Not enough words in the file to generate text.");
            }

            List<string> randomWords = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                int index = random.Next(words.Count);
                randomWords.Add(words[index]);
            }

            return string.Join(" ", randomWords);
        }
        private DateOnly GenerateRandomDate()
        {
            DateTime startDate = new DateTime(2000, 1, 1);
            int range = (DateTime.Today - startDate).Days;
            DateTime randomDate = startDate.AddDays(random.Next(range));
            return DateOnly.FromDateTime(randomDate);
        }
        public IEnumerable<RandomClass> GenerateRecords(int numRecords)
        {
            List<RandomClass> records = new List<RandomClass>();
            for (int i = 1; i <= numRecords; i++)
            {
                yield return new RandomClass
                {
                    Id = i,
                    Number = GenerateRandomNumber(),
                    Text = GenerateRandomText(),
                    Date = GenerateRandomDate()
                };
            }
        }
    }
}
