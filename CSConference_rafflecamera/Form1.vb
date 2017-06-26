Imports System.IO
Imports System.Net
Imports Emgu
Imports Newtonsoft.Json

Public Class Form1

    Dim hasCaptured As Boolean = False
    Dim camera As Emgu.CV.Capture
    Dim imageHolder, capturedHolder As Bitmap
    Dim apifindclient As New WebClient
    Dim apidetailsfinder As New WebClient
    Dim apiresponse As String

    Dim jss As New Newtonsoft.Json.JsonSerializer

    Dim apithread As Threading.Thread

    Dim attendee_id As String

    Dim apideleteclient As New WebClient
    Dim apideletestring As String

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Start()
        camera = New CV.Capture(1)
    End Sub


    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If hasCaptured Then
            apideletestring = apideleteclient.DownloadString("http://localhost/feutechcs/public/deletependingtask/" + attendee_id)
            capturedHolder.Save("raffle_images/" + attendee_id + ".jpg")
            MsgBox("saved")
            hasCaptured = False
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        hasCaptured = False
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        If hasCaptured = False Then
            imageHolder = New Bitmap(camera.QueryFrame.Bitmap)
            If imageHolder IsNot Nothing Then
                PictureBox1.Image = imageHolder
            End If
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        TextBox1.Text = ""
        apiresponse = apifindclient.DownloadString("http://localhost/feutechcs/public/getpendingtask")
        If apiresponse Is Nothing Or apiresponse Is String.Empty Then
            MsgBox("no task")
        Else
            'apiresponse = attendee_id
            Dim findDictionary As Dictionary(Of String, String) = jss.Deserialize(
                Of Dictionary(Of String, String))(
                New JsonTextReader(New StringReader(apiresponse)))

            attendee_id = findDictionary("attendee_id")
            Dim detailsurl As String = "http://localhost/feutechcs/public/getdetails1/" + attendee_id

            Dim attendee_details As String = apidetailsfinder.DownloadString(detailsurl)

            Dim detailsDictionary As Dictionary(Of String, String) = jss.Deserialize(
                Of Dictionary(Of String, String))(
                New JsonTextReader(New StringReader(attendee_details)))

            Dim fullname As String = detailsDictionary("last_name") + ", " +
                                    detailsDictionary("first_name") + " " +
                                    detailsDictionary("middle_name").ElementAt(0) + "."
            TextBox1.AppendText("Name: " + fullname + Environment.NewLine)
            TextBox1.AppendText("Course: " + detailsDictionary("course") + Environment.NewLine)
            TextBox1.AppendText("School: " + detailsDictionary("school") + Environment.NewLine)
            TextBox1.AppendText("Email: " + detailsDictionary("email") + Environment.NewLine)
            TextBox1.AppendText("Contact: " + detailsDictionary("contact") + Environment.NewLine)
            TextBox1.AppendText("Date Registered: " + detailsDictionary("created_at") + Environment.NewLine)

        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        capturedHolder = imageHolder
        PictureBox1.Image = capturedHolder
        hasCaptured = True
    End Sub
End Class
