using System.Text;
using dotnet_notely.Configurations;
using dotnet_notely.Contracts;
using dotnet_notely.Data;
using dotnet_notely.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8080");

var connectionString = builder.Configuration.GetConnectionString("NotelyDbConnectionString");
builder.Services.AddDbContext<NotelyDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentityCore<ApiUser>()
   .AddRoles<IdentityRole>()
   .AddEntityFrameworkStores<NotelyDbContext>();

builder.Services.AddAutoMapper(typeof(MapperConfig));
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();

builder.Services.AddAuthentication(options => {
   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
   options.TokenValidationParameters = new TokenValidationParameters {
       ValidateIssuerSigningKey = true,
       ValidateIssuer = true,
       ValidateAudience = true,
       ValidateLifetime = true,
       ClockSkew = TimeSpan.Zero,
       ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
       ValidAudience = builder.Configuration["JwtSettings:Audience"],
       IssuerSigningKey = new SymmetricSecurityKey(
           Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
   };
});

builder.Services.AddAuthorization(options => {
   options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
   options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

builder.Services.AddCors(options => {
   options.AddPolicy("AllowAll", 
      builder => builder
         .AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader());
});

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

try 
{
   var logger = app.Services.GetRequiredService<ILogger<Program>>();
    
   using (var scope = app.Services.CreateScope())
   {
      var context = scope.ServiceProvider.GetRequiredService<NotelyDbContext>();
      await context.Database.MigrateAsync();
      logger.LogInformation("Database migrations completed successfully");
   }
}
catch (Exception ex)
{
   var logger = app.Services.GetRequiredService<ILogger<Program>>();
   logger.LogError(ex, "An error occurred while initializing the database");
   throw;
}

if (app.Environment.IsDevelopment()) {
   app.UseSwagger();
   app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseRouting(); 
// app.UseCors("CorsPolicy");
// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();

app.MapControllers();
app.MapHub<NoteHub>("/hubs/signal");

app.Run();