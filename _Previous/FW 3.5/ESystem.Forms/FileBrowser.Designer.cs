namespace ESystem.Forms
{
  partial class FileBrowser
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.ofd = new System.Windows.Forms.OpenFileDialog();
      this.sfd = new System.Windows.Forms.SaveFileDialog();
      this.btnBrowse = new System.Windows.Forms.Button();
      this.txtFileName = new ETextBox(this.components);
      this.SuspendLayout();
      // 
      // btnBrowse
      // 
      this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.btnBrowse.Image = global::ESystem.Forms.Properties.Resources.open_icon;
      this.btnBrowse.Location = new System.Drawing.Point(512, 0);
      this.btnBrowse.Name = "btnBrowse";
      this.btnBrowse.Size = new System.Drawing.Size(30, 23);
      this.btnBrowse.TabIndex = 0;
      this.btnBrowse.UseVisualStyleBackColor = true;
      this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
      // 
      // txtFileName
      // 
      this.txtFileName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtFileName.BackColor = System.Drawing.SystemColors.Info;
      this.txtFileName.EmptyBackColor = System.Drawing.SystemColors.Info;
      this.txtFileName.EmptyForeColor = System.Drawing.SystemColors.GrayText;
      this.txtFileName.EmptyText = "Select file...";
      this.txtFileName.ForeColor = System.Drawing.SystemColors.GrayText;
      this.txtFileName.Location = new System.Drawing.Point(0, 2);
      this.txtFileName.LostFocusBehavior = ETextBox.eLostFocusBehavior.ShowEnd;
      this.txtFileName.Name = "txtFileName";
      this.txtFileName.NonEmptyBackColor = System.Drawing.SystemColors.Window;
      this.txtFileName.NonEmptyForeColor = System.Drawing.SystemColors.WindowText;
      this.txtFileName.Size = new System.Drawing.Size(506, 20);
      this.txtFileName.TabIndex = 1;
      // 
      // FileBrowser
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.txtFileName);
      this.Controls.Add(this.btnBrowse);
      this.Name = "FileBrowser";
      this.Size = new System.Drawing.Size(545, 25);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnBrowse;
    private ETextBox txtFileName;
    private System.Windows.Forms.OpenFileDialog ofd;
    private System.Windows.Forms.SaveFileDialog sfd;
  }
}
