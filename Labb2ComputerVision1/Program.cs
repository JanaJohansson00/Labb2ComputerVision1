using Labb2BildTjanster;

namespace Labb2BildTjanster
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // L�ser in inst�llningarna fr�n appsettings.json
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // L�gg till tj�nster f�r Azure Computer Vision
            builder.Services.AddSingleton<ComputerVisionService>();

            // L�gg till MVC-tj�nster
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Image}/{action=Upload}/{id?}");

            app.Run();
        }
    }
}