using LibrarySystem.Data.Repository;
using LibrarySystem.Models;

namespace LibrarySystem.Services.IService
{
    public interface IAuthorService:IRepository<Author>
    {
        Task<bool> ExistsAuthor(string name);
    }
}
