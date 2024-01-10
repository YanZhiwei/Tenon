using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tenon.BloomFilter
{
    public interface IBloomFilter
    {
        Task<bool> AddAsync(string key, string value);
        Task<bool[]> AddAsync(string key, IEnumerable<string> values);
        Task<bool> ExistsAsync(string key, string value);
        Task<bool[]> ExistsAsync(string key, IEnumerable<string> values);
        Task ReserveAsync(string key, double errorRate, int initialCapacity);
    }
}