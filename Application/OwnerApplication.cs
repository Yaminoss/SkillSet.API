using Skillest.API.DTO;
using Skillest.API.Extenstions;
using Skillest.API.Utilities;
using SkillSet.API.Models;
using SkillSet.API.Repositories;

namespace Skillest.API.Application
{
    public interface IOwnerApplication
    {
        Task<bool> AddOwners(IEnumerable<Owner> owners);
        OwnersDto GetAllOwners(OwnerParameters ownerParameters);
    }

    public class OwnerApplication : IOwnerApplication
    {
        private readonly IOwnerRepository _ownerRepo;
        public OwnerApplication(IOwnerRepository _context)
        {
            this._ownerRepo = _context;
        }

        public async Task<bool> AddOwners(IEnumerable<Owner> parsedOwners)
        {
            var owners = parsedOwners.ToLookup(o => _ownerRepo.IsNewUser(o.Id, o.Address));
            var newO = owners[true];
            var oldO = owners[false];
            return await _ownerRepo.AddOwners(newO) || await _ownerRepo.UpdateOwners(oldO);
        }

        public OwnersDto GetAllOwners(OwnerParameters ownerParameters)
        {
            return _ownerRepo.GetAllOwners(ownerParameters);
        }

    }
}