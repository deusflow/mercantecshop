using Novell.Directory.Ldap;

namespace WebShopMercantec.Services
{
    public interface ILdapService
    {
        Task<List<Dictionary<string, string>>> SearchAsync(string searchBase, string[] attributes, string searchFilter);
        Task<LdapConnection?> ConnectAsync(string username, string password);
        Task<bool> ValidateAsync(string username, string password);
        string FormatUsername(string username, string domainName);
    }
}
