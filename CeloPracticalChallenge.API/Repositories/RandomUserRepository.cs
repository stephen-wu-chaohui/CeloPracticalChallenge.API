using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CeloPracticalChallenge.API.Entities;
using CeloPracticalChallenge.API.Models;
using CeloPracticalChallenge.API.ResourceParameters;

namespace CeloPracticalChallenge.API.Repositories
{
    public class RandomUserRepository : IRandomUserRepository
    {
        private readonly CeloPracticalChallengeDBContext _context;

        public RandomUserRepository(CeloPracticalChallengeDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); ;
        }

        public async Task<IEnumerable<RandomUser>> ListAsync(RandomUsersResourceParameters parameters)
        {
            if (parameters == null) {
                throw new ArgumentNullException(nameof(parameters));
            }

            var collection = _context.RandomUser as IQueryable<RandomUser>;

            if (!string.IsNullOrWhiteSpace(parameters.FirstName)) {
                var firstName = parameters.FirstName.Trim();
                collection = collection.Where(a => a.FirstName == firstName);
            }

            if (!string.IsNullOrWhiteSpace(parameters.LastName)) {
                var lastName = parameters.LastName.Trim();
                collection = collection.Where(a => a.LastName == lastName);
            }

            if (parameters.Results > 0) {
                collection = collection.Take(parameters.Results);
            }

            return await collection.ToListAsync();
        }

        public async Task<RandomUser> GetAsync(int id)
        {
            var randomUser = await _context.RandomUser.FindAsync(id);

            if (randomUser == null) {
                return null;
            }

            return randomUser;
        }

        public async Task<bool> ModifyAsync(RandomUser randomUser)
        {
            _context.Entry(randomUser).State = EntityState.Modified;
            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!RandomUserExists(randomUser.Id)) {
                    return false;
                } else {
                    throw;
                }
            }

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var randomUser = await _context.RandomUser.FindAsync(id);
            if (randomUser == null) {
                return false;
            }

            _context.RandomUser.Remove(randomUser);
            await _context.SaveChangesAsync();

            return true;
        }

        private bool RandomUserExists(int id) => _context.RandomUser.Any(u => u.Id == id);
    }
}
