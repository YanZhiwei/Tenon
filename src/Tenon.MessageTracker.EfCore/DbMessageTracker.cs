using Tenon.DistributedId.Abstractions;
using Tenon.MessageTracker.Abstractions;
using Tenon.MessageTracker.EfCore.Entities;
using Tenon.Repository.EfCore;

namespace Tenon.MessageTracker.EfCore;

public sealed class DbMessageTracker(IDGenerator idGenerator, EfRepository<EventTracker> trackerRepository)
    : IMessageTracker
{
    private readonly IDGenerator _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));

    private readonly EfRepository<EventTracker> _trackerRepository =
        trackerRepository ?? throw new ArgumentNullException(nameof(trackerRepository));

    public string Name => nameof(DbMessageTracker);

    public async Task<bool> HasProcessedAsync(long eventId, string trackerName, CancellationToken token = default)
    {
        return await _trackerRepository.AnyAsync(x => x.EventId == eventId && x.TrackerName == trackerName, token);
    }

    public async Task MarkAsProcessedAsync(long eventId, string trackerName, CancellationToken token = default)
    {
        await _trackerRepository.InsertAsync(new EventTracker
        {
            Id = _idGenerator.GetNextId(),
            EventId = eventId,
            TrackerName = trackerName
        }, token);
    }
}