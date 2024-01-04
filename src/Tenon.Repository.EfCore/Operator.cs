namespace Tenon.Repository.EfCore;

public class Operator<TAuditKey>
{
    public TAuditKey Id { get; set; }
    public string Account { get; set; }
}