//Imports ESystem.Collections
//Imports ESystem.Extensions

//Public Class TimeStats
//  Implements ICloneable

//  Private overCount As Integer = 0
//  Private firstItemTime? As DateTime = Nothing
//  Private lastItemTime? As DateTime = Nothing
//  Private queue As New List(Of DateTime)
//  Private isPacked As Boolean = False

//  ''' <summary>
//  ''' Private field for property Interval
//  ''' </summary>
//  ''' <default>new TimeSpan(1,0,0);</default>
//  <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Advanced)> _
//  Private _Interval As TimeSpan = New TimeSpan(1, 0, 0)
//  ''' <summary>
//  ''' Defines Interval
//  ''' </summary>
//  ''' <remarks>Default value is new TimeSpan(1,0,0);</remarks>
//  ''' <default>new TimeSpan(1,0,0);</default>
//  Public Property Interval() As TimeSpan
//    Get
//      Return _Interval
//    End Get
//    Private Set(ByVal value As TimeSpan)
//      _Interval = value
//    End Set
//  End Property

//  ''' <summary>
//  ''' Returns item count in last interval
//  ''' </summary>
//  ''' <value></value>
//  ''' <returns></returns>
//  ''' <remarks></remarks>
//  Public ReadOnly Property CountInLastInterval() As Integer
//    Get
//      CheckForPack()

//      Return queue.Count
//    End Get
//  End Property

//  ''' <summary>
//  ''' Return items/per interval value calculated by last interval
//  ''' </summary>
//  ''' <value></value>
//  ''' <returns></returns>
//  ''' <remarks></remarks>
//  Public ReadOnly Property CountPerLastInterval() As Double
//    Get
//      CheckForPack()

//      Dim ret As Double

//      If queue.Count = 0 Then
//        ret = 0
//      Else
//        ret = GetMovesPerInterval( _
//          queue.First(), queue.Last(), queue.Count)
//      End If

//      Return ret
//    End Get
//  End Property

//  ''' <summary>
//  ''' Return total count
//  ''' </summary>
//  ''' <value></value>
//  ''' <returns></returns>
//  ''' <remarks></remarks>
//  Public ReadOnly Property Count() As Integer
//    Get
//      Return CountInLastInterval + overCount
//    End Get
//  End Property

//  ''' <summary>
//  ''' Return count per interval calculated from whole data
//  ''' </summary>
//  ''' <value></value>
//  ''' <returns></returns>
//  ''' <remarks></remarks>
//  Public ReadOnly Property CountPerInterval() As Double
//    Get
//      CheckForPack()

//      Dim ret As Double = 0

//      If Not firstItemTime.HasValue Then
//        ret = 0

//      Else

//        ret = GetMovesPerInterval( _
//          firstItemTime.Value, queue.Last, Count)

//      End If

      

//      Return ret
//    End Get
//  End Property

//  ''' <summary>
//  ''' Updates values for last interval
//  ''' </summary>
//  ''' <remarks></remarks>
//  Public Sub Pack()
//    If queue.Count > 0 Then _
//      Pack(queue.Last())
//  End Sub
//  ''' <summary>
//  ''' Updates values for last interval by parameter - current time
//  ''' </summary>
//  ''' <param name="currentTime"></param>
//  ''' <remarks></remarks>
//  Public Sub Pack(ByVal currentTime As DateTime)


//    If (queue.Count > 0) Then

//      Dim deadLine As DateTime = currentTime.Add(Interval.Negate())

//      While queue.First() < deadLine
//        queue.RemoveAt(0)
//        overCount += 1
//      End While

//    End If


//    isPacked = True

//  End Sub

//  Private Sub CheckForPack()
//    If isPacked Then Exit Sub

//    If queue.Count > 0 Then _
//     Pack(queue.Last())
//  End Sub

//  Private Function GetMovesPerInterval(ByVal eFrom As DateTime, ByVal eTo As DateTime, ByVal count As Integer) As Double
//    Dim diff As TimeSpan = eTo - eFrom
//    Dim delta As Double = diff.DivideBy(Interval)
//    Dim ret As Double = count / delta

//    Return ret
//  End Function

//  ''' <summary>
//  ''' Creates new instance for interval = 1 hour
//  ''' </summary>
//  ''' <remarks></remarks>
//  Public Sub New()
//    Me.Interval = New TimeSpan(1, 0, 0)
//  End Sub
//  ''' <summary>
//  ''' Creates new instance for specified interval
//  ''' </summary>
//  ''' <param name="interval"></param>
//  ''' <remarks></remarks>
//  Public Sub New(ByVal interval As TimeSpan)
//    Me.Interval = interval
//  End Sub

//  Public Sub Add(ByVal time As DateTime)
//    If queue.Count > 0 AndAlso queue.Last() > time Then
//      Throw New ArgumentException("Time ticket is lower then last added. Unable to add unsorted item.")
//    Else
//      If queue.Count = 0 Then firstItemTime = time
//      lastItemTime = time
//      queue.Add(time)
//      isPacked = False
//    End If
//  End Sub

//  Public Sub Insert(ByVal time As DateTime)
//    queue.Add(time)
//    queue.Sort()
//    lastItemTime = queue(queue.LastIndex)
//    isPacked = False
//  End Sub

//  ''' <summary>
//  ''' Clones value into new one
//  ''' </summary>
//  ''' <returns></returns>
//  ''' <remarks></remarks>
//  Public Function Clone() As Object Implements ICloneable.Clone
//    Dim ret As New TimeStats(Me.Interval)

//    ret.overCount = Me.overCount
//    Dim a As New EList(Of DateTime)

//    For Each fItem As DateTime In queue
//      a.Add(fItem)
//    Next

//    ret.queue = a

//    Return ret

//  End Function

//End Class
