using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Your API", Version = "v1" });

    // Swagger'da güvenlik tanımlaması ekleme
    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        { securityScheme, new[] { "Bearer" } },
    };
    c.AddSecurityRequirement(securityRequirement);
});
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddSingleton<AuthenticationService>();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("studentapigizemgizemapigizemapigizemapi"))
        };
    });
   builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    c.RoutePrefix = "swagger";

    // SwaggerUI'da token girişi yapabileceğiniz bir alan ekleyin
    c.InjectStylesheet("/swagger/custom.css");
});
}

app.UseHttpsRedirection();


app.MapPost("/login", (string username, string password, [FromServices] AuthenticationService authService) =>
{
    // Replace this with your authentication logic
    var user = new { Username = "gizem", UserId = 123 };

    // Create token
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes("studentapigizemgizemapigizemapigizemapi");
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
        }),
        Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);

    // Return token
    return new { Token = tokenString };
})
.WithName("Login")
.WithOpenApi();


app.MapPost("/student", (  Student student) =>
{
    var context= new AppDbContext();
   var newstudent= context?.Students?.Add(student);
   context?.SaveChanges();
   return newstudent?.Entity;
})
.WithName("AddStudent")
.WithOpenApi();


app.MapPost("/lesson", ( Lesson lesson) =>
{
    var context= new AppDbContext();
   var newlesson= context?.Lessons?.Add(lesson);
   context?.SaveChanges();
   return newlesson?.Entity;
})
.WithName("AddLesson")
.WithOpenApi();

app.MapPut("/updatestudent", ( Student student) =>
{
    var context= new AppDbContext();
   var newstudent= context?.Students?.Update(student);
   context?.SaveChanges();
   return newstudent?.Entity;
})
.WithName("UpdateStudent")
.WithOpenApi();

app.MapPut("/updatelesson", ( Lesson lesson) =>
{
    var context= new AppDbContext();
   var newstudent= context?.Lessons?.Update(lesson);
   context?.SaveChanges();
   return newstudent?.Entity;
})
.WithName("UpdateLesson")
.WithOpenApi();

app.MapDelete("/deletelesson/{lessonId}", ( int lessonId) =>
{
    
    var context= new AppDbContext();
   var deletedlesson= context?.Lessons?.Find(lessonId);
   if(deletedlesson != null)
   {
    context?.Lessons?.Remove(deletedlesson);
 context?.SaveChanges();
   return "Ders Silindi";
   }else
   {
    return "Ders Bulunamadı";

   }
   
})
.WithName("deleteLesson")
.WithOpenApi();

app.MapDelete("/deletestudent/{studentId}", ( int studentId) =>
{
    
    var context= new AppDbContext();
   var deletedstudent= context?.Students?.Find(studentId);
   if(deletedstudent != null)
   {
    var relatedLessons = context?.Lessons?.Where(x => x.StudentID == studentId).ToList();
    if (relatedLessons != null)
            {
                return "Öğrencinin Ders Kaydı Bulunmaktadır Silinemez";
            }
    context?.Students?.Remove(deletedstudent);
    context?.SaveChanges();
   return "Öğrenci Silindi";
   }
   else
   {
    return "Öğrenci Bulunamadı";

   }
   
})
.WithName("deletestudent")
.WithOpenApi();

app.MapGet("/getstudent", (int page=1, int pageSize=10) =>
{
    var context= new AppDbContext();
    var totalStudents = context?.Students?.Count();
   var newstudent= context?.Students?.Where(x => x.StudentID >= (page - 1) * pageSize + 1 && x.StudentID <= page * pageSize)
        .Include(x => x.Lessons)
        .ToList();
   context?.SaveChanges();
    return new
    {
        TotalStudents = totalStudents,
        Page = page,
        PageSize = pageSize,
        Students = newstudent
    };
})
.WithName("getstudent")
.WithOpenApi();


app.UseAuthentication();
app.UseAuthorization();
app.Run();



record AddStudent()
{
    
}
