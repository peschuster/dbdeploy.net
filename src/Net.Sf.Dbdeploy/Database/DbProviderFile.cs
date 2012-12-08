using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Net.Sf.Dbdeploy.Database
{
    public class DbProviderFile
    {
        public const string ProviderFilename = @"dbproviders.xml";

        private string path;

        public DbProviderFile()
        {
            this.path = null;
        }

        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        public DbProviders LoadProviders()
        {
            Stream providerStream;

            if (!string.IsNullOrEmpty(this.Path) && !File.Exists(this.Path))
                throw new FileNotFoundException("Could not load provider file from " + this.Path);

            string path = string.IsNullOrEmpty(this.Path)
                ? GetDefaultPath()
                : this.Path;

            providerStream = File.Exists(path) 
                ? File.OpenRead(path) 
                : typeof(DbProviderFile).Assembly.GetManifestResourceStream(typeof(DbProviderFile), ProviderFilename);

            try
            {
                using (var reader = new XmlTextReader(providerStream))
                {
                    providerStream = null;

                    var serializer = new XmlSerializer(typeof(DbProviders));

                    return (DbProviders)serializer.Deserialize(reader);
                }
            }
            finally
            {
                if (providerStream != null)
                    providerStream.Dispose();
            }
        }

        private static string GetDefaultPath()
        {
            DirectoryInfo assemblyDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;

            string providerFilePath = System.IO.Path.Combine(assemblyDirectory.FullName, ProviderFilename);

            if (!File.Exists(providerFilePath))
            {
                providerFilePath = System.IO.Path.Combine(Environment.CurrentDirectory, ProviderFilename);
            }

            return providerFilePath;
        }
    }
}