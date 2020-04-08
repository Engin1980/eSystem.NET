using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ESystem.Extensions
{
  public static class System_IO_FileInfo
  {

#if SILVERLIGHT == false

    #region "Icon extractor"

    public enum eIconSize
    {
      Small,
      Large
    }

    /// <summary> 
    /// 
    /// http://www.vbforums.com/showthread.php?t=330985
    /// 
    /// Extracts the icon associated with any file on your system. 
    /// Author: WidgetMan http://softwidgets.com 
    /// 
    /// </summary> 
    /// <remarks> 
    /// 
    /// Class requires the IconSize enumeration that is implemented in this 
    /// same file. For best results, draw an icon from within a control's Paint 
    /// event via the e.Graphics.DrawIcon method. 
    /// 
    /// </remarks> 

    private const uint SHGFI_ICON = 0x100;
    private const uint SHGFI_LARGEICON = 0x0;

    private const uint SHGFI_SMALLICON = 0x1;
    [StructLayout(LayoutKind.Sequential)]
    private struct SHFILEINFO
    {
      public IntPtr hIcon;
      public IntPtr iIcon;
      public uint dwAttributes;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string szDisplayName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
      public string szTypeName;
    }

    [DllImport("shell32.dll")]
    private static extern IntPtr SHGetFileInfo(
      string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

    #endregion

    public static System.Drawing.Icon GetFileIcon(this System.IO.FileInfo fileInfo, eIconSize Size)
    {
      IntPtr hIcon = default(IntPtr);
      SHFILEINFO shinfo = new SHFILEINFO();

      if (Size == eIconSize.Large)
      {
        hIcon = SHGetFileInfo(fileInfo.FullName, 0, ref shinfo, Convert.ToUInt32(Marshal.SizeOf(shinfo)), SHGFI_ICON | SHGFI_LARGEICON);
      }
      else
      {
        hIcon = SHGetFileInfo(fileInfo.FullName, 0, ref shinfo, Convert.ToUInt32(Marshal.SizeOf(shinfo)), SHGFI_ICON | SHGFI_SMALLICON);
      }

      return System.Drawing.Icon.FromHandle(shinfo.hIcon);
    }

    public static System.Drawing.Icon GetFileIcon(this System.IO.FileInfo fileInfo)
    {
      return GetFileIcon(fileInfo, eIconSize.Small);
    }

#endif


  }

}
