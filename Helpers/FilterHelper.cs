namespace NoteFeature_App.Helpers
{
    public static class FilterHelper
    {
        public static List<string> ParseStatusFilter(string? statusFilter)
        {
            if (string.IsNullOrWhiteSpace(statusFilter))
                return new List<string>();
            
            return statusFilter.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(s => s.Trim())
                              .Where(s => !string.IsNullOrEmpty(s))
                              .ToList();
        }
    }
}
