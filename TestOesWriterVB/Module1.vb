Imports OesWriter

Module Module1

    Sub Main()
        Dim version As String = OesWriter.Version.GetVersion()

        'Production transaction
        Dim productionTransaction As New ProductionTransaction()
        productionTransaction.CellId = "K04902"
        productionTransaction.ItemId = "QG912123123"
        productionTransaction.GeneratedBarcode = ""
        productionTransaction.RequestCode = 0
        productionTransaction.FailureCode = 0
        productionTransaction.Status = 0
        productionTransaction.ProcessHistoryValues(0) = "1.1"
        productionTransaction.ProcessHistoryValues(1) = "2.2"
        productionTransaction.ProcessHistoryValues(2) = "3.3"
        productionTransaction.ProcessHistoryValues(3) = "4.4"
        productionTransaction.ProcessHistoryValues(4) = "5.5"
        productionTransaction.ProcessHistoryValues(5) = "6.6"
        productionTransaction.ProcessHistoryValues(6) = "7.7"
        productionTransaction.ProcessHistoryValues(7) = "8.8"
        productionTransaction.ProcessHistoryValues(8) = "9.9"
        'productionTransaction.ProcessHistoryValues(9) = "10.10"
        'productionTransaction.ProcessHistoryValues(10) = "11.11"
        'productionTransaction.ProcessHistoryValues(11) = "12.12"
        'productionTransaction.ProcessHistoryValues(12) = "13.13"
        'productionTransaction.ProcessHistoryValues(13) = "14.14"

        Dim productionResponse = productionTransaction.SendCsvTransaction("10.50.5.34")

        Console.WriteLine(productionResponse)

        Console.ReadLine()

        'Setup transaction
        Dim setupTransaction As New SetupTransaction()
        setupTransaction.CellId = "130401"
        setupTransaction.ModelNumber = "PROTOTYPE"
        setupTransaction.OpNumber = "10"
        setupTransaction.RequestCode = 4

        Dim setupResponse As SetupResponse = setupTransaction.SendCsvTransaction("10.50.5.34")

        Console.WriteLine(setupResponse.ResponseString)

        Console.ReadLine()

        'Login Transaction

        Dim loginTransaction As New LoginTransaction()
        loginTransaction.CellId = "130401"
        loginTransaction.RequestCode = 2
        loginTransaction.OperatorId = "50034"

        Dim loginResponse = loginTransaction.SendCsvTransaction("10.50.5.34")

        Console.WriteLine(loginResponse)

        Console.ReadLine()

        'Serial Request Transaction
        Dim serialTransaction As New SerialRequest()
        serialTransaction.CellId = 130411
        serialTransaction.RequestCode = 21

        Dim serialResponse = serialTransaction.SendTransaction("10.50.5.34")

        Console.WriteLine(serialResponse)

        Console.ReadLine()


    End Sub

End Module
