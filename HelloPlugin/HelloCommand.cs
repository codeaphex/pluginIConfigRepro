using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PluginBase;

namespace HelloPlugin
{
    public class HelloCommand : ICommand
    {
        private readonly IConfiguration _configuration;

        public HelloCommand(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Name { get => "hello"; }
        public string Description { get => "Displays hello message."; }

        public int Execute()
        {
            Console.WriteLine(_configuration["HelloMessage"]);
            return 0;
        }
    }
}
