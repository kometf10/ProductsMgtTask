using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Domin.Paging
{
    public class PagingLink
    {
        public int Page { get; set; }
        public string Text { get; set; }

        public bool Enabled { get; set; }

        public bool Active { get; set; }

        public PagingLink(int page, string text, bool enabled)
        {
            this.Page = page;
            this.Text = text;
            this.Enabled = enabled;
        }

    }
}
