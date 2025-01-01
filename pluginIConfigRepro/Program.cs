using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PluginBase;
using System.Reflection;

namespace pluginIConfigRepro
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);            

            try
            {
                List<string> pluginPaths =
                [                
                    @"HelloPlugin\bin\Debug\net9.0\HelloPlugin.dll"
                ];

                pluginPaths.ForEach((pluginPath) =>
                {
                    Assembly pluginAssembly = LoadPlugin(pluginPath);
                    RegisterCommands(pluginAssembly, builder);
                });
                var host = builder.Build();

                var configuration = host.Services.GetRequiredService<IConfiguration>();
                Console.WriteLine($"ApplicationName from config:\t {configuration["ApplicationName"]}");

                var commandServices = host.Services.GetServices<ICommand>().ToList();

                Console.WriteLine("Commands: ");
                commandServices.ForEach((command) =>
                {
                    Console.WriteLine($"{command.Name}\t - {command.Description}");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading assembly from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }  

        static void RegisterCommands(Assembly assembly, HostApplicationBuilder builder)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(ICommand).IsAssignableFrom(type))
                {
                    Console.WriteLine($"Registering type: {type}");
                    builder.Services.AddSingleton(typeof(ICommand), type);
                    count++;
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}
