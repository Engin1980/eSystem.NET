namespace ESystem.Forms
{
  partial class FolderBrowser
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
      this.btnBrowse = new System.Windows.Forms.Button();
      this.txtFolderName = new ETextBox(this.components);
      this.fbd = new System.Windows.Forms.FolderBrowserDialog();
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
      // txtFolderName
      // 
      this.txtFolderName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtFolderName.BackColor = System.Drawing.SystemColors.Info;
      this.txtFolderName.EmptyBackColor = System.Drawing.SystemColors.Info;
      this.txtFolderName.EmptyForeColor = System.Drawing.SystemColors.GrayText;
      this.txtFolderName.EmptyText = "Select folder...";
      this.txtFolderName.ForeColor = System.Drawing.SystemColors.GrayText;
      this.txtFolderName.Location = new System.Drawing.Point(0, 2);
      this.txtFolderName.LostFocusBehavior = ETextBox.eLostFocusBehavior.ShowEnd;
      this.txtFolderName.Name = "txtFolderName";
      this.txtFolderName.NonEmptyBackColor = System.Drawing.SystemColors.Window;
      this.txtFolderName.NonEmptyForeColor = System.Drawing.SystemColors.WindowText;
      this.txtFolderName.Size = new System.Drawing.Size(506, 20);
      this.txtFolderName.TabIndex = 1;
      this.txtFolderName.TextChanged += new System.EventHandler(this.txtFolderName_TextChanged);
      // 
      // FolderBrowser
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.txtFolderName);
      this.Controls.Add(this.btnBrowse);
      this.Name = "FolderBrowser";
      this.Size = new System.Drawing.Size(545, 25);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnBrowse;
    private ETextBox txtFolderName;
    private System.Windows.Forms.FolderBrowserDialog fbd;
  }
}
