using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediafulTest
{
    public class MockBrowserFile : IBrowserFile
    {
        public string Name { get; }

        public DateTimeOffset LastModified { get; }

        public long Size { get; }

        public string ContentType { get; }

        public byte[] Content { get; }

        public MockBrowserFile(string name, DateTimeOffset lastModified, long size, string contentType, byte[] content)
        {
            Name = name;
            LastModified = lastModified;
            Size = size;
            ContentType = contentType;
            Content = content;
        }

        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default) => new MemoryStream(Content);
    }
}
