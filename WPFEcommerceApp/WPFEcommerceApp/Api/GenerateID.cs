using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class GenerateID
    {
        static char reVal(long num)
        {
            if (num >= 0 && num < 10)
                return (char)(num + '0'); 
            else if (num >= 10 && num < 36)
                return (char)(num - 10 + 'A');  // 'A' - 'Z'
            else if (num >= 36 && num < 62)
                return (char)(num - 36 + 'a');  // 'a' - 'z'
            return 'X';  // Return 'X' if out of range
        }

        static string encode(long inputNum)
        {
            inputNum += 261816;
            inputNum = (long)(inputNum * 1.62);
            string res = "";
            while (inputNum > 0)
            {
                res += reVal(inputNum % 62); 
                inputNum /= 62;
            }
            return res;
        }

        // Sanitize the generated ID for Azure compatibility
        static string SanitizeForAzureFilename(string id)
        {
            // Remove any invalid characters for Azure storage filenames
            string sanitized = Regex.Replace(id, @"[^a-zA-Z0-9-_]", "_");

            // Ensure the filename is not too long (maximum length for Azure blob names is 1024 characters)
            if (sanitized.Length > 1024)
            {
                sanitized = sanitized.Substring(0, 1024);
            }

            return sanitized;
        }

        // Generate a unique ID
        static public async Task<string> Gen(Type type, string checkProperty = "Id")
        {
            long res = 0;

            // Query the database for the current count of records for the specified type
            await Task.Run(() => {
                using (var context = new EcommerceAppEntities())
                {
                    string t = type.Name;
                    var sql = $"SELECT COUNT(*) FROM dbo.{t}";
                    res = context.Database.SqlQuery<int>(sql).Single();
                }
            });

            bool check = false;
            string id = "";
            while (!check)
            {
                id = encode(res);

                // Ensure the ID is at least 6 characters long (pad with "0" if needed)
                while (id.Length < 6)
                {
                    id = "0" + id;  // Padding with zeroes
                }

                // Add random characters to make the ID unique and more secure
                char r1 = (char)(new Random().Next(0, 10) + '0');  // Random number
                char r2 = (char)(new Random().Next(36, 62) - 36 + 'a');  // Random letter
                id = r1 + id + r2;  // Prefix and suffix with random characters

                // Sanitize the ID for Azure Storage compatibility
                id = SanitizeForAzureFilename(id);

                // Check if the generated ID is unique in the database
                await Task.Run(() => {
                    using (var context = new EcommerceAppEntities())
                    {
                        string t = type.Name;
                        var sql = $"SELECT COUNT(1) FROM dbo.{t} WHERE {checkProperty} = '{id}'";
                        check = context.Database.SqlQuery<int>(sql).Single() == 0 ? true : false;
                    }
                });

                if (!check)
                {
                    res = (long)(res * 1.5);  // If not unique, modify the number for the next iteration
                }
            }

            return id;
        }

        // Generate a random ID of the specified length
        static public string RandomID(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Generate a unique ID based on the current DateTime
        static public string DateTimeID()
        {
            var time = DateTime.Now;
            string res = time.Year.ToString("D2") + time.Month.ToString("D2")
                + time.Day.ToString("D2") + time.Hour.ToString("D2")
                + time.Minute.ToString("D2") + time.Second.ToString("D2");
            return res;
        }
    }
}
