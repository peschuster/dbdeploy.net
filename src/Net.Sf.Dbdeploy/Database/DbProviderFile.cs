using System.IO;
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

            if (string.IsNullOrEmpty(this.Path))
            {
                providerStream = typeof(DbProviderFile).Assembly.GetManifestResourceStream(typeof(DbProviderFile), ProviderFilename);
            }
            else if (!File.Exists(this.Path))
            {
                throw new FileNotFoundException("Could not load provider file from " + path);
            }
            else
            {
                providerStream = File.OpenRead(this.Path);
            }

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
    }
}