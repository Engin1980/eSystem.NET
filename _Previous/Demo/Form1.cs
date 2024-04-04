using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESystem.Forms.Extenders;
using ESystem.Extensions;

namespace Demo
{
  public partial class Form1 : Form
  {

    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      Properties.Settings.Default.Save();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      this.Show();

      TimeSpan t = DateTime.Now.TimeOfDay;

      Console.WriteLine(t.ToString("HH:mm:ss"));

    }

  }
}
