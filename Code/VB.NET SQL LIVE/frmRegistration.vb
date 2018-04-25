Public Class frmRegistration
    Dim dbCon As New SqlConnection("Server=.\SQLEXPRESS;Database=LIP#153;Integrated Security=true")
    Dim dbDA As New SqlDataAdapter
    Dim dbDS As New DataSet
    Dim dbUPDATE As New SqlCommandBuilder

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        On Error GoTo X

        dbUPDATE.DataAdapter = dbDA
        dbDA.Update(dbDS, "Account")

        MsgBox("Record Updated.")

        Exit Sub
X:
        MsgBox(Err.Description)

    End Sub

    Private Sub frmOptions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        dbDA.SelectCommand = New SqlCommand("SELECT * FROM Account", dbCon)
        If dbDS.Tables.Contains("Account") Then
            dbDS.Tables.Remove("Account")
        End If
        dbDA.Fill(dbDS, "Account")
        dg.DataSource = dbDS.Tables("Account")
    End Sub

    Private Sub frmGridView_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        dg.Width = Me.Width - 40
        btnUpdate.Width = Me.Width - 40
    End Sub
End Class