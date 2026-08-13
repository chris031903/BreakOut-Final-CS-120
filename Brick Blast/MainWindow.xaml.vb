Imports System.Windows.Threading

Class MainWindow

    Dim GameLoop As New DispatcherTimer

    Private BRICK_W As Integer = 200
    Private BRICK_H As Integer = 20
    Private BRICK_COLS As Integer = 5
    Private BRICK_ROWS As Integer = 5
    Private BRICK_GAP As Integer = 5

    Dim PADDLE_BUFFER As Integer = 15
    Dim PADDLE As New Rectangle()
    Const PADDLE_DISTANCE_FROM_BOTTOM As Double = 50
    Dim CENTER_OF_PADDLE As Double
    Private PADDLE_SPEED As Double = 5
    Dim PADDLE_TRANSLATE As New TranslateTransform(0, 0)
    Private MOVE_LEFT As Boolean
    Private MOVE_RIGHT As Boolean


    Private BALL As New Ellipse()
    Private BALL_BUFFER As Double = 5
    Private BALL_TRANSLATE As New TranslateTransform(0, 0)
    Private BALL_SPEED_Y As Double = 5
    Private BALL_SPEED_X As Double = 5
    Private WALL_TOP As Double = 0
    Private WALL_LEFT As Double = 0
    Private WALL_RIGHT As Double = 0
    Private WALL_BOTTOM As Double

    Private LIVES As Integer = 3






    Sub New()

        GameLoop.Interval = TimeSpan.FromMilliseconds(16)
        AddHandler GameLoop.Tick, AddressOf UpdateLoop
        InitializeComponent()
        DrawPaddle()
        DrawBall()
        DrawBricks()
        SETWALLS()
        GameLoop.Start()


    End Sub



    Private Sub UpdateLoop(sender As Object, e As EventArgs)
        MovePaddle()
        MoveBall()
        Check_Collision()
    End Sub



    Private Sub Check_Collision()
        Dim pt As Point = New Point(BALL_TRANSLATE.X, BALL_TRANSLATE.Y)
        VisualTreeHelper.HitTest(MainCanvas, Nothing, (AddressOf MyHitTestResult), New PointHitTestParameters(pt))
        If BALL_TRANSLATE.Y < WALL_TOP And BALL_SPEED_Y < 0 Then
            BALL_SPEED_Y *= -1
        End If
        If BALL_TRANSLATE.X < WALL_LEFT Then
            BALL_SPEED_X *= -1
        End If
        If BALL_TRANSLATE.X + BALL.Width > WALL_RIGHT Then
            BALL_SPEED_X *= -1
        End If

        If BALL_TRANSLATE.Y > WALL_BOTTOM Then

            LIVES -= 1
            LivesText.Text = "Lives: " & LIVES

            If LIVES <= 0 Then
                GameLoop.Stop()
                MessageBox.Show("Game Over!")
            Else
                BALL_TRANSLATE.X = (MainCanvas.Width - BALL.Width) / 2
                BALL_TRANSLATE.Y = (MainCanvas.Height - BALL.Height) / 2
                BALL_SPEED_X = 5
                BALL_SPEED_Y = -5
            End If

        End If

    End Sub


    Private Function MyHitTestResult(ByVal result As HitTestResult) As HitTestResultBehavior
        If result.VisualHit.GetType() Is GetType(Rectangle) Then

            If (CType(result.VisualHit, Rectangle)).Tag = "brick" Then
                MainCanvas.Children.Remove(result.VisualHit)
                BALL_SPEED_Y *= -1

                Return HitTestResultBehavior.Stop
            End If

            If result.VisualHit Is PADDLE Then
                BALL_SPEED_Y = -Math.Abs(BALL_SPEED_Y)

                Dim centerOfPaddleX As Double = PADDLE_TRANSLATE.X + CENTER_OF_PADDLE
                Dim ballDistFromPaddleCenterX As Double = BALL_TRANSLATE.X - centerOfPaddleX

                BALL_SPEED_X = ballDistFromPaddleCenterX * 0.08

                Return HitTestResultBehavior.Stop
            End If

        End If

        Return HitTestResultBehavior.Continue
    End Function

    Private Sub DrawBricks()


        For row = 0 To BRICK_ROWS - 1

            For columns = 0 To BRICK_COLS - 1

                Dim BRICK As New Rectangle()

                BRICK.Height = BRICK_H
                BRICK.Width = BRICK_W
                BRICK.Fill = Brushes.Brown
                BRICK.StrokeThickness = 2

                BRICK.RenderTransform = New TranslateTransform(
                    (BRICK_W + BRICK_GAP) * columns,
                    (BRICK_H + BRICK_GAP) * row)
                BRICK.Tag = "brick"
                MainCanvas.Children.Add(BRICK)

            Next

        Next

    End Sub




    Private Sub DrawBall()
        With BALL
            .Width = 20
            .Height = 20
            .Fill = Brushes.Red
            .Stroke = Brushes.Black
            .StrokeThickness = 2
            .RenderTransform = BALL_TRANSLATE
        End With
        BALL_TRANSLATE.X = (MainCanvas.Width - BALL.Width) / 2
        BALL_TRANSLATE.Y = (MainCanvas.Height - BALL.Height) / 2
        MainCanvas.Children.Add(BALL)
    End Sub
    Private Sub DrawPaddle()
        With PADDLE
            .Width = 124
            .Height = 20
            .Fill = Brushes.Red
            .Stroke = Brushes.Black
            .StrokeThickness = 2
            .RenderTransform = PADDLE_TRANSLATE
            CENTER_OF_PADDLE = .Width / 2
        End With

        PADDLE_TRANSLATE.X = (MainCanvas.Width - PADDLE.Width - PADDLE_BUFFER)
        PADDLE_TRANSLATE.Y = MainCanvas.Height - PADDLE.Height - (PADDLE_DISTANCE_FROM_BOTTOM)
        MainCanvas.Children.Add(PADDLE)
    End Sub

    Private Sub MoveBall()
        BALL_TRANSLATE.X += BALL_SPEED_X
        BALL_TRANSLATE.Y += BALL_SPEED_Y
        BALL.RenderTransform = BALL_TRANSLATE
    End Sub
    Private Sub MovePaddle()
        If MOVE_LEFT Then
            PADDLE_TRANSLATE.X -= PADDLE_SPEED
        End If
        If MOVE_RIGHT Then
            PADDLE_TRANSLATE.X += PADDLE_SPEED
        End If
        PADDLE.RenderTransform = PADDLE_TRANSLATE
    End Sub
    Private Sub SETWALLS()
        WALL_RIGHT = MainCanvas.Width - (BALL.Width + BALL_BUFFER)
        WALL_TOP += BALL.Height
        WALL_BOTTOM = MainCanvas.Height - (BALL.Height + BALL_BUFFER)
    End Sub
    Private Sub BrickBlast_KeyDown(sender As Object, e As KeyEventArgs) Handles BrickBlast.KeyDown

        Select Case e.Key
            Case Key.A
                Console.WriteLine("<--A was pressed")
                MOVE_LEFT = True
            Case Key.W
                Console.WriteLine("W was pressed")
            Case Key.S
                Console.WriteLine("S was pressed")
            Case Key.D
                Console.WriteLine("D was pressed- - ->")
                MOVE_RIGHT = True
            Case Key.Escape
                Me.Close()
        End Select
    End Sub

    Private Sub BrickBlast_KeyUp(sender As Object, e As KeyEventArgs) Handles BrickBlast.KeyUp

        Select Case e.Key
            Case Key.A
                Console.WriteLine("<-- A was pressed")
                MOVE_LEFT = False

            Case Key.W
                Console.WriteLine("W was pressed")

            Case Key.S
                Console.WriteLine("S was pressed")

            Case Key.D
                Console.WriteLine("D was pressed -->")
                MOVE_RIGHT = False

            Case Key.Escape
                Me.Close()
        End Select
    End Sub
End Class