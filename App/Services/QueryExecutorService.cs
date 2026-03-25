using Microsoft.EntityFrameworkCore;
using App.Data;

namespace App.Services
{
    public class QueryExecutorService
    {
        private readonly AppDbContext _context;

        public QueryExecutorService(AppDbContext context)
        {
            _context = context;
        }

        public string GenerateUUID()
        {
            return Guid.NewGuid().ToString();
        }

        public bool IsNumeric(string value)
        {
            return int.TryParse(value, out _);
        }

        public DateTime GetCurrentDate()
        {
            return DateTime.Now;
        }

        public async Task<List<dynamic>> ExecuteQuery(string sqlQuery)
        {
            var results = new List<dynamic>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlQuery;
                await _context.Database.OpenConnectionAsync();
                using (var result = await command.ExecuteReaderAsync())
                {
                    while (await result.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < result.FieldCount; i++)
                        {
                            row[result.GetName(i)] = result.GetValue(i);
                        }
                        results.Add((dynamic)row);
                    }
                }
            }
            return results;
        }

        public string ReverseString(string input)
        {
            return new string(input.Reverse().ToArray());
        }

        public int CountCharacters(string text)
        {
            return text?.Length ?? 0;
        }
    }
}
