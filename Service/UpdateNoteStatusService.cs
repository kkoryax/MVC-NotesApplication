using Microsoft.EntityFrameworkCore;
using NoteFeature_App.Data;
using NoteFeature_App.Helpers;

public class UpdateNoteStatusService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public UpdateNoteStatusService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdateNoteStatus();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task UpdateNoteStatus()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

            var now = DateTime.Now;

            var notesToUpdate = await db.Notes
                .Where(n => n.FlagActive)
                .ToListAsync();

            int updatedCount = 0;

            foreach (var note in notesToUpdate)
            {
                bool shouldBePublic = CaculateIsPublicHelper.CalculateIsPublic(note.ActiveFrom, note.ActiveUntil);

                if (note.IsPublic != shouldBePublic)
                {
                    note.IsPublic = shouldBePublic;
                    updatedCount++;
                }
            }

            if (updatedCount > 0)
            {
                await db.SaveChangesAsync();
            }
        }
    }
}
