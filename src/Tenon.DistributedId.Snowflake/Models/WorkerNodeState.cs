namespace Tenon.DistributedId.Snowflake.Models
{
    internal sealed class WorkerNodeState(string workerKey, string workerValue, int milliSeconds)
    {
        public readonly string WorkerKey = workerKey;
        public readonly double MilliSeconds = milliSeconds;
        public readonly string WorkerValue = workerValue;
    }
}