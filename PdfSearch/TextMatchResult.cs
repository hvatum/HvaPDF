using System;
using System.IO;
using System.Windows.Forms;

namespace PdfSearch
{
    internal class TextMatchResult
    {
        public int page;
        public string surroundingText;
        private string file;
        private string searchFor;
        private bool miss;

        public TextMatchResult(string file, int page, string surroundingText, string searchFor)
        {
            this.miss = false;
            this.file = file;
            this.page = page;
            this.surroundingText = surroundingText;
            this.searchFor = searchFor;
        }

        public TextMatchResult(string file, string searchFor)
        {
            this.miss = true;
            this.file = file;
            this.searchFor = searchFor;
        }

        internal void OpenFile()
        {
            System.Diagnostics.Process.Start(file);
        }

        internal bool IsMiss()
        {
            return miss;
        }

        public override string ToString()
        {
            if (miss)
            {
                return Path.GetFileNameWithoutExtension(file) + ": No hit...";
            }
            else
            {
                return Path.GetFileNameWithoutExtension(file) + ": Page " + page.ToString() + " - " + surroundingText;
            }
        }

        public string GetTooltip()
        {
            return file;
        }
    }

}