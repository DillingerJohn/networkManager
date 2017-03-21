namespace NetworkManager.Data.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<NetAppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "AppDbContext";
        }

        protected override void Seed(NetAppDbContext context)
        {
            base.Seed(context);
        }
    }
}
