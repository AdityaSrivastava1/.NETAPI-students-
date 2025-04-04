using ConnectDb;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


//  DB context
builder.Services.AddDbContext<AppDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();



app.MapGet("/", () => "Hello Welcome search Students!");


app.MapGet("/students", async (AppDbContext db) =>
    await db.Students.ToListAsync());

app.MapGet("/students/{id}", async (int id, AppDbContext db) =>
    await db.Students.FindAsync(id) is Student student
        ? Results.Ok(student)
        : Results.NotFound());

app.MapPost("/students", async (Student student, AppDbContext db) =>
{
    db.Students.Add(student);
    await db.SaveChangesAsync();
    return Results.Created($"/students/{student.Id}", student);
});

app.MapPut("/students/{id}", async (int id, Student updated, AppDbContext db) =>
{
    var student = await db.Students.FindAsync(id);
    if (student is null) return Results.NotFound();

    student.Name = updated.Name;
    await db.SaveChangesAsync();
    return Results.Ok(student);
});

app.MapDelete("/students/{id}", async (int id, AppDbContext db) =>
{
    var student = await db.Students.FindAsync(id);
    if (student is null) return Results.NotFound();

    db.Students.Remove(student);
    await db.SaveChangesAsync();
    return Results.Ok($"Student {id} deleted");
});


app.Run();
