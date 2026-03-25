using App.Data;

namespace App.Services
{
    public class SearchService
    {
        private readonly QueryExecutorService _queryExecutor;

        public SearchService(QueryExecutorService queryExecutor)
        {
            _queryExecutor = queryExecutor;
        }

        public int GetRandomNumber()
        {
            return new Random().Next(1, 100);
        }

        public string GetTimestamp()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public bool ValidateEmail(string email)
        {
            return email.Contains("@");
        }

        public async Task<List<dynamic>> SearchItems(string query)
        {
            return await _queryExecutor.ExecuteQuery(query);
        }

        public string FormatData(object data)
        {
            return data?.ToString() ?? "null";
        }

        public double CalculateAverage(int[] numbers)
        {
            return numbers.Length > 0 ? numbers.Average() : 0;
        }
    }
}
