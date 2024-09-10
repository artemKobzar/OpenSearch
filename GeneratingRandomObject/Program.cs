namespace GeneratingRandomObject
{
    public class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\Users\Artem\Desktop\TextForSearch.txt";
            GeneratingService generatingService = new GeneratingService(filePath);

            IEnumerable<RandomClass> records = generatingService.GenerateRecords(1000);

            string csvFilePath = @"C:\Users\Artem\Desktop\output.csv"; // Path to save the CSV file
            CsvHelper csvHelperService = new CsvHelper();
            csvHelperService.WriteRecordsToCsv(records, csvFilePath);

            Console.WriteLine("CSV file has been created successfully.");

            //string filePath = @"C:\Users\Artem\Desktop\TextForSearch.txt";
            //GeneratingService service = new GeneratingService(filePath);

            //List<RandomClass> records = service.GenerateRecords(100);

            //// Print out the first few records to verify
            //foreach (var record in records.Take(5))
            //{
            //    Console.WriteLine($"Id: {record.Id}; Number: {record.Number}; Text: {record.Text}; Date: {record.Date}");
            //}
            Console.ReadLine();
        }
    }
}
