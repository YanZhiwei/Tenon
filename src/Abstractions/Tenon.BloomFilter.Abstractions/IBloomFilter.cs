using System.Collections.Generic;
using System.Threading.Tasks;
using Tenon.BloomFilter.Abstractions.Configurations;

namespace Tenon.BloomFilter.Abstractions;

public interface IBloomFilter
{
    BloomFilterOptions Options { get; }
    Task InitAsync();
    void Init();
    Task<bool> AddAsync(string value);
    bool Add(string value);
    Task<bool[]> AddAsync(IEnumerable<string> values);
    bool[] Add(IEnumerable<string> values);
    Task<bool> ExistsAsync(string value);
    bool Exists(string value);
    Task<bool[]> ExistsAsync(IEnumerable<string> values);
    Task<bool> ExistsAsync();
    bool Exists();
}