using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Jimu.Common
{
    public class JsonEnvParamParserFileProvider : IFileProvider
    {
        private class InMemoryFile : IFileInfo
        {
            private readonly byte[] _data;
            public InMemoryFile(string json)
            {
                string jsonStr;
                using (StreamReader sr = new StreamReader(json))
                {
                    jsonStr = sr.ReadToEnd();

                }
                if (!string.IsNullOrEmpty(jsonStr))
                {
                    var builder = new ConfigurationBuilder();
                    var config = builder.AddEnvironmentVariables().Build();
                    var reg = new Regex("([$]{[\\w\\d-_.]+})");
                    var envVars = reg.Matches(jsonStr);
                    foreach (Match match in envVars)
                    {
                        var envVarName = match.Value.TrimStart(new char[] { '$', '{' }).TrimEnd('}');
                        var envVarValue = config[envVarName];
                        if (!string.IsNullOrEmpty(envVarValue))
                        {
                            jsonStr = jsonStr.Replace(match.Value, envVarValue.Replace(@"\", @"\\"));
                        }
                    }
                }
                _data = Encoding.UTF8.GetBytes(jsonStr);
            }
            public Stream CreateReadStream() => new MemoryStream(_data);
            public bool Exists { get; } = true;
            public long Length => _data.Length;
            public string PhysicalPath { get; } = string.Empty;
            public string Name { get; } = string.Empty;
            public DateTimeOffset LastModified { get; } = DateTimeOffset.UtcNow;
            public bool IsDirectory { get; } = false;
        }

        private readonly IFileInfo _fileInfo;
        public JsonEnvParamParserFileProvider(string json) => _fileInfo = new InMemoryFile(json);
        public IFileInfo GetFileInfo(string _) => _fileInfo;
        public IDirectoryContents GetDirectoryContents(string _) => null;
        public IChangeToken Watch(string _) => NullChangeToken.Singleton;
    }
}
