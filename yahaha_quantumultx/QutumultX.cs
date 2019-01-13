using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yahaha_quantumultx
{
    public class QutumultX
    {
        public string version { get; set; }
        public Object error { get; set; }
        public Data data { get; set; }
    }
    public class Error
    {
        public int code { get; set; }
        public string message { get; set; }
    }
    public class Data
    {
        public string provider_image_url { get; set; }
        public string provider_name { get; set; }
        public string account_description { get; set; }
        public int extension_check_interval { get; set; }
        public int container_check_interval { get; set; }
        public int expire_time { get; set; }
        public long upload { get; set; }
        public long download { get; set; }
        public long total { get; set; }
        public int configuration_profile_modified_at { get; set; }
        public string configuration_profile_url { get; set; }
        public List<Servers> servers { get; set; }
        public string tips { get; set; }
    }
    public class Servers
    {
        public string tag { get; set; }
        public bool online { get; set; }
        public string description { get; set; }
    }
}
