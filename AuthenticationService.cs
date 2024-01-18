public class AuthenticationService
{
    private Dictionary<string, string> users = new Dictionary<string, string>
    {
        { "gizem@g.com", "123456" }
    };

    public bool ValidateCredentials(string username, string password)
    {
        
        if (users.TryGetValue(username, out var expectedPassword))
        {
            return expectedPassword == password;
        }

        return false;
    }
}