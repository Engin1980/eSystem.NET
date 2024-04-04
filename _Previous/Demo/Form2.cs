using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Demo
{
  public partial class Form2 : Form
  {
    public Form2()
    {
      InitializeComponent();
    }

    private void Form2_Load(object sender, EventArgs e)
    {
      //testingBindingSource.DataSource = new Testing();
      //fileBrowser1.RelativePath = @"C:\Program Files\Microsoft Visual Studio 9.0";
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
      label2.Text = textBox1.Text;
    }

    private void folderBrowser1_FolderNameChanged(object sender, EventArgs e)
    {
      label1.Text = folderBrowser1.FolderName;
    }

    private void folderBrowser1_Validating(object sender, CancelEventArgs e)
    {
      MessageBox.Show("ing");
    }

    private void folderBrowser1_Validated(object sender, EventArgs e)
    {
      MessageBox.Show("ed");
    }
  }
}
