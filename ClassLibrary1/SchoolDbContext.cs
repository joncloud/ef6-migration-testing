using System.Data.Entity;

namespace ClassLibrary1
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext() : base(nameof(SchoolDbContext))
        {
        }

        public SchoolDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        static SchoolDbContext()
        {
            Database.SetInitializer<SchoolDbContext>(null);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasRequired(x => x.Person).WithMany().HasForeignKey(x => x.PersonId);

            modelBuilder.Entity<Student>()
                .HasRequired(x => x.Person).WithMany().HasForeignKey(x => x.PersonId);

            modelBuilder.Entity<Person>()
                .Property(x => x.Name).HasMaxLength(24);
        }
    }
}
