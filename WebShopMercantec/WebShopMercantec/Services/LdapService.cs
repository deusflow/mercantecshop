using Novell.Directory.Ldap;

namespace Service;

public class LdapService : ILdapService
{
    private readonly IConfiguration _configuration;

    public LdapService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<Dictionary<string, string>>> SearchAsync(string searchBase, string[] attributes, string searchFilter)
    {
        string loginInfo = Environment.GetEnvironmentVariable("AD_CONNECTION_TEST_LOGIN") ?? "";
        if (string.IsNullOrEmpty(loginInfo))
            throw new Exception("LDAP user login info is missing");

        string[] login = loginInfo.Split("__");
        if (login.Length != 2)
            throw new Exception("Invalid LDAP user login info format.");
        LdapConnection? connection = await ConnectAsync(login[0], login[1]);
        var list = new List<Dictionary<string, string>>();

        if (connection == null)
            return list;

        LdapSearchConstraints searchConstraints = connection.SearchConstraints;
        searchConstraints.ReferralFollowing = true;

        ILdapSearchResults search = await connection.SearchAsync(
            searchBase,
            LdapConnection.ScopeSub,
            searchFilter,
            attributes,
            false,
            searchConstraints
        );

        await foreach (LdapEntry entry in search)
        {
            var searchResults = new Dictionary<string, string>();
            try
            {
                LdapAttributeSet attributeSet = entry.GetAttributeSet();
                foreach (LdapAttribute attr in attributeSet)
                {
                    searchResults[attr.Name.ToLower()] = attr.StringValue;
                }
                list.Add(searchResults);
            }
            catch (LdapException e)
            {
                Console.WriteLine("Error: " + e.LdapErrorMessage);
            }
            finally
            {
                connection?.Disconnect();
            }
        }

        return list;
    }
    public async Task<LdapConnection?> ConnectAsync(string username, string password)
    {
        LdapConnection ldapConnection = new()
        {
            SecureSocketLayer = false,
            ConnectionTimeout = 10000,
        };
        try
        {
            string? connectionString = _configuration["environmentVariables:AD_CONNECTION_STRING"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = Environment.GetEnvironmentVariable("AD_CONNECTION_STRING");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception(
                    "LDAP connection string is missing in both appsettings and environment variables"
                );
            }

            string[] connectionParams = connectionString.Split("__");
            if (connectionParams.Length != 2)
            {
                throw new Exception("Invalid LDAP connection string format.");
            }

            string host = connectionParams[0];
            string domainName = connectionParams[1];

            await ldapConnection.ConnectAsync(host, 389);

            string formattedName = FormatUsername(username, domainName);
            await ldapConnection.BindAsync(formattedName, password);
            return ldapConnection;
        }
        catch (LdapException ex)
        {
            Console.WriteLine("LDAP Error: " + ex.LdapErrorMessage);
            return null;
        }
    }

    public async Task<bool> ValidateAsync(string username, string password)
    {
        LdapConnection? connection = await ConnectAsync(username, password);
        connection?.Disconnect();
        return connection != null;
    }

    public string FormatUsername(string username, string domainName)
    {
        username = username.ToLower();
        if (!username.Contains(domainName))
        {
            username = $"{username}@{domainName}";
        }
        return username;
    }
}