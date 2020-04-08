namespace Demo
{
  partial class Form2
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.folderBrowser1 = new ESystem.Forms.FolderBrowser();
      this.SuspendLayout();
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(18, 230);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(406, 20);
      this.textBox1.TabIndex = 1;
      this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(32, 104);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "label1";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(32, 208);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(35, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "label2";
      // 
      // folderBrowser1
      // 
      this.folderBrowser1.Location = new System.Drawing.Point(18, 13);
      this.folderBrowser1.Name = "folderBrowser1";
      this.folderBrowser1.RelativePath = "C:\\Program Files\\Microsoft Visual Studio 9.0\\Common7\\IDE";
      this.folderBrowser1.FolderName = "";
      this.folderBrowser1.Size = new System.Drawing.Size(545, 25);
      this.folderBrowser1.TabIndex = 4;
      this.folderBrowser1.Title = "Select folder";
      this.folderBrowser1.FolderNameChanged += new System.EventHandler(this.folderBrowser1_FolderNameChanged);
      this.folderBrowser1.Validated += new System.EventHandler(this.folderBrowser1_Validated);
      this.folderBrowser1.Validating += new System.ComponentModel.CancelEventHandler(this.folderBrowser1_Validating);
      // 
      // Form2
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(692, 266);
      this.Controls.Add(this.folderBrowser1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.textBox1);
      this.Name = "Form2";
      this.Text = "Form2";
      this.Load += new System.EventHandler(this.Form2_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private ESystem.Forms.FolderBrowser folderBrowser1;

  }
}