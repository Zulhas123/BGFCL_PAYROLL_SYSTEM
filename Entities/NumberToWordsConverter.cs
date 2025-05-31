using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    //public static class NumberToWordsConverter
    //{
    //    public static string Convert(decimal number)
    //    {
    //        if (number == 0)
    //            return "zero";

    //        var words = "";

    //        if (number < 0)
    //        {
    //            words = "minus ";
    //            number = Math.Abs(number);
    //        }

    //        int intPortion = (int)number;
    //        int fractionalPortion = (int)((number - intPortion) * 100);

    //        words += NumberToWords(intPortion)+ " Taka";

    //        if (fractionalPortion > 0)
    //        {
    //            words += " and " + NumberToWords(fractionalPortion) + " Poisa";
    //        }

    //        return words+" Only";
    //    }

    //    private static string NumberToWords(int number)
    //    {
    //        if (number == 0)
    //            return "";

    //        if (number < 0)
    //            return "minus " + NumberToWords(Math.Abs(number));

    //        string words = "";

    //        if ((number / 1000000) > 0)
    //        {
    //            words += NumberToWords(number / 1000000) + " Million ";
    //            number %= 1000000;
    //        }

    //        if ((number / 1000) > 0)
    //        {
    //            words += NumberToWords(number / 1000) + " Thousand ";
    //            number %= 1000;
    //        }

    //        if ((number / 100) > 0)
    //        {
    //            words += NumberToWords(number / 100) + " Hundred ";
    //            number %= 100;
    //        }

    //        if (number > 0)
    //        {
    //            if (words != "")
    //                words += "and ";

    //            var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
    //                               "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
    //            var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

    //            if (number < 20)
    //                words += unitsMap[number];
    //            else
    //            {
    //                words += tensMap[number / 10];
    //                if ((number % 10) > 0)
    //                    words += "-" + unitsMap[number % 10];
    //            }
    //        }

    //        return words.Trim();
    //    }
    //}


    public static class NumberToWordsConverter
    {
        public static string Convert(decimal number)
        {
            if (number == 0)
                return "Zero Only";

            var words = "";

            if (number < 0)
            {
                words = "Minus ";
                number = Math.Abs(number);
            }

            int intPortion = (int)number;
            int fractionalPortion = (int)((number - intPortion) * 100);

            words += NumberToWords(intPortion) + " Taka";

            if (fractionalPortion > 0)
            {
                words += " and " + NumberToWords(fractionalPortion) + " Poisa";
            }

            return words;
        }

        private static string NumberToWords(int number)
        {
            if (number == 0)
                return "";

            if (number < 0)
                return "Minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 10000000) > 0)
            {
                words += NumberToWords(number / 10000000) + " Crore ";
                number %= 10000000;
            }

            if ((number / 100000) > 0)
            {
                words += NumberToWords(number / 100000) + " Lakh ";
                number %= 100000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += " ";//and

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
                                "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words.Trim();
        }
    }

}
