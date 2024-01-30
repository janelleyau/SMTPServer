using SmtpServer;
using SmtpServer.Authentication;


namespace SMTPServerTesting
{
    public sealed class UserAuthenticatorChecker : IUserAuthenticator
    {
        static Dictionary<string, string>? AccountList { get; set; }

        public Task<bool> AuthenticateAsync(
            ISessionContext context,
            string user,
            string password,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(CheckUserAndPassword(user, password));
        }

        public IUserAuthenticator CreateInstance(ISessionContext context)
        {
            return new UserAuthenticatorChecker();
        }

        public static void SetAccountList(string[] details)
        {
            AccountList = details.Select(line => line.Split(' ')).ToDictionary(split => split[0], split => split[1]);
        }

        static bool CheckUserAndPassword(string user, string password)
        {
            foreach (KeyValuePair<string, string> account in AccountList!)
            {
                if (account.Key.Equals(user) && account.Value.Equals(password))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
