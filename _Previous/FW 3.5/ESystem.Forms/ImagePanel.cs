using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ESystem.Forms
{
  public partial class ImagePanel : UserControl
  {
    public ImagePanel()
    {
      InitializeComponent();
    }

    public PictureBox PictureBox
    {
      get
      {
        return this.pic;
      }
    }

    public void SetImage(Image image)
    {
      this.pic.Image = image;
    }
    public void SetImage(string pathOrUrl)
    {
      this.pic.ImageLocation = pathOrUrl;
    }

    private void pic_SizeModeChanged(object sender, EventArgs e)
    {
      this.pic.SizeMode = PictureBoxSizeMode.AutoSize;
    }
  }
}
