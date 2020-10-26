using System.Collections.Generic;

namespace CordiaManagamentTools.Api.Contracts
{
    public static class FormattingUtils
    {
        public static string CorrectHChar(string str)
        {
            var replaceTokens = new Dictionary<string, string>
            {
                { "&#225;", "á" },
                { "&#233;", "é" },
                { "&#237;", "í" },
                { "&#243;", "ó" },
                { "&#246;", "ö" },
                { "&#245;", "ő" },
                { "&#250;", "ú" },
                { "&#252;", "ü" },
                { "&#251;", "ű" },

                { "&#193;", "Á" },
                { "&#201;", "É" },
                { "&#205;", "Í" },
                { "&#211;", "Ó" },
                { "&#214;", "Ö" },
                { "&#213;", "Ő" },
                { "&#218;", "Ú" },
                { "&#220;", "Ü" },
                { "&#219;", "Ű" },
            };

            string corrected = str;

            foreach (var item in replaceTokens)
            {
                corrected = corrected.Replace(item.Key, item.Value);
            }

            return corrected;
        }
    }
}
