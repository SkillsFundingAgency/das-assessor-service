using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers
{
    public class RandomDataGenerator
    {
        const String alphabets = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const String numbers = "0123456789";
        const String specialChars = "!@£$%^&*()_+{}:<>?-=[];',./";

        public static String GenerateRandomAlphabeticString(int length)
        {
            return GenerateRandomString(alphabets, length);
        }

        public static String GenerateRandomNumber(int length)
        {
            return GenerateRandomString(numbers, length);
        }

        public static String GenerateRandomAlphanumericString(int length)
        {
            return GenerateRandomString(alphabets + numbers, length);
        }

        public static String GenerateRandomAlphanumericStringWithSpecialCharacters(int length)
        {
            return GenerateRandomString(alphabets + numbers + specialChars, length);
        }

        public static String GenerateRandomEmail()
        {
            String emailDomain = "@example.com";
            return GenerateRandomAlphanumericString(10) + DateTime.Now.Millisecond + emailDomain;
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

        private static String GenerateRandomString(String characters, int length)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat(characters, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}