using Microsoft.EntityFrameworkCore;

namespace App.Services
{
    public class QueryExecutorService
    {

        public QueryExecutorService()
        {
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
