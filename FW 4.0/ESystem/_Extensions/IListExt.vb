Option Explicit On
Option Strict On

Imports System.Runtime.CompilerServices

Namespace Extensions

  Public Module IListExt
    ''' <summary> 
    ''' Adds new item into collection, if item is not already inserted. 
    ''' Return true if item inserted, false if not. 
    ''' </summary> 
    ''' <param name="item"></param> 
    <Extension()> _
    Public Function AddDistinct(Of T)(ByVal coll As ICollection(Of T), ByVal item As T) As Boolean
      If Not coll.Contains(item) Then
        coll.Add(item)
        Return True
      Else
        Return False
      End If
    End Function

    ''' <summary> 
    ''' Adds new item into collection, if item is not already inserted. 
    ''' Return true if item inserted, false if not. 
    ''' </summary> 
    ''' <param name="items"></param> 
    <Extension()> _
    Public Function AddDistinct(Of T)(ByVal coll As IList(Of T), ByVal items As IEnumerable(Of T)) As Boolean

      items.EForEach( _
        Function(i) _
          AddDistinct(coll, i))

    End Function
  End Module



End Namespace