namespace ProjectArena.Domain.Registry
{
    public interface IContentMigration
    {
        void Up(RegistryContext context);
    }
}