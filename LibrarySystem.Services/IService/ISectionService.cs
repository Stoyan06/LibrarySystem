using LibrarySystem.Data.Repository;
using LibrarySystem.Models;
using System.Linq.Expressions;

namespace LibrarySystem.Services.IService
{
    public interface ISectionService:IRepository<Section>
    {
        Task<bool> ExisisSection(string name);
    }
}