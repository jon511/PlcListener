Imports OESListener

Module Module1


    Sub Main()

        Dim listener As New Listener()
        listener.Listen()

        AddHandler listener.ProductionReceived, AddressOf ProductionTransactionReceived
        AddHandler listener.SetupReceived, AddressOf SetupTransactionReceived
        AddHandler listener.LoginReceived, AddressOf LoginTransactionReceived

        Logger.LogPath = "C:\\Users\\jonosborne\\Dropbox\\Developer\\Visual Studio\\Source\\Repos\\PlcListener.git\\TestVB\\bin\\Debug\\Log"
        Logger.LogToFile = True
        Logger.LogToConsole = True

    End Sub

    Private Sub ProductionTransactionReceived(sender As Object, e As ProductionEventArgs)

        Logger.Log("ProductionResponse from VB")

        'call static method ProductionResponse and pass back the PorductionEventArgs as the parameter
        Listener.ProductionResponse(e)

    End Sub

    Private Sub SetupTransactionReceived(sender As Object, e As SetupEventArgs)

        Logger.Log("SetupResponse from VB")

        e.Response.Component1.AccessId = "1"
        e.Response.Component1.ModelNumber = "B0111100-00"
        e.Response.Component2.AccessId = "2"
        e.Response.Component2.ModelNumber = "B0222200-00"
        e.Response.PlcModelSetup(0) = "1.1"
        e.Response.PlcModelSetup(1) = "2.2"
        e.Response.PlcModelSetup(2) = "3.3"
        e.Response.Acknowledge = "1"
        e.Response.ErrorCode = "1"


        'call static method SetupResponse and pass back the SetupEventArgs as the parameter
        Listener.SetupResponse(e)

    End Sub

    Private Sub LoginTransactionReceived(sender As Object, e As LoginEventArgs)

        Console.WriteLine(e.Status)
        Console.WriteLine(e.FailureCode)
        e.FailureCode = 0
        e.Status = 1
        'call static method LoginResponse and pass back the LoginEventArgs as the parameter
        Listener.LoginResponse(e)

    End Sub

End Module
