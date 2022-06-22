using System.Linq;
using Microsoft.EntityFrameworkCore;
using Skillest.API.DTO;
using Skillest.API.Extenstions;
using Skillest.API.Utilities;
using SkillSet.API.Data;
using SkillSet.API.Models;

namespace SkillSet.API.Repositories
{

    public interface IOwnerRepository
    {
        Task<bool> AddOwners(IEnumerable<Owner> owners);
        Task<bool> UpdateOwners(IEnumerable<Owner> ownersId);
        bool IsNewUser(int id, string address);
        OwnersDto GetAllOwners(OwnerParameters ownerParameters);
    }

    public class OwnerRepository : IOwnerRepository
    {
        private readonly Context _context;
        public OwnerRepository(Context context)
        {
            _context = context;
        }

        public async Task<bool> AddOwners(IEnumerable<Owner> owners)
        {
            if (owners.Count() > 0)
            {
                await _context.Owners.AddRangeAsync(owners);
                return _context.SaveChanges() > 0;
            }
            return false;
        }

        public async Task<bool> UpdateOwners(IEnumerable<Owner> owners)
        {
            if (owners.Count() > 0)
            {
                var dbOwners = await _context.Owners.Where(o => owners.Select(x => x.Id).Contains(o.Id)).AsNoTracking().ToListAsync();
                var toUpdate = new List<Owner>();
                foreach (var owner in dbOwners)
                {
                    var newValues = owners.FirstOrDefault(o => o.Id == owner.Id);
                    owner.DateOfBirth = newValues?.DateOfBirth;
                    owner.FirstName = newValues?.FirstName;
                    owner.LastName = newValues?.LastName;
                    toUpdate.Add(newValues);
                }
                _context.Owners.UpdateRange(toUpdate);
                return _context.SaveChanges() > 0;
            }
            return false;
        }

        public bool IsNewUser(int id, string address)
        {
            if (_context.Owners.Any(o => o.Address == address) || _context.Owners.Any(o => o.Id == id))
                return false;
            return true;
        }

        public OwnersDto GetAllOwners(OwnerParameters ownerParameters)
        {
            if (ownerParameters.provinceCode == "")
            {
                return new OwnersDto()
                {
                    length = _context.Owners.Count(),
                    owners = _context.Owners.OrderBy(on => on.Id)
                            .Skip((ownerParameters.PageNumber - 1) * ownerParameters.PageSize)
                            .Take(ownerParameters.PageSize)
                            .ToList()
                };
            }
            else
            {
                var codes = ownerParameters.provinceCode.Split(',', StringSplitOptions.TrimEntries).ToList();
                var owners = _context.Owners.OrderBy(on => on.Id).ToList();
                var filteredOwners = new List<Owner>();
                for (int i = 0; i < owners.Count(); i++)
                {
                    if (codes.Contains(ConverteAddressToCode(owners[i].Address)))
                    {
                        Console.WriteLine(ConverteAddressToCode(owners[i].Address));
                        filteredOwners.Add(owners[i]);
                    }
                }
                return new OwnersDto()
                {
                    length = filteredOwners.Count(),
                    owners = filteredOwners.Skip((ownerParameters.PageNumber - 1) * ownerParameters.PageSize)
                                .Take(ownerParameters.PageSize).ToList()
                };
            }
        }

        private string ConverteAddressToCode(string address)
        {
            var x = address.Split(',', StringSplitOptions.None)[1].Substring(0, 3).Trim();
            return x;
        }
    }
}