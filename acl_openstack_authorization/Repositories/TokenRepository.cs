namespace acl_openstack_authorization.Repositories
{
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    public interface ITokenRepository
    {
        Task AddToBlackList(string token);
        Task<bool> IsTokenBlacklisted(string token);
    }

    public class TokenRepository : ITokenRepository
    {
        private static readonly ConcurrentDictionary<string, bool> _blacklist = new ConcurrentDictionary<string, bool>();

        public Task AddToBlackList(string token)
        {
            _blacklist[token] = true;
            return Task.CompletedTask;
        }

        public Task<bool> IsTokenBlacklisted(string token)
        {
            return Task.FromResult(_blacklist.ContainsKey(token));
        }
    }
}
