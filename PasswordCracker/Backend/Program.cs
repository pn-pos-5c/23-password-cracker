using Backend;
using Backend.Services;

string corsKey = "_myCorsKey";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsKey, x => x.SetIsOriginAllowed(_ => true)
    .AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
});

// Add services to the container.
builder.Services.AddSingleton<PasswordHub>();
builder.Services.AddSignalR();
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
builder.Services.AddScoped<PasswordService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsKey);
// app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapHub<PasswordHub>("/PasswordCracker"));
app.UseMvc();
app.MapControllers();
app.Run();
