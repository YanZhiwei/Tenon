namespace Tenon.DistributedId.Snowflake.Exceptions;

public sealed class IdGeneratorWorkerNodeException(string message) : Exception(message);