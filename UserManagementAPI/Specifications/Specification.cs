namespace UserManagementAPI.Specifications
{
    public abstract class Specification<T> : ISpecification<T>
    {
        protected readonly List<string> Conditions = new();
        protected readonly Dictionary<string, object> Parameters = new();
        protected string Sorting { get; set; } = string.Empty;

        public virtual string ToSql()
        {
            return Conditions.Count > 0 ? string.Join(" AND ", Conditions) : string.Empty;
        }

        public virtual Dictionary<string, object> GetParameters()
        {
            return Parameters;
        }

        public string GetOrderBy()
        {
            return Sorting;
        }
    }
}
