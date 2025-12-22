using Microsoft.EntityFrameworkCore;

namespace Users.APP.Domain
{
    public class UsersDb : DbContext
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<UserMovie> UserMovies { get; set; }
        public UsersDb(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Title data of the Groups table can not have multiple same values.
            modelBuilder.Entity<Group>().HasIndex(groupEntity => groupEntity.Title).IsUnique();

            // Name data of the Roles table can not have multiple same values.
            modelBuilder.Entity<Role>().HasIndex(roleEntity => roleEntity.Name).IsUnique();

            // UserName data of the Users table can not have multiple same values.
            modelBuilder.Entity<User>().HasIndex(userEntity => userEntity.UserName).IsUnique();

            // Composite index on FirstName and LastName for optimizing searches involving both fields.
            modelBuilder.Entity<User>().HasIndex(userEntity => new { userEntity.FirstName, userEntity.LastName });

            // Relationship configurations:
            // Configuration should start with the entities that have the foreign keys.
            modelBuilder.Entity<UserRole>()
                .HasOne(userRoleEntity => userRoleEntity.User) 
                .WithMany(userEntity => userEntity.UserRoles) 
                .HasForeignKey(userRoleEntity => userRoleEntity.UserId) 
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserRole>()
                .HasOne(userRoleEntity => userRoleEntity.Role) 
                .WithMany(roleEntity => roleEntity.UserRoles) 
                .HasForeignKey(userRoleEntity => userRoleEntity.RoleId) 
                .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<User>()
                .HasOne(userEntity => userEntity.Group) 
                .WithMany(groupEntity => groupEntity.Users) 
                .HasForeignKey(userEntity => userEntity.GroupId) 
                .OnDelete(DeleteBehavior.NoAction); 
            
            modelBuilder.Entity<UserMovie>()
                .HasKey(userMovie => new { userMovie.UserId, userMovie.MovieId });

            modelBuilder.Entity<UserMovie>()
                .HasOne(userMovie => userMovie.User)
                .WithMany()
                .HasForeignKey(userMovie => userMovie.UserId);
            
            modelBuilder.Entity<Group>().HasData(
                new Group { Id = 1, Title = "Child" },
                new Group { Id = 2, Title = "Adult" }
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Customer" } 
            );
            
        }
    }
}