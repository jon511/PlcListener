Imports OESListener

Module Module1


    Sub Main()

        'Logging can be use by calling Logger.Log("string to be loggd"), if Logger.LogToFile is set to True, it will be logged to the logfile.
        'if Logger.LogToConsole is set to True, it will log to screen.
        'if Logger.LogPath is not set, the default logPath will in the same location as the exe file. you can set a custom log file location by
        'setting Logger.LogPath to the desired location.

        'Logger.LogPath = "C:\Users\jonosborne\Dropbox\Developer\Visual Studio\Source\Repos\PlcListener.git\TestVB\bin\Debug\Log"
        'Logger.LogToFile = True
        'Logger.LogToConsole = True
        'Logger.EnableDllLogging = False

        'dim listener with ip only will use default port of 55001 for the tcp listener
        'that can be changed by passing in the desired port as the second argument
        Dim listener As New Listener("10.50.71.118")

        'if listener.PrintFromFile is set to true, the print code needs to be placed in C:\PrintCode\Final\ for Final labels and
        'C:\PrintCode\Iterim\ for interim labels.
        'if listener.PrintFromFile is set to false, the print code needs to be sent back in the LabelPrintTransactionReceived method
        listener.PrintFromFile = True
        listener.Listen()

        'Add listeners for all transaction types that you want to use. All transaction events will be raised in the respective methods.
        AddHandler listener.ProductionReceived, AddressOf ProductionTransactionReceived
        AddHandler listener.SetupReceived, AddressOf SetupTransactionReceived
        AddHandler listener.LoginReceived, AddressOf LoginTransactionReceived
        AddHandler listener.SerialRequestReceived, AddressOf SerialRequestTransactionReceived
        AddHandler listener.LabelPrintReceived, AddressOf LabelPrintTransactionReceived

        'call Console.Readline() to keep main thread alive if using a console app. If using a form app, then this is not needed.
        Console.ReadLine()

    End Sub

    Private Sub ProductionTransactionReceived(sender As Object, e As ProductionEventArgs)

        'Production transaction will come in here, use the ProductionEventArgs to extract the data you need.

        'do database work here
        'threading is not needed here, all async is handled by the dll

        'update e with values from database and pass back in listenerResponse.ProductionResponse

        Dim listenerResponse As New ListenerResponse()
        listenerResponse.ProductionResponse(e)


    End Sub

    Private Sub SetupTransactionReceived(sender As Object, e As SetupEventArgs)

        'Setup transaction will come in here, use the SetupEventArgs to extract the data you need.

        'do database work here

        'setup return data will be written to e.Response
        'example:
        'e.Response.Component1.AccessId = "1"
        'e.Response.Component1.ModelNumber = "B0111100-00"
        'e.Response.Component2.AccessId = "2"
        'e.Response.Component2.ModelNumber = "B0222200-00"
        'e.Response.PlcModelSetup(0) = "1.1"
        'e.Response.PlcModelSetup(1) = "2.2"
        'e.Response.PlcModelSetup(2) = "3.3"
        'e.Response.Acknowledge = "1"
        'e.Response.ErrorCode = "1"

        'update e with values from database and pass back in listenerResponse.SetupResponse
        Dim listenerResponse As New ListenerResponse()
        listenerResponse.SetupResponse(e)

    End Sub

    Private Sub LoginTransactionReceived(sender As Object, e As LoginEventArgs)

        'do database work here

        'e.FailureCode = 0
        'e.Status = 1

        'update e with values from database and pass back in listenerResponse.LoginResponse
        Dim listenerResponse As New ListenerResponse()
        listenerResponse.LoginResponse(e)

    End Sub

    Private Sub SerialRequestTransactionReceived(sender As Object, e As SerialRequestEventArgs)

        'Serial request transaction comes in here

        'do database work here

        'update e with values from database and pass back in listenerResponse.SerialRequestResponse
        Dim listenerResponse As New ListenerResponse()
        listenerResponse.SerialRequestResponse(e)

    End Sub

    Private Sub LabelPrintTransactionReceived(sender As Object, e As LabelPrintEventArgs)
        'if listener.PrintFromFile is set to true, this method will not be used.
        'if listener.PrintFromFile is set to false, the print code needs to be assigned to e.PrintCode and 
        'and pass e back in listenerResponse.
        Dim listenerResponse As New ListenerResponse()
        listenerResponse.FinalPrintResponse(e)

    End Sub

End Module
