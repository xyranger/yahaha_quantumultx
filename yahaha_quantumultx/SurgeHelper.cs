using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace yahaha_quantumultx
{
    public class SurgeHelper
    {
        // static string[] region = new string[] { "CN", "HK", "JP", "KR", "MO", "PH", "RUS", "SG", "TW", "UK", "US" };
        static HttpClient httpClient = new HttpClient();
        public async Task<string> GetSurgeConfig(string host, string url, string levels, int days)
        {
            var responseMessage = await httpClient.GetAsync(url);
            var headers = responseMessage.Headers.GetValues("Subscription-userinfo").FirstOrDefault() + ";";
            Dictionary<string, long> dict = GetSubscriptionInfo(headers);
            string config = await responseMessage.Content.ReadAsStringAsync();
            string pattern = @"link\/([\s\S]*?)\?";
            Match m = Regex.Match(url, pattern);
            string path = Environment.CurrentDirectory.ToString();
            path += "/" + m.Groups[1].Value + ".conf";
            using (StreamWriter writer = System.IO.File.AppendText(path))
            {
                writer.WriteLine(config);
            }
            QutumultX qutumultX = new QutumultX();
            qutumultX.version = "1.0";
            qutumultX.error = new Object();
            List<Servers> servers = new List<Servers>();
            servers.Add(new Servers()
            {
                tag = "Yahaha节点",
                online = true,
                description = "https://www.yahaha.us",
            });
            qutumultX.data = new Data()
            {
                provider_image_url = "https://www.yahaha.us/theme/material/images/users/favicon.png",
                provider_name = "Yahaha-LTD",
                account_description = levels,
                extension_check_interval = 3600,
                container_check_interval = 60,
                expire_time = (int)(DateTime.Now.AddDays(days).Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                upload = dict["upload"],
                download = dict["download"],
                total = dict["total"],
                configuration_profile_modified_at = (int)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                configuration_profile_url = $"https://{host}/api/yahaha/getconf?url={m.Groups[1].Value}",
                servers = servers,
                tips = "@宝可梦研究中心"
            };

            return JsonConvert.SerializeObject(qutumultX);
        }

        public async Task<string> InitConf(string url)
        {
            string path = Environment.CurrentDirectory.ToString();
            path += "/" + url + ".conf";
            string config = System.IO.File.ReadAllText(path);
            //string config = await httpClient.GetStringAsync(url);
            string proxys = GetProxys(config);
            string fullproxys = GetFullProxys(config);
            string shadowsocks = "";
            foreach (var proxyname in proxys.Split(','))
            {
                string ss = UpdateProxy(proxyname, fullproxys);
                shadowsocks += ss + "\n";
            }
            string templateConfi = System.IO.File.ReadAllText("yahaha-template.conf");
            templateConfi = templateConfi.Replace("@Nodes", proxys).Replace("@shadowsocks", shadowsocks);
            System.IO.File.Delete(path);
            return templateConfi;
        }

        private Dictionary<string, long> GetSubscriptionInfo(string headers)
        {
            string uploadPattern = @"upload=([\s\S]*?);";
            string downloadPattern = @"download=([\s\S]*?);";
            string totalPattern = @"total=([\s\S]*?);";
            Match uploadMatch = Regex.Match(headers, uploadPattern);
            long upload = Convert.ToInt64(uploadMatch.Groups[1].Value);
            Match downloadMatch = Regex.Match(headers, downloadPattern);
            long download = Convert.ToInt64(downloadMatch.Groups[1].Value);
            Match totalMatch = Regex.Match(headers, totalPattern);
            long total = Convert.ToInt64(totalMatch.Groups[1].Value);
            return new Dictionary<string, long>
            {
                { "upload",upload },
                { "download",download },
                { "total",total },
            };
        }
        private string GetProxys(string config)
        {
            string pattern = @"PROXY =([\s\S]*?)\n";
            Match m = Regex.Match(config, pattern);
            string proxys = m.Groups[1].ToString().Trim().Replace("select,", "").Replace("AUTO,", "");
            return proxys;
        }

        private string GetFullProxys(string config)
        {
            string pattern = @"\[Proxy\]\n([\s\S]*?)\[Proxy Group\]";
            Match m = Regex.Match(config, pattern);
            string proxys = m.Groups[1].ToString().Trim().Replace("DIRECT = direct", "").Replace(" ", "");
            return proxys;
        }

        private string UpdateProxy(string proxy, string fullproxy)
        {
            string pattern = proxy.Replace(" ", "").Replace("[Game]", @"\[Game\]") + @"=custom,([\s\S]*?)\n";
            Match m = Regex.Match(fullproxy + "\n", pattern);
            string[] ssconfig = m.Groups[1].ToString().Trim().Split(',');
            return $"shadowsocks={ssconfig[0]}:{ssconfig[1]}, method={ssconfig[2]}, password={ssconfig[3]}, {ssconfig[5]}, {ssconfig[6]}, {ssconfig[8]}, tag={proxy}";
        }
    }
}
