using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PdfSearch
{
    internal class TextMatchListBox : ListBox
    {
        List<TextMatchResult> textMatchResult = new List<TextMatchResult>();

        public TextMatchListBox()
        {
            MouseDoubleClick += TextMatchListBox_MouseDoubleClick;
            DrawMode = DrawMode.OwnerDrawFixed;
            DrawItem += TextMatchListBox_DrawItem;
            SelectionMode = SelectionMode.One;
        }

        private void TextMatchListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            Color foreground;
            TextMatchResult item = textMatchResult[e.Index];
            e.DrawBackground();
            if (item.IsMiss())
            {
                foreground = Color.Red;
            }
            else
            {
                foreground = Color.Black;
            }
            e.Graphics.DrawString(item.ToString(), e.Font, new SolidBrush(foreground), e.Bounds);
        }

        private void TextMatchListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.IndexFromPoint(e.Location);
            if (index >= 0 && index < textMatchResult.Count)
            {
                textMatchResult[index].OpenFile();
            }
        }

        internal void AddResults(List<TextMatchResult> textMatchResult)
        {
            this.textMatchResult.AddRange(textMatchResult);
            this.Items.AddRange(textMatchResult.ToArray());
        }
    }
}