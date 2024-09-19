using Microsoft.AspNetCore.Http.Features;
using OpenSearch.Client;
using OpenSearch.Net;
using OpenSearchTest.Interfaces;
using OpenSearchTest.Services;

namespace OpenSearchTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 500 * 1024 * 1024);
            builder.Services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
            });
                // Add services to the container.

            builder.Services.AddControllers();
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var openSearchUri = new Uri("https://search-mytestdomain-5hba2rt273ckarshnxsegh2qye.aos.us-east-1.on.aws");
            var pool = new SingleNodeConnectionPool(openSearchUri);
            var settings = new ConnectionSettings(pool).DefaultIndex("search-data-index").BasicAuthentication("admin","Casper1030!");
            var client = new OpenSearchClient(settings);
            
            builder.Services.AddSingleton<IOpenSearchClient>(client);
            builder.Services.AddScoped<IOpenSearchService, OpenSearchService>();
            builder.Services.AddScoped<ISearchService, SearchService>();
            builder.Services.AddScoped<ISearchServiceOpenApi, SearchServiceOpenApi>();
            builder.Services.AddScoped<IUploadDataOpenSearchApi, UploadDataOpenSearchApi>();
            builder.Services.AddHttpClient<SearchServiceOpenApi>();
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("AllowAll");
            app.MapControllers();

            app.Run();
        }
    }
}
