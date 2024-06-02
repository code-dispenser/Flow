using Flow.Core.Demos.AppServer.Areas.Customers;
using Flow.Core.Demos.AppServer.Common.ErrorHandlers;
using Flow.Core.Demos.AppServer.Common.Seeds;
using Flow.Core.Demos.AppServer.EFCore;
using Flow.Core.Demos.AppServer.GrpcServices;
using Flow.Core.Demos.Contracts.Utilities;
using Microsoft.EntityFrameworkCore;
using ProtoBuf.Grpc.Server;
using System.Text.Json.Serialization.Metadata;

namespace Flow.Core.Demos.AppServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /*
                * Only needed if you want to use your own custom failure classes for grpc code first serialization/deserialization like the example. 
            */ 
            CustomFailureHelper.AddCustomFailuresToGrpcRuntimeModel();

            var builder = WebApplication.CreateBuilder(args);

            /*
                * Only needed if you want to use your own custom failure classes for json serialization/deserialization like the example. 
            */
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.TypeInfoResolver = CustomFailureHelper.GetJsonCustomFailureTypeResolver();
                
                //Or options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(CustomFailureHelper.AddCustomFailuresToJsonResolver);
                
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
            //builder.Services.AddControllers();

            builder.Services.AddDbContext<IDbContextWrite, CustomersDbWrite>(options =>
            {
                options.UseSqlite("DataSource=Customers.sqlite;");
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            });

            builder.Services.AddDbContext<IDbContextReadOnly, CustomersDbReadOnly>(options =>
            {
                options.UseSqlite("DataSource=Customers.sqlite;Mode=ReadOnly;");
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });


            builder.Services.AddTransient<CustomerSearchQueryHandler>();
            builder.Services.AddTransient<AddCustomerCommandHandler>();
            builder.Services.AddSingleton<IDbExceptionHandler, SqliteDbExceptionHandler>();
            
            builder.Services.AddCodeFirstGrpc(); 

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.MapGrpcService<GrpcCustomerService>();

            app.Run();

           
        }

    }
}
