using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinINetLogger
{
    public partial class ItemForm : Form
    {
        public ItemForm(ListViewItem item)
        {
            InitializeComponent();
            var subitems = item.SubItems;
            ItemExtend itemEx = (ItemExtend)item.Tag;
            Text = subitems[0].Text + " " + subitems[1].Text + " " + subitems[4].Text;
            textBox1.AppendText(subitems[2].Text + Environment.NewLine);
            textBox1.AppendText(itemEx.reqheader);
            textBox1.AppendText(itemEx.respheader);
        }
    }
}
