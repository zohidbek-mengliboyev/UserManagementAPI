namespace UserManagementAPI.Specifications
{
    public interface ISpecification<T>
    {
        string ToSql();
        string GetOrderBy();
        Dictionary<string, object> GetParameters();
    }
}
