Option Explicit On
Option Strict On

Imports System.Runtime.CompilerServices
Imports System.Reflection
Imports System.Text

Namespace Extensions

  Public Module ObjectExt

    Private Const NULL As String = "(null)"


#Region "InLine"

    <Extension()> _
    Public Function ToInlineInfoString(ByVal obj As Object) As String

      If (obj Is Nothing) Then
        Return NULL
      ElseIf TypeOf obj Is String Then
        Return CType(obj, String)
      End If

      Dim cont As New StringBuilder()
      Dim ret As New System.Text.StringBuilder()
      Dim t As Type = obj.GetType()

      If TypeOf obj Is ICollection Then
        Dim ie As ICollection = CType(obj, ICollection)

        For Each fItem As Object In ie
          Dim valueString As String = _
            ToInlineInfoString(fItem)

          cont.AppendPreDelimited(valueString, ";"c)

        Next

      ElseIf TypeOf obj Is IEnumerable Then
        Dim ie As IEnumerable = CType(obj, IEnumerable)

        For Each fItem As Object In ie
          Dim valueString As String = _
            ToInlineInfoString(fItem)

          cont.AppendPreDelimited(valueString, ";"c)

        Next
      Else

        Dim props() As PropertyInfo = t.GetProperties()
        Dim mems() As MemberInfo = props.ToArray()

        Dim loc As String = GenerateInlineContent( _
          obj, mems)

        If (loc.Length > 0) Then
          cont.AppendPreDelimited(loc.ToString(), ";"c)
        End If

      End If

      If (cont.Length = 0) Then
        ret.Append(obj.ToString())
      Else
        ret.Append("{" + cont.ToString() + "}")

        ret.Insert(0, "(" + t.Name + ")")
      End If

      Return ret.ToString()
    End Function

    Private Function GenerateInlineContent( _
      ByVal obj As Object, ByVal members() As MemberInfo) As String
      Dim sb As New System.Text.StringBuilder()

      For Each Item As MemberInfo In members
        Dim fieldValue As Object = _
          obj.GetType().InvokeMember(Item.Name, BindingFlags.GetProperty, Nothing, obj, Nothing, Nothing)
        Dim fieldValueString As String = _
          ToInlineInfoString(fieldValue)

        sb.AppendPreDelimited(Item.Name + "=" + fieldValueString, ","c)
      Next

      Return sb.ToString()
    End Function
#End Region 'InLine


#Region "MultiLine"

    <Extension()> _
       Public Function ToMultilineInfoString(ByVal obj As Object) As String

      Return ToMultilineInfoString(obj, 0)
    End Function

    Private Function ToMultilineInfoString(ByVal obj As Object, ByVal level As Integer) As String

      Dim prefix As String = "  ".Repeat(level)

      If (obj Is Nothing) Then
        Return prefix & NULL
      ElseIf TypeOf obj Is String Then
        Return prefix & CType(obj, String)
      End If

      Dim cont As New StringBuilder()
      Dim ret As New System.Text.StringBuilder()
      Dim t As Type = obj.GetType()

      If TypeOf obj Is ICollection Then
        Dim ie As ICollection = CType(obj, ICollection)

        For Each fItem As Object In ie
          Dim valueString As String = _
            ToMultilineInfoString(fItem, level + 1)

          cont.AppendPreDelimited(prefix & "  " & valueString.TrimStart(), vbCrLf)

        Next

      ElseIf TypeOf obj Is IEnumerable Then
        Dim ie As IEnumerable = CType(obj, IEnumerable)

        For Each fItem As Object In ie
          Dim valueString As String = _
            ToMultilineInfoString(fItem, level + 1)

          cont.AppendPreDelimited(prefix & "  " & valueString.TrimStart(), vbCrLf)

        Next
      Else

        Dim props() As PropertyInfo = t.GetProperties()
        Dim mems() As MemberInfo = props.ToArray()

        Dim loc As String = GenerateMultilineContent( _
          obj, mems, level)

        If (loc.Length > 0) Then
          cont.AppendPreDelimited(loc.ToString(), ";"c)
        End If

      End If

      If (cont.Length = 0) Then
        ret.Append(prefix & obj.ToString())
      Else
        ret.Append(vbCrLf & cont.ToString())

        ret.Insert(0, prefix & "(" + t.Name + ")")
      End If

      Return ret.ToString()
    End Function

    Private Function GenerateMultilineContent( _
      ByVal obj As Object, ByVal members() As MemberInfo, ByVal level As Integer) As String
      Dim sb As New System.Text.StringBuilder()

      Dim prefix As String = "  ".Repeat(level) & " > "

      For Each Item As MemberInfo In members
        Dim fieldValue As Object = _
          obj.GetType().InvokeMember(Item.Name, BindingFlags.GetProperty, Nothing, obj, Nothing, Nothing)
        Dim fieldValueString As String = _
          ToMultilineInfoString(fieldValue, level + 1)

        sb.AppendPreDelimited(prefix & Item.Name + "=" + fieldValueString.TrimStart(), vbCrLf)
      Next

      Return sb.ToString()
    End Function

#End Region 'MultiLine


#Region "XML"

#Region "Nested"

    Private Const INTENDER As String = "  "
    Private Const DEFAULT_ENCAPSULATING_TAG As String = "This"
    Private Const DEFAULT_ENUMERATIONITEM_TAG As String = "Item"
    Private Const DEFAULT_NULL_TAGITEM As String = "(null)"
    Public Class Settings
      Friend useLongTypeNames As Boolean = False
      Friend expandValueTypes As Boolean = False
      Friend encapsulatingTag As String = DEFAULT_ENCAPSULATING_TAG
      Friend enumerationItemTag As String = DEFAULT_ENUMERATIONITEM_TAG
      Friend maxDepth As Integer = 3
      Friend currentDepth As Integer = 0
      Friend sortByName As Boolean = False
      Friend includeFields As Boolean = True
      Friend includeProperties As Boolean = True
      Friend expandEnumerations As Boolean = True
      Friend nullTagItem As String = DEFAULT_NULL_TAGITEM
      Friend inlineValues As Boolean = False
    End Class

    Private Class MemberInfoByNameComparer
      Implements IComparer(Of MemberInfo)

      Public Function Compare(ByVal x As System.Reflection.MemberInfo, ByVal y As System.Reflection.MemberInfo) As Integer Implements System.Collections.Generic.IComparer(Of System.Reflection.MemberInfo).Compare
        Return x.Name.CompareTo(y.Name)
      End Function

    End Class

#End Region 'Nested


#Region "Public methods"

    <Extension()> _
  Public Function ToXml(ByVal item As Object) As String
      Dim ret As String = _ToXml(item, New Settings())
      Return ret
    End Function

    <Extension()> _
 Public Function ToXml( _
   ByVal item As Object, _
    ByVal useLongTypeNames As Boolean, _
    ByVal encapsulatingTag As String) As String

      Dim sett As New Settings()
      sett.useLongTypeNames = useLongTypeNames
      sett.encapsulatingTag = encapsulatingTag

      Dim ret As String = _ToXml(item, sett)

      Return ret

    End Function

    <Extension()> _
Public Function ToXml( _
ByVal item As Object, _
ByVal useLongTypeNames As Boolean, _
ByVal encapsulatingTag As String, _
ByVal inlineValues As Boolean) As String

      Dim sett As New Settings()
      sett.useLongTypeNames = useLongTypeNames
      sett.encapsulatingTag = encapsulatingTag
      sett.inlineValues = inlineValues

      Dim ret As String = _ToXml(item, sett)

      Return ret

    End Function

    <Extension()> _
  Public Function ToXml( _
    ByVal item As Object, _
     ByVal useLongTypeNames As Boolean, _
     ByVal expandValueTypes As Boolean, _
     ByVal includeProperties As Boolean, _
     ByVal includeFields As Boolean, _
     ByVal expandEnumerations As Boolean, _
     ByVal sortByName As Boolean _
     ) As String

      Dim sett As New Settings()
      sett.useLongTypeNames = useLongTypeNames
      sett.expandValueTypes = expandValueTypes
      sett.sortByName = sortByName
      sett.includeFields = includeFields
      sett.includeProperties = includeProperties
      sett.expandEnumerations = expandEnumerations

      Dim ret As String = _ToXml(item, sett)

      Return ret

    End Function

    <Extension()> _
  Public Function ToXml( _
    ByVal item As Object, _
     ByVal useLongTypeNames As Boolean, _
     ByVal expandValueTypes As Boolean, _
     ByVal includeProperties As Boolean, _
     ByVal includeFields As Boolean, _
     ByVal expandEnumerations As Boolean, _
     ByVal sortByName As Boolean, _
     ByVal maxDepth As Integer) As String

      Dim sett As New Settings()
      sett.useLongTypeNames = useLongTypeNames
      sett.expandValueTypes = expandValueTypes
      sett.maxDepth = maxDepth
      sett.sortByName = sortByName
      sett.includeFields = includeFields
      sett.includeProperties = includeProperties
      sett.expandEnumerations = expandEnumerations

      Dim ret As String = _ToXml(item, sett)

      Return ret

    End Function

    <Extension()> _
  Public Function ToXml( _
    ByVal item As Object, _
     ByVal useLongTypeNames As Boolean, _
     ByVal expandValueTypes As Boolean, _
     ByVal includeProperties As Boolean, _
     ByVal includeFields As Boolean, _
     ByVal expandEnumerations As Boolean, _
     ByVal sortByName As Boolean, _
     ByVal encapsulatingTag As String, _
     ByVal enumerationItemTag As String, _
     ByVal nullTagItem As String, _
     ByVal maxDepth As Integer, _
     ByVal inlineValues As Boolean) As String

      Dim sett As New Settings()
      sett.useLongTypeNames = useLongTypeNames
      sett.expandValueTypes = expandValueTypes
      sett.encapsulatingTag = encapsulatingTag
      sett.enumerationItemTag = enumerationItemTag
      sett.maxDepth = maxDepth
      sett.sortByName = sortByName
      sett.includeFields = includeFields
      sett.includeProperties = includeProperties
      sett.expandEnumerations = expandEnumerations
      sett.nullTagItem = nullTagItem
      sett.inlineValues = inlineValues

      Dim ret As String = _ToXml(item, sett)

      Return ret

    End Function

#End Region 'Public methods


#Region "Private methods"

    Private Function _ToXml(ByVal item As Object, ByVal sett As Settings) As String

      If item Is Nothing Then
        Throw New ArgumentNullException("Cannot generate XML content from null/nothing value.")
      End If

      Dim ret As String = CreateXmlByType(sett.encapsulatingTag, item, sett)

      ret = FormatXml(ret)

      Return ret

    End Function

    Private Function CreateXml(ByVal tag As String, ByVal value As ValueType, ByVal sett As Settings) As String
      Dim ret As String = _
        CreateTag(tag, value.GetType(), True, value.ToString(), sett)

      Return ret
    End Function

    Private Function CreateXml(ByVal tag As String, ByVal value As String, ByVal sett As Settings) As String
      Dim ret As String = _
        CreateTag(tag, value.GetType(), True, value, sett)

      Return ret
    End Function

    Private Function CreateXml(ByVal tag As String, ByVal value As [Enum], ByVal sett As Settings) As String
      Dim ret As String = _
        CreateTag(tag, value.GetType(), True, value.ToString(), sett)

      Return ret
    End Function

    Private Function CreateXml(ByVal tag As String, ByVal value As IEnumerable, ByVal sett As Settings) As String
      Dim content As New StringBuilder
      Dim pom As String

      If sett.expandEnumerations Then

        For Each fItem As Object In value
          pom = CreateXmlByType(sett.enumerationItemTag, fItem, sett)
          content.AppendLine(pom)
        Next
      Else

        If (TypeOf value Is ICollection) Then
          content.AppendLine("Count: " + CType(value, ICollection).Count.ToString())
        Else
          content.AppendLine()
        End If
      End If

      Dim ret As String = _
        CreateTag(tag, value.GetType(), False, content.ToString(), sett)

      Return ret

    End Function

    Private Function CreateXml(ByVal tag As String, ByVal value As Object, ByVal sett As Settings) As String

      Dim content As New StringBuilder
      Dim pom As String
      Dim itemValue As Object
      Dim valueType As Type = value.GetType()
      Dim props() As PropertyInfo = valueType.GetProperties()
      Dim fields() As FieldInfo = valueType.GetFields()
      Dim members As New List(Of MemberInfo)

      If sett.includeFields Then _
        members.AddRange(valueType.GetFields())
      If sett.includeProperties Then _
        members.AddRange(valueType.GetProperties())

      If sett.sortByName Then
        members.Sort(New MemberInfoByNameComparer())
      End If

      For Each fItem As MemberInfo In members

        If TypeOf fItem Is PropertyInfo Then

          Dim pi As PropertyInfo = CType(fItem, PropertyInfo)
          If Not pi.CanRead Then Continue For

          itemValue = valueType.InvokeMember( _
            pi.Name, BindingFlags.GetProperty, Nothing, value, Nothing, Nothing)
        Else
          itemValue = valueType.InvokeMember( _
          fItem.Name, BindingFlags.GetField, Nothing, value, Nothing, Nothing)
        End If

        If itemValue Is Nothing Then
          Dim t As Type
          If TypeOf fItem Is PropertyInfo Then
            t = CType(fItem, PropertyInfo).PropertyType
          Else
            t = CType(fItem, FieldInfo).FieldType
          End If

          pom = CreateTag(fItem.Name, t, True, sett.nullTagItem, sett)
          content.AppendLine(pom)

        Else
          pom = CreateXmlByType(fItem.Name, itemValue, sett)
          content.AppendLine(pom)
        End If

      Next

      Dim ret As String = _
        CreateTag(tag, value.GetType(), False, content.ToString(), sett)

      Return ret

    End Function

    Private Function CreateXmlByType(ByVal tag As String, ByVal value As Object, ByVal sett As Settings) As String

      If sett.maxDepth < sett.currentDepth Then _
        Return ""

      sett.currentDepth += 1

      Dim ret As String

      If TypeOf value Is ValueType Then
        Dim t As Type = value.GetType()
        If t.IsPrimitive Then
          ret = CreateXml(tag, CType(value, ValueType), sett)
        Else
          If sett.expandValueTypes Then
            ret = CreateXml(tag, CType(value, Object), sett)
          Else
            ret = CreateXml(tag, CType(value, ValueType), sett)
          End If
        End If
      ElseIf TypeOf value Is String Then
        ret = CreateXml(tag, CType(value, String), sett)
      ElseIf TypeOf value Is IEnumerable Then
        ret = CreateXml(tag, CType(value, IEnumerable), sett)
      Else
        ret = CreateXml(tag, CType(value, Object), sett)
      End If

      sett.currentDepth -= 1

      Return ret
    End Function

    Private Delegate Sub AddingDelegate(ByVal text As String)
    Private Function CreateTag( _
      ByVal tag As String, ByVal type As Type, ByVal isAtomic As Boolean, ByVal value As String, _
      ByVal sett As Settings) As String

      Dim sb As New StringBuilder
      Dim d As AddingDelegate

      If sett.inlineValues AndAlso isAtomic Then
        d = AddressOf sb.Append
      Else
        d = AddressOf sb.AppendLine
      End If

      If sett.useLongTypeNames Then
        d("<" & tag & " type=""" & type.FullName & """>")
      Else
        d("<" & tag & " type=""" & type.Name & """>")
      End If

      d(value)

      d("</" & tag & ">")

      Return sb.ToString()
    End Function

    Private Function FormatXml(ByVal text As String) As String
      Dim rdr As New System.IO.StringReader(text)
      Dim level As Integer = 0
      Dim ret As New StringBuilder()

      Dim line As String = rdr.ReadLine()

      While line IsNot Nothing
        If line.StartsWith("</") Then
          level = level - 1
          ret.AppendLine(INTENDER.Repeat(level) & line)
        ElseIf line.StartsWith("<") Then
          ret.AppendLine(INTENDER.Repeat(level) & line)
          If (Not line.Contains("</")) Then _
            level = level + 1
        ElseIf line.Trim().Length > 0 Then
          ret.AppendLine(INTENDER.Repeat(level) & line)
        End If

        line = rdr.ReadLine()
      End While

      Return ret.ToString()
    End Function

#End Region 'Private methods

#End Region 'XML



  End Module

End Namespace