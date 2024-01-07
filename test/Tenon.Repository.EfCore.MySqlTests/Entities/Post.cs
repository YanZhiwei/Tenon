namespace Tenon.Repository.EfCore.MySqlTests.Entities;

public class Post : EfBasicAuditEntity
{
    public string Title { get; set; }
    public string Content { get; set; }
    public virtual Blog Blog { get; set; }
}