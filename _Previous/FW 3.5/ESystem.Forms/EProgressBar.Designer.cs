namespace ESystem.Forms
{
  partial class EProgressBar
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
      this.pnlOuter = new System.Windows.Forms.Panel();
      this.pnlInner = new System.Windows.Forms.Panel();
      this.pnlOuter.SuspendLayout();
      this.SuspendLayout();
      // 
      // pnlOuter
      // 
      this.pnlOuter.Controls.Add(this.pnlInner);
      this.pnlOuter.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pnlOuter.Location = new System.Drawing.Point(0, 0);
      this.pnlOuter.Name = "pnlOuter";
      this.pnlOuter.Size = new System.Drawing.Size(146, 146);
      this.pnlOuter.TabIndex = 0;
      // 
      // pnlInner
      // 
      this.pnlInner.BackColor = System.Drawing.SystemColors.Info;
      this.pnlInner.Location = new System.Drawing.Point(3, 3);
      this.pnlInner.Name = "pnlInner";
      this.pnlInner.Size = new System.Drawing.Size(89, 140);
      this.pnlInner.TabIndex = 0;
      // 
      // EngProgressBar
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.ControlDark;
      this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.Controls.Add(this.pnlOuter);
      this.ForeColor = System.Drawing.SystemColors.Info;
      this.Name = "EngProgressBar";
      this.Size = new System.Drawing.Size(146, 146);
      this.Resize += new System.EventHandler(this.EngProgressBar_Resize);
      this.pnlOuter.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel pnlOuter;
    private System.Windows.Forms.Panel pnlInner;
  }
}
