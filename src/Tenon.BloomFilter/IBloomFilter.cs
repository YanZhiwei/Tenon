using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tenon.BloomFilter
{
    public interface IBloomFilter
    {
        Task<bool> AddAsync(string key, string value);
        bool Add(string key, string value);
        Task<bool[]> AddAsync(string key, IEnumerable<string> values);
        bool[] Add(string key, IEnumerable<string> values);
        Task<bool> ExistsAsync(string key, string value);
        bool Exists(string key, string value);
        Task<bool[]> ExistsAsync(string key, IEnumerable<string> values);
        Task ReserveAsync(string key, double errorRate, int initialCapacity);
        void Reserve(string key, double errorRate, int initialCapacity);
    }
}