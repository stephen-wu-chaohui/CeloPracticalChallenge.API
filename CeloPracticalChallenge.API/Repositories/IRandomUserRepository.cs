using System.Collections.Generic;
using System.Threading.Tasks;
using CeloPracticalChallenge.API.Entities;
using CeloPracticalChallenge.API.ResourceParameters;

namespace CeloPracticalChallenge.API.Repositories
{
    public interface IRandomUserRepository
    {
        Task<IEnumerable<RandomUser>> ListAsync(RandomUsersResourceParameters parameters);

        Task<RandomUser> GetAsync(int id);

        Task<bool> ModifyAsync(RandomUser randomUser);

        Task<bool> DeleteAsync(int id);
    }
}
