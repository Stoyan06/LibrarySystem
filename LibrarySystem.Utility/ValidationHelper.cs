using LibrarySystem.Models;

namespace LibrarySystem.Utility
{
    public static class ValidationHelper
    {
        public static bool IsValidIsbn(string value)
        {
            if (value == null) return false;
            if (value.Count() == 13) return true;
            else return false;
        }

        public static bool SectionExists(int id)
        {
            return false;
        }
    }
}
