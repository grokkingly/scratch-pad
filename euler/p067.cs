using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string line;
            List<int[]> triangle = new List<int[]>();
            string[] helper;
            int[] row;
            char[] delimiters = {' '};
        
            StreamReader file = new StreamReader(@"C:\MyWorkplace\triangle.txt");
            //read the input file into a list of int arrays
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim(); //remove the trailing spaces at the end of the lines
                helper = line.Split(delimiters);
                row = new int[helper.Length];
                for (int i = 0; i < helper.Length; i++)
                    row[i] = int.Parse(helper[i]);
                triangle.Add(row);
            }

            //starting at the last line, add the bigger of each two adjacent numbers (n, n + 1) to the "parent" number on position (n) on the line above
            //move up until reaching the top of the triangle
            //the resulting number on the first line is the answer
            for (int lineNum = triangle.Count - 1; lineNum > 0; lineNum--)
            {
                for (int rowPos = 0; rowPos < triangle[lineNum].Length - 1; rowPos++)
                {
                    triangle[lineNum - 1][rowPos] += Math.Max(triangle[lineNum][rowPos], triangle[lineNum][rowPos + 1]);
                }
            }

            Console.WriteLine(triangle[0][0]);
            Console.ReadKey();

        }
    }
}
