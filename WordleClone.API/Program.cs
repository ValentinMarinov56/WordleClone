using System.Text;
using Microsoft.EntityFrameworkCore;
using WordleClone.API.Data;
using WordleClone.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi(); // what are these
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => // what is this
{
    options.AddPolicy("ReactFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<ILoginService,LoginService>();
builder.Services.AddScoped<IPasswordHasher,PasswordHasher>(); 
builder.Services.AddScoped<IWordlService,WordlService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUserService, UserService>(); //When something requests an IUserService, provide a UserService instance.

//Token
var secretKey = Environment.GetEnvironmentVariable("SECRET") ?? "your-secret-key-change-this-in-production"; // we get the key from .env or give a default one if its not found
var key = Encoding.ASCII.GetBytes(secretKey); // array of bytes

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // value = bearer / looks for token in header
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // if the header doesnt contain a token -> typical response is 401 / challenge = no token
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters // rules for a valid token
    {
        ValidateIssuerSigningKey = true, // detecs if the token was signed with the secret
        IssuerSigningKey = new SymmetricSecurityKey(key), // use the secret as signature
        ValidateIssuer = false, // checks who sighned the token (WordleClone.API) if true
        ValidateAudience = false, // checks for who the token is sighed (WordleClone) if true
        ValidateLifetime = true, // checks if the token is still valid time wise 
        ClockSkew = TimeSpan.Zero // no clock tolerance
    };
});

builder.Services.AddSingleton<IJwtService>(new JwtService(secretKey,60)); // stateless service that uses the same configuration to generate JWT tokens


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var frontendPath = Path.Combine(app.Environment.ContentRootPath, "dist"); // path to the dist folder of the client app, which is the default output folder for Vite build. This is where the built frontend files are located.
if (Directory.Exists(frontendPath))
{
    var frontendFiles = new PhysicalFileProvider(frontendPath); // this is used to serve static files from the dist folder. It provides a way to access the files in the dist folder as if they were part of the web application.
    app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = frontendFiles }); // this is used to serve the default file (index.html) from the dist folder when the root URL is accessed. It looks for index.html in the dist folder and serves it as the default file.
    app.UseStaticFiles(new StaticFileOptions { FileProvider = frontendFiles }); // this is used to serve static files (like CSS, JS, images) from the dist folder. It allows the application to serve the built frontend files to the client when requested.
}
app.UseCors("ReactFrontend");
app.UseAuthentication(); // this needs to be before app.UseAuthorization() so that the user is authenticated before checking if they are authorized
app.UseAuthorization();
app.MapControllers();
app.MapFallback(async context =>
{
    if (context.Request.Path.StartsWithSegments("/api") ||
        !HttpMethods.IsGet(context.Request.Method))
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    } // if the request is not for an API endpoint and is a GET request, we will try to serve the index.html file from the dist folder.

    var indexPath = Path.Combine(frontendPath, "index.html");
    if (File.Exists(indexPath))
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(indexPath);
        return;
    } // if the index.html file is not found, we will return a 404 Not Found response.

    context.Response.StatusCode = StatusCodes.Status404NotFound; // if the request is for an API endpoint or is not a GET request, we will return a 404 Not Found response.
});

app.Run();
