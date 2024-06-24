using System;
using System.Collections.Generic;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace BuscoAPI.Helpers
{
    public class Utility
    {
        public static int RandomNumber(int numberOfDigits = 4)
        {
            //from the number 10^numberOfDigits
            int lowerLimit = (int) Math.Pow(10, numberOfDigits - 1);
            //up to number 10^numberOfDigits
            int upperLimit = (int) Math.Pow(10, numberOfDigits);

            Random rand = new Random();

            //example, if numberOfDigits is 1, lowerLimit = 1 and upperLimit = 10
            int randomNumber = rand.Next(lowerLimit, upperLimit);

            return randomNumber;
        }

        // Método para generar una cadena aleatoria de longitud dada
        public static string GenerateRandomString(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}