using SmtpServer;
using SmtpServer.ComponentModel;
using SmtpServer.Tracing;

namespace SMTPServerTesting
{
    class Program
    {
        static int SendPort { get; set; }
        static int ReceivePort { get; set; }
        public static string? AccountFile { get; set; }

        static async Task Main(string[] args)
        {
            //SetProperties(args);   
            //ReadAndMapAccountFile();
            var something = @"C:\Users\janelle.yau\OneDrive - WiseTech Global Pty Ltd\Documents\accountInfo.txt";
            ReadAndMapAccountFile(something);

            var cancellationTokenSource = new CancellationTokenSource();

            var options = new SmtpServerOptionsBuilder()
            .ServerName("localhost")
            .Endpoint(builder =>
                    builder
                        .Port(30, true)
                        .AuthenticationRequired(true)
                        .AllowUnsecureAuthentication(true)
                       )
                    .Build();

            var serviceProvider = new ServiceProvider();
            serviceProvider.Add(new MessageStorage(Console.Out));
            serviceProvider.Add(new UserAuthenticatorChecker());

            var smtpServer = new SmtpServer.SmtpServer(options, serviceProvider);
            smtpServer.SessionCreated += OnSessionCreated;
            smtpServer.SessionFaulted += OnSessionFaulted;
            smtpServer.SessionCancelled += OnSessionCancelled;
            smtpServer.SessionCompleted += OnSessionCompleted;

            //await smtpServer.StartAsync(CancellationToken.None);

            var serverTask = smtpServer.StartAsync(cancellationTokenSource.Token);

            Console.WriteLine("Server Started. Press any key to cancel the server.");
            Console.ReadKey();

            Console.WriteLine("\nForcibily cancelling the server and any active sessions");

            cancellationTokenSource.Cancel();
            serverTask.WaitWithoutException();
        }

        static void OnSessionCreated(object sender, SessionEventArgs e)
        {
            Console.WriteLine("Session Created.");

            e.Context.CommandExecuting += OnCommandExecuting;
        }

        static void OnCommandExecuting(object sender, SmtpCommandEventArgs e)
        {
            Console.WriteLine("Command Executing.");

            new TracingSmtpCommandVisitor(Console.Out).Visit(e.Command);
        }

        static void OnSessionFaulted(object sender, SessionFaultedEventArgs e)
        {
            Console.WriteLine("Session Faulted: {0}", e.Exception);
        }

        static void OnSessionCancelled(object sender, SessionEventArgs e)
        {
            Console.WriteLine("Session Cancelled");
        }

        static void OnSessionCompleted(object sender, SessionEventArgs e)
        {
            Console.WriteLine("Session Completed");
        }

        static void ReadAndMapAccountFile(string dfd)
        {
            var lines = File.ReadAllLines(dfd);
            UserAuthenticatorChecker.SetAccountList(lines);
        }

        static void SetProperties(string[] args)
        {
            SendPort = Convert.ToInt16(args[0]);
            ReceivePort = Convert.ToInt16(args[1]);
            AccountFile = args[2];
        }
    }
}