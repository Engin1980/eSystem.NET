Option Explicit On
Option Strict On

Imports System.Runtime.CompilerServices

Namespace Extensions

  Public Module ICollectionExt

    ''' <summary>
    ''' Returns index of last item in collection
    ''' </summary>
    <Extension()> _
    Public Function LastIndex(ByVal arr As ICollection) As Integer
      Return arr.Count - 1
    End Function

    ''' <summary>
    ''' Returns items joined together by separator.
    ''' </summary>
    ''' <param name="separator"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function ToString(ByVal arr As ICollection, ByVal separator As String) As String
      Dim ret As New Text.StringBuilder()

      For i As Integer = 0 To arr.Count - 1
        If i > 0 Then
          ret.Append(separator)
        End If
        ret.Append(arr(i))
      Next

      Return ret.ToString()
    End Function

    ''' <summary> 
    ''' Cuts list into two parts by index. 
    ''' </summary> 
    ''' <param name="index">Index where to split.</param> 
    ''' <param name="excludeSplittingItem">If True, indexed item will not appear in any created lists. 
    ''' If False, indexed item will be the first in the second list.</param> 
    ''' <returns></returns> 
    <Extension()> _
    Public Function SplitByIndex(Of T)(ByVal coll As ICollection(Of T), ByVal index As Integer, ByVal excludeSplittingItem As Boolean) As List(Of T)()
      Dim ret As List(Of T)() = New List(Of T)(1) {}

      ret(0) = coll.EGetRange(0, index)

      If excludeSplittingItem Then
        ret(1) = coll.EGetRange(index + 1)
      Else
        ret(1) = coll.EGetRange(index)
      End If

      Return ret
    End Function

    ''' <summary> 
    ''' Returns items in range from collection, from index to the end of source list. 
    ''' </summary> 
    ''' <param name="startIndex">Start index, inclusive.</param> 
    ''' <returns></returns> 
    <Extension()> _
    Public Function EGetRange(Of T)(ByVal coll As ICollection(Of T), ByVal startIndex As Integer) As List(Of T)
      Dim ret As List(Of T) = coll.EGetRange(startIndex, coll.Count - startIndex)

      Return ret
    End Function

    ''' <summary> 
    ''' Returns items in range from collection. 
    ''' </summary> 
    ''' <param name="startIndex">Start index, inclusive.</param> 
    ''' <param name="count">Count.</param> 
    ''' <returns></returns> 
    <Extension()> _
    Public Function EGetRange(Of T)(ByVal coll As ICollection(Of T), ByVal startIndex As Integer, ByVal count As Integer) As List(Of T)
      Dim ret As New List(Of T)

      For index As Integer = startIndex To startIndex + count - 1
        ret.Add(coll(index))
      Next

      Return ret
    End Function

    ''' <summary> 
    ''' Returns random item from collection. 
    ''' </summary> 
    ''' <returns></returns> 
    <Extension()> _
    Public Function GetRandom(Of T)(ByVal coll As ICollection(Of T), ByVal rnd As Random) As T
      If coll.Count = 0 Then
        Throw New Exception("Unable to get random item from empty collection.")
      Else
        Return coll(rnd.Next(0, coll.Count))
      End If
    End Function

    ''' <summary> 
    ''' Returns random item from collection. 
    ''' </summary> 
    ''' <returns></returns> 
    <Extension()> _
    Public Function GetRandom(Of T)(ByVal coll As ICollection(Of T)) As T
      Return GetRandom(coll, New Random())
    End Function
    ''' <summary> 
    ''' Returns random item from collection. 
    ''' </summary> 
    ''' <returns></returns> 
    <Extension()> _
    Public Function GetRandom(Of T)(ByVal coll As ICollection(Of T), ByVal seed As Integer) As T
      Return GetRandom(coll, New Random(seed))
    End Function

    <Extension()> _
    Public Function FindAll(Of T)(ByVal coll As IEnumerable(Of T), ByVal predicate As System.Func(Of T, Boolean)) As List(Of T)
      Dim ret As New List(Of T)

      For Each fItem As T In coll
        If predicate(fItem) Then ret.Add(fItem)
      Next

      Return ret

    End Function

    ''' <summary>
    ''' Removes item accepted by predicate from current collection and 
    ''' returns them in new collection.
    ''' </summary>
    ''' <param name="predicate">Predicate to match item to be removed.</param>
    ''' <returns>Removed items in new list.</returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function Cut(Of T)(ByVal coll As ICollection(Of T), ByVal predicate As System.Func(Of T, Boolean)) As List(Of T)
      Dim ret As New List(Of T)

      ret = FindAll(coll, predicate)
      RemoveRange(coll, ret)

      Return (ret)
    End Function

    ''' <summary>
    ''' Removes item accepted by predicate from current collection into the parameter.
    ''' </summary>
    ''' <param name="predicate">Predicate to match item to be removed.</param>
    ''' <returns>Removed items in new list.</returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function CutTo(Of T)(ByVal coll As ICollection(Of T), ByVal targetCollection As ICollection(Of T), ByVal predicate As System.Func(Of T, Boolean)) As List(Of T)
      Dim ret As New List(Of T)

      ret = FindAll(coll, predicate)
      RemoveRange(coll, ret)

      For Each fItem As T In ret
        targetCollection.Add(fItem)
      Next

      Return (ret)
    End Function

    <Extension()> _
    Public Sub RemoveRange(Of T)(ByVal coll As ICollection(Of T), ByVal items As IEnumerable(Of T))

      For Each fItem As T In items
        If coll.Contains(fItem) Then
          coll.Remove(fItem)
        End If
      Next

    End Sub

    <Extension()> _
Public Sub RemoveRange(Of T)(ByVal coll As ICollection(Of T), ByVal predicate As System.Func(Of T, Boolean))

      For index As Integer = coll.Count - 1 To 0 Step -1
        If (predicate(coll(index))) Then
          coll.Remove(coll(index))
        End If
      Next

    End Sub

    <Extension()> _
Public Sub LeaveRange(Of T)(ByVal coll As ICollection(Of T), ByVal predicate As System.Func(Of T, Boolean))

      For index As Integer = coll.Count - 1 To 0 Step -1
        If (Not predicate(coll(index))) Then
          coll.Remove(coll(index))
        End If
      Next

    End Sub


  End Module

End Namespace