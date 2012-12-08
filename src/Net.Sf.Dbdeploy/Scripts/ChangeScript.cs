using System;
using System.IO;
using System.Text;

namespace Net.Sf.Dbdeploy.Scripts
{
    public class ChangeScript : IComparable
    {
        private const string UndoToken = "--//@UNDO";

        private readonly int id;

        private readonly FileInfo file;
        
        private readonly String description;

        private readonly Encoding encoding;

        public ChangeScript(int id) 
            : this(id, "test")
        {
        }

        public ChangeScript(int id, FileInfo file, Encoding encoding)
        {
            this.id = id;
            this.file = file;
            this.description = file.Name;
            this.encoding = encoding;
        }

        public ChangeScript(int id, String description)
        {
            this.id = id;
            this.file = null;
            this.description = description;
        }

        public FileInfo GetFile()
        {
            return this.file;
        }

        public int GetId()
        {
            return this.id;
        }

        public String GetDescription()
        {
            return this.description;
        }

        public int CompareTo(object obj)
        {
            ChangeScript other = (ChangeScript)obj;
            return this.id.CompareTo(other.id);
        }

        public virtual string GetContent()
        {
            return this.GetFileContents(undo: false);
        }

        public virtual string GetUndoContent()
        {
            return this.GetFileContents(undo: true);
        }

        private string GetFileContents(bool undo = false)
        {
            StringBuilder result = new StringBuilder();

            bool foundUndo = false;

            using (var input = new StreamReader(this.file.FullName, this.encoding))
            {
                string str;
                while ((str = input.ReadLine()) != null)
                {
                    // Just keep looping until we find the magic "--//@UNDO"
                    if (UndoToken == str.Trim())
                    {
                        foundUndo = true;                        
                        continue;
                    }

                    if (foundUndo == undo)
                    {
                        result.AppendLine(str);
                    }
                }
            }

            return result.ToString();
        }

        public override string ToString()
        {
            return "#" + this.id + ": " + this.description;
        }
    }
}