namespace ESystem.Forms
{
  partial class InputBox
  {
    //Form overrides dispose to clean up the component list.
    [System.Diagnostics.DebuggerNonUserCode()]
    protected override void Dispose(bool disposing)
    {
      try
      {
        if (disposing && components != null)
        {
          components.Dispose();
        }
      }
      finally
      {
        base.Dispose(disposing);
      }
    }

    //Required by the Windows Form Designer

    private System.ComponentModel.IContainer components = null;

    //NOTE: The following procedure is required by the Windows Form Designer
    //It can be modified using the Windows Form Designer.  
    //Do not modify it using the code editor.
    [System.Diagnostics.DebuggerStepThrough()]
    private void InitializeComponent()
    {
      this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.OK_Button = new System.Windows.Forms.Button();
      this.Cancel_Button = new System.Windows.Forms.Button();
      this.lblText = new System.Windows.Forms.Label();
      this.txtAnswer = new System.Windows.Forms.TextBox();
      this.TableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // TableLayoutPanel1
      // 
      this.TableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.TableLayoutPanel1.ColumnCount = 2;
      this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.TableLayoutPanel1.Controls.Add(this.OK_Button, 0, 0);
      this.TableLayoutPanel1.Controls.Add(this.Cancel_Button, 1, 0);
      this.TableLayoutPanel1.Location = new System.Drawing.Point(282, 82);
      this.TableLayoutPanel1.Name = "TableLayoutPanel1";
      this.TableLayoutPanel1.RowCount = 1;
      this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.TableLayoutPanel1.Size = new System.Drawing.Size(146, 29);
      this.TableLayoutPanel1.TabIndex = 0;
      // 
      // OK_Button
      // 
      this.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.OK_Button.Location = new System.Drawing.Point(3, 3);
      this.OK_Button.Name = "OK_Button";
      this.OK_Button.Size = new System.Drawing.Size(67, 23);
      this.OK_Button.TabIndex = 0;
      this.OK_Button.Text = "OK";
      // 
      // Cancel_Button
      // 
      this.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.Cancel_Button.Location = new System.Drawing.Point(76, 3);
      this.Cancel_Button.Name = "Cancel_Button";
      this.Cancel_Button.Size = new System.Drawing.Size(67, 23);
      this.Cancel_Button.TabIndex = 1;
      this.Cancel_Button.Text = "Cancel";
      // 
      // lblText
      // 
      this.lblText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.lblText.Location = new System.Drawing.Point(12, 7);
      this.lblText.Name = "lblText";
      this.lblText.Size = new System.Drawing.Size(416, 45);
      this.lblText.TabIndex = 1;
      this.lblText.Text = "Label1";
      // 
      // txtAnswer
      // 
      this.txtAnswer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtAnswer.Location = new System.Drawing.Point(12, 55);
      this.txtAnswer.Name = "txtAnswer";
      this.txtAnswer.Size = new System.Drawing.Size(416, 20);
      this.txtAnswer.TabIndex = 2;
      this.txtAnswer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAnswer_KeyDown);
      // 
      // InputBox
      // 
      this.AcceptButton = this.OK_Button;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.Cancel_Button;
      this.ClientSize = new System.Drawing.Size(440, 123);
      this.Controls.Add(this.txtAnswer);
      this.Controls.Add(this.lblText);
      this.Controls.Add(this.TableLayoutPanel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "InputBox";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "InputBox";
      this.Activated += new System.EventHandler(this.InputBox_Activated);
      this.TableLayoutPanel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
    private System.Windows.Forms.Button OK_Button;
    private System.Windows.Forms.Button Cancel_Button;
    private System.Windows.Forms.Label lblText;
    private System.Windows.Forms.TextBox txtAnswer;

  }
}