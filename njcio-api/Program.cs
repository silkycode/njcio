using Microsoft.EntityFrameworkCore;
using njcio_api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<NjcioContext>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("devCORSPolicy", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});
builder.WebHost.UseUrls("http://0.0.0.0:5001");

var app = builder.Build();

app.UseCors("devCORSPolicy");
app.MapControllers();

app.Run();
