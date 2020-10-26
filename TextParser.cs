using System;
using System.Collections.Generic;

namespace CordiaManagamentTools.Api.Contracts
{
    public class TextParser
    {
        public enum Operator
        {
            And,
            Or,
            Not,
            Quotation
        }
        
        public static Dictionary<string, Operator> ParseString(string textIn)
        {            
            Dictionary<string, Operator> parsedString = new Dictionary<string, Operator>();
            string word = ""; Operator @operator = Operator.And; int count = 0;    

            if (textIn[0].Equals('-'))
            {
                @operator = Operator.Not;
                count = 1;
            }

            for (int i = count; i < textIn.Length; i++)
            {
                if (!(char.IsWhiteSpace(textIn[i]) || textIn[i].Equals('|') || textIn[i].Equals('-')))
                {
                    if (textIn[i].Equals('"'))
                    {
                        int leftQuote = textIn.IndexOf("\"");
                        int rightQuote = textIn.IndexOf("\"", leftQuote + 1);
                        word = textIn.Substring(leftQuote + 1, (rightQuote - (leftQuote + 1)));

                        @operator = Operator.Quotation;
                        i = rightQuote;
                    }
                    else
                    {
                        word += textIn[i];
                    }
                }

                else if (word.Length >= 3)
                {
                    try
                    {
                        parsedString.Add(word, @operator);
                    }
                    catch (ArgumentException e)
                    {
                        return parsedString;
                    }

                    if(textIn[i].Equals(' ')) { @operator = Operator.And; }
                    else if (textIn[i].Equals('|')) { @operator = Operator.Or; }
                    else if (textIn[i].Equals('-')) { @operator = Operator.Not; }

                    word = "";
                }

                if (i == textIn.Length - 1 && word.Length >= 3)
                {
                    try
                    {
                        parsedString.Add(word, @operator);
                    }
                    catch (ArgumentException e)
                    {
                        return parsedString;
                    }
                }
            }

            return parsedString;
        }
    }
}
