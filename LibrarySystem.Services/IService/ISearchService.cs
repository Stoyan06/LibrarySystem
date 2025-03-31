using LibrarySystem.Models;
using System.Web.Mvc;

namespace LibrarySystem.Services.IService
{
    public interface ISearchService
    {
        public Task<IEnumerable<object>> GetSuggestions(string query);

        public Task<List<SearchResults>> GetResults(string query);

        public Task<List<SearchResults>> ExtendedSearch(ExtendedSearchViewModel model);
    }
}
