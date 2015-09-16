using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace screencapture
{
    public partial class Form1 : Form
    {
        private VideoRecorder recorder;

        public Form1()
        {
            recorder = new VideoRecorder();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Record")
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Video Files (*.avi)|*.avi*";
                sfd.FilterIndex = 1;
                sfd.DefaultExt = "avi";
                sfd.RestoreDirectory = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    button1.Text = "Stop";
                    recorder.StartRecording(sfd.FileName.ToString());
                }
                
            }
            else
            {
                button1.Text = "Record";
                recorder.EndRecording();
            }
        }
    }
}
