using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdamCode.NoticeBoard.Theme
{
    public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
    {
        private static ResourceManifest _manifest;

        static ResourceManagementOptionsConfiguration()
        {
            _manifest = new ResourceManifest();

            _manifest
                .DefineScript("jquery")
                .SetUrl("~/");
        }
        public void Configure(ResourceManagementOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
