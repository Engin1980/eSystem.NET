namespace ESystem.Forms
{
  partial class ImagePanel
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
      this.pnl = new System.Windows.Forms.Panel();
      this.pic = new System.Windows.Forms.PictureBox();
      this.pnl.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
      this.SuspendLayout();
      // 
      // pnl
      // 
      this.pnl.AutoScroll = true;
      this.pnl.Controls.Add(this.pic);
      this.pnl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pnl.Location = new System.Drawing.Point(0, 0);
      this.pnl.Name = "pnl";
      this.pnl.Size = new System.Drawing.Size(674, 554);
      this.pnl.TabIndex = 0;
      // 
      // pic
      // 
      this.pic.Location = new System.Drawing.Point(0, 0);
      this.pic.Name = "pic";
      this.pic.Size = new System.Drawing.Size(100, 50);
      this.pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pic.TabIndex = 0;
      this.pic.TabStop = false;
      this.pic.SizeModeChanged += new System.EventHandler(this.pic_SizeModeChanged);
      // 
      // ImagePanel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.pnl);
      this.Name = "ImagePanel";
      this.Size = new System.Drawing.Size(674, 554);
      this.pnl.ResumeLayout(false);
      this.pnl.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel pnl;
    private System.Windows.Forms.PictureBox pic;
  }
}
