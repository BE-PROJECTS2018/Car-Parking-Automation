Imports System.Net

Public Class frmMain
    Dim dbCon As New ADODB.Connection
    Dim dbReserve As New ADODB.Recordset
    Dim dbACC As New ADODB.Recordset

    Private Delegate Sub myDelegate()
    Dim fPLen As Boolean
    Dim RLen, TCount, PLen, DSidx As Integer
    Dim tRBuff(100), RBuff(100), RData(100) As Char

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        GetComList()
        dbCon.Open("Driver={SQL Server}; Server=.\SQLEXPRESS;Database=LIP#153", )
        Timer1.Enabled = True
    End Sub

    Private Sub btnGPRS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGPRS.Click
        tmrLIVERx.Interval = Val(txtDelay.Text)
        tmrLIVERx.Enabled = True
    End Sub
    Private Sub GetComList()
        ' Get a list of serial port names.
        Dim ports As String() = SerialPort.GetPortNames()
        ' Display each port name to the console.
        Dim port As String
        For Each port In ports
            cmbCOM.Items.Add(port)
        Next port

        fPLen = False
        TCount = 0
        PLen = 0
        DSidx = 0

    End Sub
    Private Sub btnConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnect.Click
        On Error GoTo X
        Port.Close()
        Port.PortName = cmbCOM.Text
        Port.BaudRate = 9600
        Port.Parity = Parity.None
        Port.DataBits = 8
        Port.StopBits = StopBits.One
        Port.Handshake = Handshake.None
        Port.Open()
        MsgBox("Port is Reconfigured.")

        Exit Sub
X:
        MsgBox(Err.Description)
    End Sub
    Private Sub Port_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles Port.DataReceived
        Me.Invoke(New myDelegate(AddressOf DataArrived), New Object() {})
    End Sub
    Private Sub DataArrived()
        Dim i As Integer

        tRBuff = Port.ReadExisting.ToCharArray
        RLen = tRBuff.Length

        txtSerial.Text = txtSerial.Text & vbNewLine & tRBuff

        For i = 0 To RLen - 1
            RBuff(TCount + i) = tRBuff(i) 'add this segment to Buff
        Next i
        TCount = TCount + RLen 'increament total count
        If (TCount < 3) Then Exit Sub

        For i = 0 To TCount - 1
            If RBuff(i) = "L" And RBuff(i + 1) = "I" Then
                If (TCount >= i + 2) Then
                    PLen = Asc(RBuff(i + 2)) 'decoment this
                    DSidx = i + 3
                    fPLen = True
                    Exit For
                End If
            End If
        Next i

        If (fPLen = False) Then Exit Sub 'header not found yet

        If (TCount < DSidx + PLen) Then Exit Sub 'chk if all data recieved

        Dim j = DSidx
        For i = 0 To PLen - 1
            RData(i) = RBuff(j) 'store data
            j = j + 1
        Next i

        'process on data///////////////////////////////////////////////
        lblStatus.Text = RData
        'send to the remote Mobile
        Dim LIVE As New WebClient
        LIVE.UploadString("http://iot.logicinside.net", "LIG_2111718.txt" & "SERVER" & "S" & lblStatus.Text)

        'process on data\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        'chk for extra bytes in the buff
        If (TCount > DSidx + PLen) Then

            Dim tLen = TCount - (DSidx + PLen) 'calculate its lenght
            'shift the array to initial position
            For i = 0 To tLen - 1
                RBuff(i) = RBuff(DSidx + PLen + i)
            Next i
            TCount = tLen
        Else
            TCount = 0
        End If
        'Reset-Clear All Flags
        PLen = 0
        DSidx = 0
        fPLen = False

    End Sub
    Private Sub btnRegitration_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRegitration.Click
        frmRegistration.Show()
    End Sub
    Private Sub btnReservationRecord_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReservationRecord.Click
        frmReservation.Show()
    End Sub
    Private Sub tmrLIVERx_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrLIVERx.Tick
        On Error GoTo X

        Dim LIVE As New WebClient

        tmrLIVERx.Interval = Val(txtDelay.Text)

        Dim strLIVE As String
        strLIVE = LIVE.DownloadString("http://iot.logicinside.net/LIG_2111718.txt")
        txtLIVE.Text = strLIVE

        strLIVE = txtLIVE.Text

        If strLIVE.StartsWith("NOTHING") Then
            Exit Sub
        End If

        If strLIVE.StartsWith("MOBILE") Then
            Dim Parsed() As String
            Parsed = strLIVE.Substring(6).Split("*")

            If Parsed(0) = "STATUS" Then
                Port.Write("Q") 'ask the hardware for the current request
            End If
            If Parsed(0) = "RESERVE" Then

                Dim VID, SlotID As String
                Dim SrNo As Integer


                VID = Parsed(1)
                SlotID = Parsed(2)


                'deduction of the ammount
                dbACC.Open("SELECT * FROM Account WHERE VehicleID='" & VID & "'", dbCon, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
                If dbACC.BOF Then
                    txtMsg.Text = txtMsg.Text & vbNewLine & "Vehicle NOT Registered !!!"
                    dbACC.Close()
                    SendToMobile("Vehicle NOT Registered !!!")
                    Exit Sub
                End If

                'check for balance
                If dbACC.Fields("Balance").Value <= 50 Then
                    txtMsg.Text = txtMsg.Text & vbNewLine & "Low Balance !!!"
                    SendToMobile("Low Balance !!!-->" & dbACC.Fields("Balance").Value)
                    dbACC.Close()
                    Exit Sub
                End If

                'check the vaildation of the request
                dbReserve.Open("SELECT * FROM Reserve WHERE SlotID='" & SlotID & "' AND Status='R'", dbCon, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
                If Not dbReserve.BOF Then
                    txtMsg.Text = txtMsg.Text & vbNewLine & "Already Reserved..."
                    dbReserve.Close()
                    SendToMobile("SlotID Already Reserved!")
                    Exit Sub
                Else
                    txtMsg.Text = txtMsg.Text & vbNewLine & "Reservation Allowed..."
                    dbReserve.Close()
                End If

                'store the request in database
                dbReserve.Open("SELECT * FROM Reserve", dbCon, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
                If (dbReserve.BOF) Then
                    SrNo = 1
                Else
                    dbReserve.MoveLast()
                    SrNo = dbReserve.Fields("SrNo").Value + 1
                End If
                dbReserve.AddNew()
                dbReserve.Fields("SrNo").Value = SrNo
                dbReserve.Fields("RDateTime").Value = Date.Now
                dbReserve.Fields("VID").Value = VID
                dbReserve.Fields("Code").Value = Format(SrNo, "00000")
                dbReserve.Fields("SlotID").Value = SlotID
                dbReserve.Fields("Status").Value = "R"
                dbReserve.Update()
                dbReserve.Close()


                txtMsg.Text = txtMsg.Text & vbNewLine & "Reservation Done Succesfully."
                SendToMobile("Reservation Done. Code:" & Format(SrNo, "00000"))

            End If
        End If

        Exit Sub
X:
        txtMsg.Text = Err.Description
    End Sub
    Sub SendToMobile(ByVal Msg As String)
        Dim LIVE As New WebClient
        LIVE.UploadString("http://iot.logicinside.net", "LIG_2111718.txt" & "SERVER" & "M" & Msg)

        txtMsg.Text = txtMsg.Text & vbNewLine & "Msg Sent!"
    End Sub

    Private Sub btnEntry_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEntry.Click
        'check the vaildation of the request
        dbReserve.Open("SELECT * FROM Reserve WHERE Code='" & txtCode.Text & "' AND Status='R'", dbCon, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
        If Not dbReserve.BOF Then
            txtMsg.Text = txtMsg.Text & vbNewLine & "Succsess."
            dbReserve.Fields("ENDateTime").Value = DateTime.Now
            dbReserve.Fields("Status").Value = "P"
            dbReserve.Update()
            dbReserve.Close()
            Exit Sub
        Else
            txtMsg.Text = txtMsg.Text & vbNewLine & "Reservation Cancelled. OR Invalid Code."
            dbReserve.Close()
        End If
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Dim VID, balance As String


        dbReserve.Open("SELECT * FROM Reserve WHERE Code='" & txtCode.Text & "' AND Status='P'", dbCon, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
        If Not dbReserve.BOF Then
            txtMsg.Text = txtMsg.Text & vbNewLine & "Succsess."
            VID = dbReserve.Fields("VID").Value

            Dim TCount As Integer
            Dim RDT As DateTime
            RDT = dbReserve.Fields("ENDateTime").Value
            TCount = DateTime.Now.Subtract(RDT).TotalSeconds

            dbReserve.Fields("EXDateTime").Value = DateTime.Now
            dbReserve.Fields("Status").Value = "C"
            dbReserve.Fields("Duration").Value = TCount
            dbReserve.Fields("Charge").Value = TCount * 2
            dbReserve.Update()
            dbReserve.Close()

            'deduct balance
            dbACC.Fields("Balance").Value = dbACC.Fields("Balance").Value - TCount * 2
            balance = dbACC.Fields("Balance").Value
            dbACC.Update()
            dbACC.Close()

            txtMsg.Text = txtMsg.Text & vbNewLine & "Balance Deducted:" & TCount * 2

            Exit Sub
        Else
            txtMsg.Text = txtMsg.Text & vbNewLine & "NOT Parked. OR Invalid Code."
            dbReserve.Close()
        End If



    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick

        'check the vaildation of the request
        dbReserve.Open("SELECT * FROM Reserve WHERE Status='R'", dbCon, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic)
        If dbReserve.BOF Then
            dbReserve.Close()
            Exit Sub
        End If

        While Not dbReserve.EOF
            Dim TCount As Integer
            Dim RDT As DateTime
            RDT = dbReserve.Fields("RDateTime").Value
            TCount = DateTime.Now.Subtract(RDT).TotalSeconds

            If TCount > 60 Then
                dbReserve.Fields("Status").Value = "X"
                dbReserve.Update()
            End If

            dbReserve.MoveNext()
        End While
        dbReserve.Close()

    End Sub
End Class
