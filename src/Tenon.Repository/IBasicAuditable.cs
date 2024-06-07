namespace Tenon.Repository;

public interface IBasicAuditable
{
    DateTimeOffset CreatedAt { get; set; }

    DateTimeOffset? UpdatedAt { get; set; }

    DateTimeOffset? DeletedAt { get; set; }
}