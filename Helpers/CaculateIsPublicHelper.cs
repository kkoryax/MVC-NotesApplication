namespace NoteFeature_App.Helpers
{
    public class CaculateIsPublicHelper
    {
        public static bool CalculateIsPublic(DateTime activeFrom, DateTime? activeUntil)
        {
            var now = DateTime.Now;

            if (activeFrom > now)
            {
                return false;
            }

            if (activeUntil.HasValue && activeUntil.Value < now)
            {
                return false;
            }

            return true;
        }
    }
}
