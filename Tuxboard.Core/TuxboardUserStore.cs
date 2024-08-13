
using Microsoft.AspNetCore.Identity;
using Tuxboard;

// Step 1 
/* 
In your Startup.cs file, add the ASP Identity services to the ConfigureServices, confirue methods:
This allows you set the flag to ture if you want identity with tuxboard 
--> bool createTuxboardIdentity = Configuration.GetValue<bool>("CreateTuxboardIdentity", false);

public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<MyDbContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
    //This allows you set the flag to ture if you want identity with tuxboard 
    bool createTuxboardIdentity = Configuration.GetValue<bool>("CreateTuxboardIdentity", false);

    if (createTuxboardIdentity)
    {
        using (var scope = services.BuildServiceProvider().CreateScope())
        {
            var context = scope.ServiceProvider.GetService<MyDbContext>();

            if (!context.Database.EnsureCreated())
            {
                // If the database does not exist, create it
                context.Database.Migrate();
            }

            // Check if the Tuxboard identity tables exist
            if (!context.Users.Any() && !context.Roles.Any())
            {
                // If the tables do not exist, create them
                context.Database.Migrate();
            }
        }
    }

    services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<MyDbContext>()
        .AddDefaultTokenProviders();

    services.AddTuxboard();
}


public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // ...

    app.UseTuxboard(dashboard =>
    {
        dashboard.AddDashboard<MyDashboard>();

        dashboard.UseIdentity<TuxboardUserStore, IdentityUser, IdentityRole>();
    });
}
*/


// now add these for identity 
public class TuxboardUserStore : IUserStore<IdentityUser>
{
    private readonly MyDbContext _context;

    public TuxboardUserStore(MyDbContext context)
    {
        _context = context;
    }

    public Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken = default)
    {
        // Create a new user in the database
        _context.Users.Add(user);
        _context.SaveChanges();
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken = default)
    {
        // Delete a user from the database
        _context.Users.Remove(user);
        _context.SaveChanges();
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        // Find a user by ID in the database
        return Task.FromResult(_context.Users.Find(userId));
    }

    public Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        // Find a user by username in the database
        return Task.FromResult(_context.Users.FirstOrDefault(u => u.NormalizedUserName == normalizedUserName));
    }

    public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken = default)
    {
        // Get the normalized username for a user
        return Task.FromResult(user.NormalizedUserName);
    }

    public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken = default)
    {
        // Get the user ID for a user
        return Task.FromResult(user.Id);
    }

    public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken = default)
    {
        // Get the username for a user
        return Task.FromResult(user.UserName);
    }

    public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken = default)
    {
        // Update a user in the database
        _context.Users.Update(user);
        _context.SaveChanges();
        return Task.FromResult(IdentityResult.Success);
    }
}

// this can be a seperate file, but for simplicity, add your TuxboardRoleStore
using Microsoft.AspNetCore.Identity;
using Tuxboard;

public class TuxboardRoleStore : IRoleStore<IdentityRole>
{
    private readonly MyDbContext _context;

    public TuxboardRoleStore(MyDbContext context)
    {
        _context = context;
    }

    public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken = default)
    {
        // Create a new role in the database
        _context.Roles.Add(role);
        _context.SaveChanges();
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken = default)
    {
        // Delete a role from the database
        _context.Roles.Remove(role);
        _context.SaveChanges();
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = default)
    {
        // Find a role by ID in the database
        return Task.FromResult(_context.Roles.Find(roleId));
    }

    public Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        // Find a role by name in the database
        return Task.FromResult(_context.Roles.FirstOrDefault(r => r.NormalizedName == normalizedRoleName));
    }

    public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken = default)
    {
        // Get the normalized role name for a role
        return Task.FromResult(role.NormalizedName);
    }

    public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken = default)
    {
        // Get the role ID for a role
        return Task.FromResult(role.Id);
    }

    public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken = default)
    {
        // Get the role name for a role
        return Task.FromResult(role.Name);
    }

    public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken = default)
    {
        // Update a role in the database
        _context.Roles.Update(role);
        _context.SaveChanges();
        return Task.FromResult(IdentityResult.Success);
    }
}
