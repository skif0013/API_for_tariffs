using API_for_tariffs;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using System.Reflection; // Добавьте это пространство имен

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddScoped<TariffService>();

// Swagger документация
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        
        Title = "Tariffs API",

        
    });

    // Поддержка комментариев из XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; // Теперь будет работать
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policyBuilder => policyBuilder
            .AllowAnyOrigin()    // Разрешить запросы с любого домена
            .AllowAnyHeader()    // Разрешить любые заголовки
            .AllowAnyMethod()    // Разрешить любые HTTP методы
            .WithExposedHeaders("X-Total-Count") // Экспонировать заголовок X-Total-Count
    );
});



builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Tariffs API");
    options.DocumentTitle = "Tariffs API Documentation";
    options.DefaultModelsExpandDepth(-1);
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
