namespace TendersApi.Domain
{
    public class Supplier
    {
        public int Id { get; init; }
        public string Name { get; init; }

        public Supplier(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}

