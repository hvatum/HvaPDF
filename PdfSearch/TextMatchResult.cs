using System.IO;

namespace PdfSearch
{
    internal class TextMatchResult
    {
        public int page;
        public string surroundingText;
        private string file;
        private int i;
        private string searchFor;
        private string v;

        public TextMatchResult(string file, int page, string surroundingText, string searchFor)
        {
            this.file = file;
            this.page = page;
            this.surroundingText = surroundingText;
            this.searchFor = searchFor;
        }

        public override string ToString()
        {
            return Path.GetFileNameWithoutExtension(file) + ": Page " + page.ToString() + " - " + surroundingText;
        }
    }

}