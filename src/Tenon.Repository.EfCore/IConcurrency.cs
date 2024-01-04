namespace Tenon.Repository.EfCore
{
    public interface IConcurrency
    {
        public byte[] RowVersion { get; set; }
    }
}