using Xunit;

// 使用默认 MemoryCache 的测试共享 MemoryCache.Default，必须串行执行以避免互相干扰。
[assembly: CollectionBehavior(DisableTestParallelization = true)]
