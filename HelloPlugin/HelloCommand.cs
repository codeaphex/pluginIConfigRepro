using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PluginBase;

namespace HelloPlugin
{
    public class HelloCommand : ICommand
    {
        private readonly IConfiguration? _configuration;
        private readonly IServiceProvider? _serviceProvider;

        // This ctor fails immediately
        //public HelloCommand(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //    var baseUrl = configuration["Plugins:HOU:BaseUrl"];
        //}

        public HelloCommand(IServiceProvider serviceProvider)
        {
            // IConfiguration seems to be listed in descriptors
            // But fails when trying to retrieve an instance
            _serviceProvider = serviceProvider;
            //_configuration = serviceProvider.GetRequiredService<IConfiguration>();
        }

        public string Name { get => "hello"; }
        public string Description { get => "Displays hello message."; }

        public int Execute()
        {
            Console.WriteLine("Hello !!!");
            return 0;
        }
    }
}
