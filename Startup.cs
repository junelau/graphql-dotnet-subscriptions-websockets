using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using GraphQL.Subscription;
using GraphQL.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using WebSocketManager;

namespace GraphQL.Subscriptions.Example
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            services.AddSingleton<ISubscriptionExecuter, SubscriptionExecuter>();
            services.AddSingleton<IWebSocketWriter, GQLWebSocketWriter>();
            services.AddSingleton<IWebSocketReceiver, GQLWebSocketReceiver>();
            services.AddSingleton<ChatQuery>();
            services.AddSingleton<ChatMutation>();
            services.AddSingleton<ChatSubscription>();
            services.AddSingleton<MessageType>();
            services.AddSingleton<MessageInputType>();
            services.AddSingleton<WebSocketConnectionManager>();

            var sp = services.BuildServiceProvider();
            services.AddSingleton<ISchema>(s =>
                new ChatSchema(new FuncDependencyResolver(type => (GraphType)s.GetService(type)))
            );

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();

            app.UseMvcWithDefaultRoute();
            app.MapWebSocketManager("/hub", serviceProvider.GetService<IWebSocketReceiver>(), "graphql-ws");
        }
    }
}
