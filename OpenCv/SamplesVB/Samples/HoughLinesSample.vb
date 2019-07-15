﻿Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Text
Imports OpenCvSharp

' Namespace OpenCvSharpSamplesVB
Imports SampleBase

''' <summary>
''' ハフ変換による直線検出
''' </summary>
''' <remarks>http://opencv.jp/sample/special_transforms.html#hough_line</remarks>
Friend Module HoughLinesSample
    Public Sub Start()

        ' (1)画像の読み込み 
        Using imgGray As New Mat(FilePath.Image.Goryokaku, ImreadModes.GrayScale), _
                 imgStd As New Mat(FilePath.Image.Goryokaku, ImreadModes.Color), _
             imgProb As Mat = imgStd.Clone()
            ' Preprocess
            Cv2.Canny(imgGray, imgGray, 50, 200, 3, False)

            ' (3)標準的ハフ変換による線の検出と検出した線の描画
            Dim segStd() As LineSegmentPolar = Cv2.HoughLines(imgGray, 1, Math.PI / 180, 50, 0, 0)
            Dim limit As Integer = Math.Min(segStd.Length, 10)
            For i As Integer = 0 To limit - 1
                Dim rho As Single = segStd(i).Rho
                Dim theta As Single = segStd(i).Theta

                Dim a As Double = Math.Cos(theta)
                Dim b As Double = Math.Sin(theta)
                Dim x0 As Double = a * rho
                Dim y0 As Double = b * rho
                Dim pt1 As Point = New Point With {.X = Math.Round(x0 + 1000 * (-b)), .Y = Math.Round(y0 + 1000 * (a))}
                Dim pt2 As Point = New Point With {.X = Math.Round(x0 - 1000 * (-b)), .Y = Math.Round(y0 - 1000 * (a))}
                imgStd.Line(pt1, pt2, New Scalar(0, 0, 255), 3, LineTypes.AntiAlias, 0)
            Next i

            ' (4)確率的ハフ変換による線分の検出と検出した線分の描画
            Dim segProb() As LineSegmentPoint = Cv2.HoughLinesP(imgGray, 1, Math.PI / 180, 50, 50, 10)
            For Each s As LineSegmentPoint In segProb
                imgProb.Line(s.P1, s.P2, New Scalar(0, 0, 255), 3, LineTypes.AntiAlias, 0)
            Next s


            ' (5)検出結果表示用のウィンドウを確保し表示する
            Using TempCvWindow As Window = New Window("Hough_line_standard", WindowMode.AutoSize, imgStd), _
                 TempCvWindowProb As Window = New Window("Hough_line_probabilistic", WindowMode.AutoSize, imgProb)
                Window.WaitKey(0)
            End Using
        End Using
    End Sub

End Module
' End Namespace
