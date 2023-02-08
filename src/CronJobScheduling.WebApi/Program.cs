var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCronJobScheduling();
builder.Services.AddCronJobSchedulingJobs();
builder.Services.AddCronJobSchedulingDataStore(
    migrationsAssembly: typeof(ApplicationDbContext).Assembly,
    connectionString: builder.Configuration.GetConnectionString("Npgsql"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCronJobSchedulingDataStore();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.RunCronJobScheduling();

app.Run();
