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
        private ListBox listBoxResults;

        public Form1()
        {
            InitializeComponent();
            tabControl1.DrawItem += TabControl1_DrawItem;
            tabControl1.MouseDown += TabControl1_MouseDown;
            textBoxSearch.FindForm().AcceptButton = button2;
            textBoxPath.Text = System.Environment.GetEnvironmentVariable("USERPROFILE");
        }

        private Rectangle getCloseRect(Rectangle bounds)
        {
            return new Rectangle(bounds.Left + 5, bounds.Top + 8, 6, 6);
        }

        private void TabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            //Looping through the controls.
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
                //Getting the position of the "x" mark.
                Rectangle r = tabControl1.GetTabRect(i);

                Rectangle closeButton = getCloseRect(r);
                if (closeButton.Contains(e.Location))
                {
                    this.tabControl1.TabPages.RemoveAt(i);
                    break;
                }
            }
        }

        private void TabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            //This code will render a "x" mark at the end of the Tab caption. 
            Pen pen = new Pen(Color.Black, 2);
            Rectangle closeRect = getCloseRect(e.Bounds);
            e.Graphics.DrawLine(pen, closeRect.X, closeRect.Y, closeRect.Right, closeRect.Bottom);
            e.Graphics.DrawLine(pen, closeRect.Right, closeRect.Y, closeRect.X, closeRect.Bottom);
            pen = new Pen(Color.LightGray, 1);
            e.Graphics.DrawRectangle(pen, closeRect);
            e.Graphics.DrawString(this.tabControl1.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 12, e.Bounds.Top + 4);
            e.DrawFocusRectangle();
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
                listBoxResults = new ListBox();
                TabPage tc = new TabPage(textBoxSearch.Text + "    ");
                tc.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                listBoxResults.Dock = DockStyle.Fill;
                tc.Controls.Add(listBoxResults);
                tabControl1.Controls.Add(tc);
                tabControl1.SelectedIndex = tabControl1.TabCount - 1;
                string path = textBoxPath.Text;
                string searchFor = textBoxSearch.Text;
                if (Directory.Exists(path))
                {
                    IEnumerable<string> files = Directory.EnumerateFiles(path, "*.pdf");
                    foreach (string file in files)
                    {
                        List<TextMatchResult> r = searchPdf(file, searchFor);
                        AddResult(file, r, searchFor);
                    }
                }
                else if (File.Exists(path))
                {
                    List<TextMatchResult> r = searchPdf(path, searchFor);
                    AddResult(path, r, searchFor);
                }
            }
            finally
            {
                button2.Text = "Search";
                button2.Enabled = true;
            }
        }

        private void removeTc(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AddResult(string file, List<TextMatchResult> r, string match)
        {
            if (r.Count > 0)
            {
                listBoxResults.Items.AddRange(r.ToArray());
            }
            else
            {
                listBoxResults.Items.Add(System.IO.Path.GetFileNameWithoutExtension(file) + ": No matches for \"" + match + "\"");
            }
        }

        private List<TextMatchResult> searchPdf(string file, string searchFor)
        {

            searchFor = searchFor.ToUpper();
            List<TextMatchResult> r = new List<TextMatchResult>();
            PdfReader reader = new PdfReader(file);

            int before = 15;
            int after = 120;

            for (int i = 1; i < reader.NumberOfPages; i++)
            {
                string text = PdfTextExtractor.GetTextFromPage(reader, i);
                int pos = text.ToUpper().IndexOf(searchFor);
                if (pos > -1)
                {
                    int startPosition = pos > before ? pos - before : 0;
                    int len = text.Length > (startPosition + after) ? after : text.Length - startPosition - 1;
                    TextMatchResult tmr = new TextMatchResult(file, i, text.Substring(startPosition, len), searchFor);
                    r.Add(tmr);
                }
            }
            reader.Close();
            return r;
        }
    }
}
