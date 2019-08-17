Public Class Form1

    Private useGuide As Boolean = False
    Private hoursPrint As Boolean = True
    Private minutesPrint As Boolean = True
    Private secondsPrint As Boolean = True
    Private useButton As Boolean = False

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'MessageBox.Show("エルモだよ！")

        Me.useGuide = Not Me.useGuide

        Me.Invalidate()

    End Sub


    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint

        Dim g As Graphics = e.Graphics
        'g.DrawLine(Pens.Black, 20, 60, 250, 140)
        'g.DrawLine(Pens.Blue, 20, 200, 300, 20)
        'g.DrawLine(Pens.LightYellow, 300, 400, 50, 10)
        'Using p As New Pen(Color.Black, 3)
        '    g.DrawLine(p, 0, 150, 400, 150)
        'End Using

        Dim centerX As Integer = e.ClipRectangle.Width \ 2
        Dim centerY As Integer = e.ClipRectangle.Height \ 2

        If Me.useGuide Then

            g.DrawLine(Pens.Red, centerX, 0, centerX, e.ClipRectangle.Height)
            g.DrawLine(Pens.Red, 0, centerY, e.ClipRectangle.Width, centerY)

        End If

        Dim minSize As Integer = Math.Min(e.ClipRectangle.Width, e.ClipRectangle.Height)
        Dim center As New Point(centerX, centerY)
        Dim radius As Integer = minSize * 0.3

        Me.DrawBezel(g, center, radius)

        Me.DrawFaces(g, center, radius)

        Me.DrawHands(g, center, radius)


    End Sub

    Private Sub DrawFaces(g As Graphics, center As Point, radius As Integer)

        'Dim face As Integer = 0
        'Dim zero As PointF = Me.PointOnCircle(radius, 0, center)

        'g.DrawString(face.ToString(), Me.Font, Brushes.Black, zero)

        Dim faces() As String = {"Ⅰ", "Ⅱ", "Ⅲ", "Ⅳ", "Ⅴ", "Ⅵ", "Ⅶ", "Ⅷ", "Ⅸ", "Ⅹ", "XI", "XII"}

        For num As Integer = 1 To 12
            Dim face As String = faces(num - 1)
            Dim numPoint As PointF = Me.PointOnCircle(radius * 0.95, num * (360 / 12) - 6, center)
            Dim numSize As SizeF = g.MeasureString(face, Me.Font)
            Dim color As Brush
            Dim n As Integer = num Mod 3
            If n.Equals(0) Then
                color = Brushes.Red
            Else

                color = Brushes.Black
            End If
            Me.DrawLeanString(g, face, (360 / 12) * num, numPoint, Me.Font, color)

            'numPoint.X -= (numSize.Width / 2)

            'numPoint.Y -= (numSize.Height / 2)

            'g.DrawString(face, Me.Font, Brushes.Black, numPoint)

        Next

    End Sub

    Private Sub DrawLeanString(g As Graphics, s As String, degrees As Single, p As PointF, f As Font, b As Brush)

        Dim img0 As New Bitmap(1, 1)
        Dim bg0 As Graphics = Graphics.FromImage(img0)
        Dim w As Integer = bg0.MeasureString(s, f).Width
        Dim h As Integer = f.GetHeight(bg0)
        Dim img As New Bitmap(w, h)
        Dim bg As Graphics = Graphics.FromImage(img)

        bg.DrawString(s, f, b, 0, 0)

        Dim d As Double = degrees / (180 / Math.PI)
        Dim x1 As Single = p.X + img.Width * Math.Cos(d)
        Dim y1 As Single = p.Y + img.Width * Math.Sin(d)
        Dim x2 As Single = p.X - img.Height * Math.Sin(d)
        Dim y2 As Single = p.Y + img.Height * Math.Cos(d)
        Dim imgPoints As PointF() = {New PointF(p.X, p.Y), New PointF(x1, y1), New PointF(x2, y2)}

        g.DrawImage(img, imgPoints)

    End Sub

    Private Sub DrawHands(g As Graphics, center As Point, radius As Integer)

        Me.FillBezel(g, center, Brushes.Black)

        Dim timeNow As DateTime = DateTime.Now
        Dim time As TimeSpan = timeNow.TimeOfDay

        If Me.secondsPrint Then

            Dim seconds As PointF = Me.PointOnCircle(radius * 0.85, time.TotalSeconds * (360 / 60), center)

            Me.FillBezel(g, Me.ToPoint(seconds), Brushes.Blue)

            'g.DrawLine(Pens.Blue, center, seconds)

            If Me.useButton Then

                g.DrawString(time.Seconds.ToString(), Me.Font, Brushes.Black, New Point(200, 16))
            End If

        End If

        If Me.minutesPrint Then

            Dim minutes As PointF = Me.PointOnCircle(radius * 0.63, time.TotalMinutes * (360 / 60), center)

            Me.FillBezel(g, Me.ToPoint(minutes), Brushes.Green)

            'g.DrawLine(Pens.Green, center, minutes)

            If Me.useButton Then

                g.DrawString(time.Minutes.ToString(), Me.Font, Brushes.Black, New Point(200, 16))
            End If

        End If

        If Me.hoursPrint Then

            Dim hours As PointF = Me.PointOnCircle(radius * 0.5, time.TotalHours * (360 / 12), center)

            Me.FillBezel(g, Me.ToPoint(hours), Brushes.Red)

            'g.DrawLine(Pens.Red, center, hours)

            If Me.useButton Then

                g.DrawString(time.Hours.ToString(), Me.Font, Brushes.Black, New Point(200, 16))
            End If

        End If
    End Sub

    Private Sub FillBezel(g As Graphics, p As Point, color As Brush)

        Dim circle As New Rectangle(p, Size.Empty)

        circle.Inflate(5, 5)
        g.FillEllipse(color, circle)

    End Sub

    Private Sub DrawBezel(g As Graphics, center As Point, radius As Integer)

        Dim circle As New Rectangle(center, Size.Empty)

        circle.Inflate(radius, radius)
        g.DrawEllipse(Pens.Black, circle)

    End Sub

    ''' <summary>
    ''' 原点を指定して円周上の座標を求めます。
    ''' </summary>
    ''' <param name="radius">半径</param>
    ''' <param name="degrees">角度(度数法)</param>
    ''' <param name="center">原点(中心の座標)</param>
    ''' <returns>円周上の座標</returns>
    Private Function PointOnCircle(radius As Single, degrees As Single, center As Point) As PointF

        Dim radians As Double = (degrees * Math.PI) / 180 - (Math.PI / 2)
        Dim x As Single = radius * Math.Cos(radians)
        Dim y As Single = radius * Math.Sin(radians)

        Return New PointF(x + center.X, y + center.Y)

    End Function

    Private Function ToPoint(p As PointF)

        Return New Point(p.X, p.Y)

    End Function

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        Me.Invalidate()

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        If Me.hoursPrint Then

            If Me.useButton Then
                'ボタン2回目クリック

                Me.hoursPrint = True
                Me.minutesPrint = True
                Me.secondsPrint = True
                Me.useButton = False

            Else
                'ボタン1回目クリック

                Me.minutesPrint = False
                Me.secondsPrint = False
                Me.useButton = True

            End If


        ElseIf Me.useButton Then
            'ほかのボタンをクリック後

            Me.hoursPrint = True
            Me.minutesPrint = False
            Me.secondsPrint = False

        End If



    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        If Me.minutesPrint Then

            If Me.useButton Then
                'ボタン2回目クリック

                Me.hoursPrint = True
                Me.minutesPrint = True
                Me.secondsPrint = True
                Me.useButton = False

            Else
                'ボタン1回目クリック

                Me.hoursPrint = False
                Me.secondsPrint = False
                Me.useButton = True

            End If

        ElseIf Me.useButton Then
            'ほかのボタンをクリック後

            Me.hoursPrint = False
            Me.minutesPrint = True
            Me.secondsPrint = False


        End If

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

        If Me.secondsPrint Then

            If Me.useButton Then
                'ボタン2回目クリック

                Me.hoursPrint = True
                Me.minutesPrint = True
                Me.secondsPrint = True
                Me.useButton = False

            Else
                'ボタン1回目クリック

                Me.hoursPrint = False
                Me.minutesPrint = False
                Me.useButton = True

            End If

        ElseIf Me.useButton Then
            'ほかのボタンをクリック後

            Me.hoursPrint = False
            Me.minutesPrint = False
            Me.secondsPrint = True


        End If

    End Sub

End Class
