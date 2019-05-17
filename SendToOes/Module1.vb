Imports System.Net.Sockets
Imports System.Text


Module Module1

    Sub Main()
        Dim client As New TcpClient
        client.Connect("10.50.5.34", 55001)

        Dim stream As NetworkStream = client.GetStream()

        Dim ProdData As New ProductionData
        ProdData.Command = "PROD"
        ProdData.CellId = "130401"
        ProdData.ItemId = "QG9VGJSON01"
        ProdData.GeneratedBarcode = ""
        ProdData.RequestCode = 0
        ProdData.Status = 0
        ProdData.FailureCode = 0
        ReDim ProdData.ProcessHistoryValues(2)
        ProdData.ProcessHistoryValues(0) = "1"
        ProdData.ProcessHistoryValues(1) = "2"
        ProdData.ProcessHistoryValues(2) = "3"

        Dim data As String = Newtonsoft.Json.JsonConvert.SerializeObject(ProdData)

        'csv format command,cellID,itemID,genBarcode,processtype,status,faultCode,processhistoryValue1,processhistoryValue2,
        'Dim data As String
        'data = "PROD,130401,QG9_VB_CSV1,0,0,0,0.0,0"
        Dim outData As [Byte]() = Encoding.ASCII.GetBytes(data)

        stream.Write(outData, 0, outData.Length)

    End Sub

    Class ProductionData
        Public Command As String
        Public CellId As String
        Public ItemId As String
        Public GeneratedBarcode As String
        Public RequestCode As Int32
        Public Status As Int32
        Public FailureCode As Int32
        Public ProcessHistoryValues() As String



    End Class

End Module
