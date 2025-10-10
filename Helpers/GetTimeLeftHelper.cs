using NoteFeature_App.Models.Note;

namespace NoteFeature_App.Helpers
{
    public static class GetTimeLeftHelper
    {
        public static string GetTimeLeftDisplay(this NoteModel note)
        {
            if (note?.ActiveUntil == null)
                return "Unknown";

            TimeSpan time_left = note.ActiveUntil.Value - DateTime.Now;

            if (time_left.TotalDays >= 730) return "Over a years";
            if (time_left.TotalDays >= 365) return "Over a year";
            if (time_left.TotalDays >= 1) return $"{Math.Floor(time_left.TotalDays)} Days Left";
            if (time_left.TotalHours >= 1) return $"{Math.Floor(time_left.TotalHours)} Hours Left";
            if (time_left.TotalMinutes >= 1) return $"{Math.Floor(time_left.TotalMinutes)} Minutes Left";
            if (time_left.TotalSeconds > 0) return "Less than a minute";

            return "Expired soon";
        }
    }
}
