using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace FluentValidation.Document.Rules
{
    public static class Helper
    {
        internal static string ResolvePath(string location)
        {
            return location
                       .Replace("{Desktop}", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), StringComparison.OrdinalIgnoreCase)
                       .Replace("{MyDocuments}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), StringComparison.OrdinalIgnoreCase);
        }

        private static string GetSolutionRoot()
        {
            if (Directory.GetCurrentDirectory().Contains("FluentValidation.Document.Rules", StringComparison.CurrentCultureIgnoreCase))
            {
                var myPathArray = Directory.GetCurrentDirectory().Split('\\');
                var sb = new StringBuilder();
                var i = 0;
                sb.Append($"{myPathArray[0]}\\");
                while (i + 1 < myPathArray.Length && myPathArray[i].ToLower() != "mrs.database")
                {
                    sb.Append($"{myPathArray[i + 1]}\\");
                    i++;
                }
                return sb.ToString();
            }
            else
            {
                var root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent + "\\FluentValidation.Document.Rules\\";
                return root;
            }
        }

        public static class Refelction
        {
            private const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Default;

            public static object RefelctInstanceField(object instance, string fieldName, Type type)
            {
                var field = type.GetField(fieldName, bindFlags);
                return field?.GetValue(instance);
            }

            public static object RefelctInstanceProperty(object instance, string propertyName)
            {
                string masterPropertyName = propertyName;
                if (propertyName.Contains("."))
                {
                    masterPropertyName = propertyName;
                    propertyName = masterPropertyName.Split('.')[0];
                }
                var fieldValue = instance.GetType().GetProperty(propertyName, bindFlags)?.GetValue(instance);
                if (masterPropertyName.Contains("."))
                {
                    masterPropertyName = masterPropertyName.Substring(propertyName.Length + 1);
                    fieldValue = RefelctInstanceProperty(fieldValue, masterPropertyName);
                }

                return fieldValue;
            }

            public static object ReflectInstanceField(object instance, string fieldName)
            {
                var t = instance.GetType();
                return RefelctInstanceField(instance, fieldName, t);
            }
        }

        public static class Settings
        {
            private static IConfiguration configuration;

            public static string AssemblyToDocument => Configuration["AssemblyToDocument"];

            public static IConfiguration Configuration
            {
                get
                {
                    if (configuration == null)
                    {
                        configuration = GetConfiguration();
                    }
                    return configuration;
                }
                private set => configuration = value;
            }

            public static IConfiguration GetConfiguration(string settingsFileName = "appsettings.json")
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), settingsFileName);
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException(path);
                }

                var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile(path, optional: false, reloadOnChange: true)
                        // .AddEnvironmentVariables()
                        ;

                path = Path.Combine(Directory.GetCurrentDirectory(), settingsFileName.Replace(".json", $".{Environment.UserName}.json"));
                if (File.Exists(path))
                {
                    builder.AddJsonFile(path, optional: false, reloadOnChange: true);
                }

#if DEBUG
                path = Path.Combine(Directory.GetCurrentDirectory(), "secrets.json");
                if (File.Exists(path))
                {
                    builder.AddJsonFile(path, optional: false, reloadOnChange: true);
                }
#endif
                return builder.Build();
            }

            public static class ExportFolder
            {
                public static string Markdown => ResolvePath(Configuration["ExportFolder:Markdown"]);
            }
        }
    }
}