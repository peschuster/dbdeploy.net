using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Net.Sf.Dbdeploy.Database
{
    public class QueryStatementSplitter
    {
        private string delimiter = ";";

        private IDelimiterType delimiterType = new NormalDelimiter();

        private string lineEnding = Database.LineEnding.Platform;

        public QueryStatementSplitter()
        {
        }

        public string Delimiter
        {
            get { return this.delimiter; }
            set { this.delimiter = value; }
        }

        public IDelimiterType DelimiterType
        {
            get { return this.delimiterType; }
            set { this.delimiterType = value; }
        }

        public string LineEnding
        {
            get { return this.lineEnding; }
            set { this.lineEnding = value; }
        }

        public virtual ICollection<string> Split(string input)
        {
            var statements = new List<string>();
            var currentSql = new StringBuilder();

            string[] lines = Regex.Split(input, "\r\n|\r|\n");
            bool insideText = false;

            foreach (string line in lines)
            {
                // Check wether line is inside of a text block
                insideText ^= (line.Length - line.Replace("'", string.Empty).Length) % 2 == 1;

                // Only trim none text lines.
                string strippedLine = insideText ? line : line.TrimEnd();
                
                // Allow empty lines inside of text blocks.
                if (!insideText && string.IsNullOrEmpty(strippedLine))
                    continue;

                if (currentSql.Length != 0)
                {
                    currentSql.Append(this.lineEnding);
                }

                currentSql.Append(strippedLine);

                // don't split query inside of a text block
                if (!insideText && this.delimiterType.Matches(strippedLine, this.delimiter))
                {
                    statements.Add(currentSql.ToString(0, currentSql.Length - this.delimiter.Length));

                    // Clear StringBuilder
                    currentSql.Length = 0;
                }
            }

            if (currentSql.Length != 0)
            {
                statements.Add(currentSql.ToString());
            }

            return statements;
        }
    }
}