using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;

namespace WinINetLogger
{
    struct ItemExtend
    {
        public string reqheader;
        public string respheader;
    }
    public partial class MainForm : Form
    {
        readonly TraceEventSession session;
        public MainForm()
        {
            InitializeComponent();
            listView.GetType()
                 .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                 .SetValue(listView, true);
            session = new TraceEventSession("WinINetLogger_guage");
            //0x4000000000000000  Microsoft-Windows-WinINet/UsageLog
            session.EnableProvider("Microsoft-Windows-WinINet", matchAnyKeywords: 0x4000000000000000);
            session.Source.Dynamic.All += OnEvent;
            new Thread(() =>
            {
                session.Source.Process();
            }).Start();
        }
        private void OnEvent(TraceEvent data)
        {
            int pid = data.ProcessID;
            string pname = "unknow";
            try { pname = Process.GetProcessById(pid).ProcessName; }
            catch { }
            var item = new ListViewItem(new string[]
            {
                string.Format("{0}({1})", pname, pid),
                data.ThreadID.ToString(),
                data.PayloadValue(0).ToString(),
                data.PayloadValue(4).ToString(),
                data.TimeStamp.ToString(),

            })
            {
                Tag = new ItemExtend
                {
                    reqheader = data.PayloadValue(2).ToString(),
                    respheader = data.PayloadValue(3).ToString(),
                }
            };
            Action action = () =>
            {
                listView.Items.Insert(0, item);
            };

            try { Invoke(action); } catch { };

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            session.Source.StopProcessing();
            session.Dispose();
        }

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && listView.SelectedItems.Count > 0)
            {
                new ItemForm(listView.SelectedItems[0]).Show();
            }
            
        }

        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right && listView.SelectedItems.Count > 0)
            {
                var text = listView.SelectedItems[0].SubItems[2].Text;
                Clipboard.SetText(text);
            }
        }
    }
}
