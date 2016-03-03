/* 
    PdfSearch quickly searches through all PDFs in a folder for a given text string
    Copyright(C) 2016  Stian Hvatum

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.If not, see<http://www.gnu.org/licenses/>.
*/

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PdfSearch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBoxPath.Text;
            DialogResult r = folderBrowserDialog1.ShowDialog();
            if (r == DialogResult.OK)
            {
                textBoxPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button2.Text = "Searching...";
            try
            {

            listBoxResults.Items.Clear();
            string path = textBoxPath.Text;
            string searchFor = textBox1.Text;
            if (Directory.Exists(path))
            {
                IEnumerable<string> files = Directory.EnumerateFiles(path, "*.pdf");
                foreach (string file in files)
                {
                    List<int> r = searchPdf(file, searchFor);
                    AddResult(file, r, searchFor);
                }
            }
            else if (File.Exists(path))
            {
                List<int> r = searchPdf(path, searchFor);
                AddResult(path, r, searchFor);
            }
            } finally
            {
                button2.Text = "Search";
                button2.Enabled = true;
            }
        }

        private void AddResult(string file, List<int> r, string match)
        {
            if (r.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(file).Append(": Match for \"").Append(match).Append("\" on page ");

                bool first = true;
                foreach (int i in r)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }
                    sb.Append(i);
                }
                listBoxResults.Items.Add(sb.ToString());
            }
            else
            {
                listBoxResults.Items.Add(file + ": No matches for \"" + match + "\"");
            }
        }

        private List<int> searchPdf(string file, string searchFor)
        {
            
            searchFor = searchFor.ToUpper();
            List<int> r = new List<int>();
            PdfReader reader = new PdfReader(file);
            
            for (int i = 1; i < reader.NumberOfPages; i++)
            {
                string text = PdfTextExtractor.GetTextFromPage(reader, i);
                if (text.ToUpper().Contains(searchFor))
                {
                    r.Add(i);
                }
            }
            return r;
        }
    }
}
