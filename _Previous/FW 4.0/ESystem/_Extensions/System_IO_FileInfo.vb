Option Explicit On
Option Strict On

Imports System.Runtime.CompilerServices
Imports System
Imports System.Runtime.InteropServices

Namespace Extensions

  Public Module System_IO_FileInfo

#Region "Icon extractor"

    Public Enum eIconSize
      Small
      Large
    End Enum

    ''' <summary> 
    ''' 
    ''' http://www.vbforums.com/showthread.php?t=330985
    ''' 
    ''' Extracts the icon associated with any file on your system. 
    ''' Author: WidgetMan http://softwidgets.com 
    ''' 
    ''' </summary> 
    ''' <remarks> 
    ''' 
    ''' Class requires the IconSize enumeration that is implemented in this 
    ''' same file. For best results, draw an icon from within a control's Paint 
    ''' event via the e.Graphics.DrawIcon method. 
    ''' 
    ''' </remarks> 

    Private Const SHGFI_ICON As UInteger = &H100
    Private Const SHGFI_LARGEICON As UInteger = &H0
    Private Const SHGFI_SMALLICON As UInteger = &H1

    <StructLayout(LayoutKind.Sequential)> _
    Private Structure SHFILEINFO
      Public hIcon As IntPtr
      Public iIcon As IntPtr
      Public dwAttributes As UInteger
      <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)> _
      Public szDisplayName As String
      <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=80)> _
      Public szTypeName As String
    End Structure

    <DllImport("shell32.dll")> _
    Private Function SHGetFileInfo(ByVal pszPath As String, ByVal dwFileAttributes As UInteger, ByRef psfi As SHFILEINFO, ByVal cbSizeFileInfo As UInteger, ByVal uFlags As UInteger) As IntPtr
    End Function

#End Region 'Icon extractor

    <Extension()> _
    Public Function GetFileIcon(ByVal fileInfo As System.IO.FileInfo, ByVal Size As eIconSize) As System.Drawing.Icon
      Dim hIcon As IntPtr
      Dim shinfo As New SHFILEINFO()

      If Size = eIconSize.Large Then
        hIcon = SHGetFileInfo(fileInfo.FullName, 0, shinfo, CUInt(Marshal.SizeOf(shinfo)), SHGFI_ICON Or SHGFI_LARGEICON)
      Else
        hIcon = SHGetFileInfo(fileInfo.FullName, 0, shinfo, CUInt(Marshal.SizeOf(shinfo)), SHGFI_ICON Or SHGFI_SMALLICON)
      End If

      Return System.Drawing.Icon.FromHandle(shinfo.hIcon)
    End Function

    <Extension()> _
    Public Function GetFileIcon(ByVal fileInfo As System.IO.FileInfo) As System.Drawing.Icon
      Return GetFileIcon(fileInfo, eIconSize.Small)
    End Function


  End Module

End Namespace
