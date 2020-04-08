
//Namespace Patterns

//  ''' <summary>
//  ''' Represents class which can share information between more objects.
//  ''' </summary>
//  ''' <typeparam name="T">Information type to share.</typeparam>
//  Public Class ShareProvider(Of T)

//    ''' <summary>
//    ''' Delegate for info what is shared.
//    ''' </summary>
//    ''' <param name="newItem">New item added for share.</param>
//    Public Delegate Sub NewItemAddedDelegate(ByVal newItem As T)
//    ''' <summary>
//    ''' Occurs when new item is added to share.
//    ''' </summary>
//    Public Event NewItemAdded As NewItemAddedDelegate

//    ''' <summary>
//    ''' Private info holds what is shared since last read-out.
//    ''' </summary>
//    Private dict As New Dictionary(Of String, List(Of T))

//    ''' <summary>
//    ''' Registers new subscriber for which information will be shared.
//    ''' New unique ID is required.
//    ''' </summary>
//    ''' <param name="subscriberID"></param>
//    Public Sub RegisterSubscriber(ByVal subscriberID As String)
//      If IsUsed(subscriberID) Then
//        Throw New Exception("This subscriberID is already used.")
//      End If

//      dict.Add(subscriberID, New List(Of T))
//    End Sub

//    ''' <summary>
//    ''' Registers new subscriber for which information will be shared.
//    ''' New unique ID is returned as value.
//    ''' </summary>
//    ''' <returns>New unique ID.</returns>
//    Public Function RegisterSubscriber() As String
//      Dim ret As String = GetRandomID()

//      RegisterSubscriber(ret)

//      Return ret
//    End Function

//    ''' <summary>
//    ''' Unregisters subscriber by id. If ID not exists, will be ignored.
//    ''' </summary>
//    ''' <param name="subscriberID"></param>
//    Public Sub UnregisterSubscriber(ByVal subscriberID As String)
//      If dict.ContainsKey(subscriberID) Then
//        dict.Remove(subscriberID)
//      End If
//    End Sub

//    ''' <summary>
//    ''' Adds new shared info.
//    ''' </summary>
//    ''' <param name="item"></param>
//    Public Sub AddSharedInfo(ByVal item As T)

//      For Each fItem As String In dict.Keys
//        dict(fItem).Add(item)
//      Next

//      RaiseEvent NewItemAdded(item)

//    End Sub
//    ''' <summary>
//    ''' Adds new shared infos.
//    ''' </summary>
//    ''' <param name="items"></param>
//    Public Sub AddSharedInfo(ByVal items As IEnumerable(Of T))

//      For Each Item As T In items
//        AddSharedInfo(Item)
//      Next

//    End Sub

//    ''' <summary>
//    ''' Reads out all new info for subscriber.
//    ''' </summary>
//    ''' <param name="subscriberID"></param>
//    ''' <returns></returns>
//    Public Function ReadOutItems(ByVal subscriberID As String) As T()
//      If Not dict.ContainsKey(subscriberID) Then
//        Throw New Exception("This subscriberID is not registered.")
//      End If

//      Dim ret As T() = dict(subscriberID).ToArray()
//      dict(subscriberID).Clear()

//      Return ret
//    End Function

//    ''' <summary>
//    ''' Returns new random ID.
//    ''' </summary>
//    ''' <returns></returns>
//    Private Function GetRandomID() As String
//      Dim r As New Random()

//      Dim ret As String = r.Next(0, 100000).ToString()

//      While (IsUsed(ret))
//        ret = r.Next(0, 1000000).ToString()
//      End While

//      Return ret
//    End Function

//    ''' <summary>
//    ''' Returns true if subscriber id is already used.
//    ''' </summary>
//    ''' <param name="subscriberID"></param>
//    ''' <returns></returns>
//    Private Function IsUsed(ByVal subscriberID As String) As Boolean
//      If dict.Keys.Contains(subscriberID) Then
//        Return True
//      Else
//        Return False
//      End If
//    End Function

//  End Class

//End Namespace