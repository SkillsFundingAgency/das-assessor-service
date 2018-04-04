using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers
{
    public class RandomDataGenerator
    {
        const string Alphabets = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string Numbers = "0123456789";
        const string SpecialChars = "!@£$%^&*()_+{}:<>?-=[];',./";

        public static string GenerateRandomAlphabeticstring(int length)
        {
            return GenerateRandomstring(Alphabets, length);
        }

        public static string GenerateRandomNumber(int length)
        {
            return GenerateRandomstring(Numbers, length);
        }

        public static string GenerateRandomAlphanumericstring(int length)
        {
            return GenerateRandomstring(Alphabets + Numbers, length);
        }

        public static string GenerateRandomAlphanumericstringWithSpecialCharacters(int length)
        {
            return GenerateRandomstring(Alphabets + Numbers + SpecialChars, length);
        }

        public static string GenerateRandomEmail()
        {
            string emailDomain = "@example.com";
            return GenerateRandomAlphanumericstring(10) + DateTime.Now.Millisecond + emailDomain;
        }

        public static int GenerateRandomDateOfMonth()
        {
            return GenerateRandomNumberBetweenTwoValues(1, 28);
        }

        public static int GenerateRandomMonth()
        {
            return GenerateRandomNumberBetweenTwoValues(1, 13);
        }

        public static int GenerateRandomNumberBetweenTwoValues(int min, int max)
        {
            Random rand = new Random();
            return rand.Next(min, max);
        }

        private static string GenerateRandomstring(string characters, int length)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat(characters, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}