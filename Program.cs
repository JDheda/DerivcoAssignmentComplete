using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace DerivcoAssessment
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Derivco Graduate Assessment: Good Match \n");

            // Please EDIT file paths for your own CSV file
            string filePath = @"C:\Users\joshd\Documents\C#\DerivcoAssessment\People.csv";
            string outputFilePath = @"C:\Users\joshd\Documents\C#\DerivcoAssessment\Output.txt";
            // Counting number of lines in CSV file
            var lineCount = File.ReadAllLines(filePath).Length;
            // Reading file and storing to 2D string array
            string[,] fileData = ReadStringCSVFile(filePath);
            
            // Declaring string lists for males and females (and genderless in case not specified)
            // Note: it's longer than it needs to be
            string[] males = new string[lineCount];
            string[] females = new string[lineCount];
            string[] genderless = new string[lineCount];

            // For counting males and females
            int maleCount = 0;
            int femaleCount = 0;
            int genderlessCount = 0;

            // Looping through 2nd column of fileData and adding to males if "m" and females if "f"
            for (int i = 0; i < lineCount; i++)
            {
                if (fileData[i,1] == "m")
                {
                    males[maleCount] = fileData[i, 0];
                    maleCount++;
                }
                else if (fileData[i,1] == "f")
                {
                    females[femaleCount] = fileData[i, 0];
                    femaleCount++;
                }
                else
                {
                    genderless[genderlessCount] = fileData[i, 0];
                    genderlessCount++;
                }
            }
            // Now need to remove all them nulls (because we didn't know in advance how many males and females)
            males = males.Where(c => c != null).ToArray();
            females = females.Where(c => c != null).ToArray();
            // Lets just get rid off duplicates while we're at it
            males = males.Distinct().ToArray();
            females = females.Distinct().ToArray();
            // Ideal time to sort alphabetically to save us the pain later
            Array.Sort(males);
            Array.Sort(females);
            // Now we know correct number of males and females
            maleCount = males.Length;
            femaleCount = females.Length;
            Console.WriteLine("There are {0} males in CSV file", maleCount);
            Console.WriteLine("There are {0} females in CSV file", femaleCount);

            if (maleCount == 0 || femaleCount == 0)
            {
                Console.WriteLine("There are either no males or no females to match");
            }
            else
            {
                FinalFunction(males, females, outputFilePath);
            }

        }
        static void FinalFunction(string[] males, string[] females, string outputFilePath)
        {
            string maleName;
            string femaleName;
            int maleCount = males.Length;
            int femaleCount = females.Length;
            string[,] resultsTable = new string[maleCount * femaleCount, 3];
            string[] mainResults = new string[maleCount * femaleCount];
            int matchPercent;
            int[] matchPercentList = new int[maleCount * femaleCount];
            string resultString;
            int k = 0;
            for (int m = 0; m < maleCount; m++)
            {
                maleName = males[m];
                for (int f = 0; f < femaleCount; f++)
                {
                    femaleName = females[f];
                    matchPercent = MainScript(maleName, femaleName);
                    resultsTable[k, 0] = maleName;
                    resultsTable[k, 1] = femaleName;
                    resultsTable[k, 2] = matchPercent.ToString();
                    resultString = maleName + " matches " + femaleName + " " + matchPercent.ToString() + "%";
                    matchPercentList[k] = matchPercent;
                    if (matchPercent > 80)
                    {
                        resultString = resultString + ", good match";
                    }
                    mainResults[k] = resultString;
                    k = k + 1;
                }
            }

            string tempRow;
            int tempVal;
            int j = 0;

            while (j < maleCount * femaleCount - 1)
            {
                if (matchPercentList[j + 1] > matchPercentList[j])
                {
                    tempRow = mainResults[j];
                    tempVal = matchPercentList[j];

                    mainResults[j] = mainResults[j + 1];
                    matchPercentList[j] = matchPercentList[j + 1];

                    mainResults[j + 1] = tempRow;
                    matchPercentList[j + 1] = tempVal;
                    j = 0;
                }
                else
                {
                    j++;
                }
            }
            WriteArrayToFile(mainResults, outputFilePath);

        }
        static string[,] ReadStringCSVFile(string filePath)
        {
            var lineCount = File.ReadAllLines(filePath).Length;
            //Console.WriteLine(lineCount);
            string[,] fileData = new string[lineCount, 2];

            StreamReader reader = null;
            if (File.Exists(filePath))
            {
                reader = new StreamReader(File.OpenRead(filePath));
                int r = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    int c = 0;
                    foreach (var item in values)
                    {
                        if (item.Any(x => Char.IsWhiteSpace(x)))
                        {
                            string item1 = DelChar(item, ' ');
                            fileData[r, c] = item1.ToLower();
                        }
                        else
                        {
                            fileData[r, c] = item;
                        }
                        c = c + 1;
                    }
                    r = r + 1;
                }
            }
            else
            {
                Console.WriteLine("File doesn't exist");
            }
            return fileData;
        }
        static int CharCounter(string s, char c)
        {
            // CharCounter(string s, char c) returns an integer number of characters c inside string s
            int count = 0;
            int nChars = s.Length;
            for (int i = 0; i < nChars; i++)
            {
                if (s[i].Equals(c))
                {
                    count = count + 1;
                }
            }
            return count;
        }
        static string DelCharAtIndex(string S, int i)
        {
            // DelCharAtIndex(string S, int i) returns a string omitting a character at a specific index i position in given string
            string s;
            if (S.Length > 0)
            {
                if (i == 0)
                {
                    s = S.Substring(1, S.Length - 1);
                }
                else if (i == S.Length - 1)
                {
                    s = S.Substring(0, S.Length - 1);
                }
                else
                {
                    s = S.Substring(0, i) + S.Substring(i + 1, S.Length - (i + 1));
                }
            }
            else
            {
                s = "";
            }
            return s;
        }
        static string DelChar(string S, char c)
        {
            // DelChar(string S, char c) returns a string after omitting a specific character c from given string s
            bool condition = true;
            int i = 0;
            while (condition)
            {
                if (S[i].Equals(c))
                {
                    S = DelCharAtIndex(S, i);
                    i = i - 1;
                }
                if (S.Length <= i+1)
                {
                    condition = false;
                    break;
                }
                i = i + 1;
            }
            return S;
        }
        static int AddFirstAndLast(string s)
        {
            // AddFirstAndLast(string s) (string s is made up of numeric characters only)
            // converts the first and last character in string to an integer and sums them and returns the result
            char s0 = s[0];
            char sn = s[s.Length - 1];
            int is0 = CharToInt(s0);
            int isn = CharToInt(sn);
            int sum = is0 + isn;
            return sum;
        }
        static string DelFirstAndLast(string s)
        {
            // DelFirstAndLast(string s) takes string s and returns new string with the first and last characters omitted
            s = DelCharAtIndex(s, 0);
            s = DelCharAtIndex(s, s.Length - 1);
            return s;
        }
        static int CharToInt(char c)
        {
            // CharToInt(char c) takes char c (applicable to numeric characters only) and returns its corresponding integer value
            int z = c - '0';
            return z;
        }
        static string ReductionRule(string s)
        {
            // ReductionRule(string s) is the reducing algorithm described by the problem statement
            string sNew = "";
            while (s.Length > 1)
            {
                sNew = sNew + AddFirstAndLast(s).ToString();
                s = DelFirstAndLast(s);
            }
            if (s.Length == 1)
            {
                sNew = sNew + s[0];
            }
            Console.WriteLine(sNew);
            return sNew;
        }
        static int MatchChecker(string name1, string name2)
        {
            // MatchChecker(string name1, string name2) takes 2 names and applies the algorithm to obtain match percent
            string s = name1 + " matches " + name2;
            Console.WriteLine(s);
            s = s.ToLower();
            s = DelChar(s, ' ');

            char s0;
            int FirstCharCount;
            string CharCountString = "";
            
            while (s.Length > 0)
            {
                s0 = s[0];
                FirstCharCount = CharCounter(s, s0);
                CharCountString = CharCountString + FirstCharCount;
                Console.WriteLine(CharCountString);
                s = DelChar(s, s0);
                Console.WriteLine(s);
            }
            string sNew = CharCountString;
            while (sNew.Length > 2)
            {
                sNew = ReductionRule(sNew);
            }
            Console.WriteLine(sNew);
            int matchPercent = Convert.ToInt32(sNew);

            string result = name1 + " matches " + name2 + " " + matchPercent.ToString() + "%";
            if (matchPercent > 80)
            {
                result = result + ", good match";
            }
            Console.WriteLine(result);
            return matchPercent;
        }
        static int MainScript(string name1, string name2)
        {
            bool result1 = name1.All(Char.IsLetter);
            bool result2 = name2.All(Char.IsLetter);
            int matchPercent = 0;

            if ((result1 && result2) == false)
            {
                Console.WriteLine("Error: At least one name entered does not contain valid characters");
                
            }
            else
            {
                matchPercent = MatchChecker(name1, name2);
            }
            return matchPercent;
        }
        static void WriteArrayToFile(Array array, string fileName)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                foreach (Object obj in array)
                {
                    file.WriteLine(obj.ToString() + ",");
                }
            }
        }
    }
}
