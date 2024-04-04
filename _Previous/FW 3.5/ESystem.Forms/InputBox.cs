using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ESystem.Forms
{
  public partial class InputBox : Form
  {
    private InputBox()
    {
      InitializeComponent();
    }

    public static string ShowDialog(string caption, string infoText, string defaultAnswer)
    {
      InputBox ib = new InputBox();

      ib.Text = caption;
      ib.lblText.Text = infoText;
      ib.txtAnswer.Text = defaultAnswer;

      ib.txtAnswer.Focus();

      ib.ShowDialog();

      string ret = ib.Result;

      return ret;
    }

    public string Result
    {
      get
      {
        if ((this.DialogResult == System.Windows.Forms.DialogResult.OK))
        {
          return txtAnswer.Text;
        }
        else
        {
          return null;
        }
      }
    }

    private void OK_Button_Click(System.Object sender, System.EventArgs e)
    {
      this.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.Close();
    }

    private void Cancel_Button_Click(System.Object sender, System.EventArgs e)
    {
      this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.Close();
    }

    private void InputBox_Activated(object sender, EventArgs e)
    {
      txtAnswer.Focus();
    }

    private void txtAnswer_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Escape)
      {
        this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.Hide();
      }
      else if (e.KeyCode == Keys.Enter)
      {
        this.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.Hide();
      }
    }

  }
}
