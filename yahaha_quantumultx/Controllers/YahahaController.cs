using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace yahaha_quantumultx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YahahaController : ControllerBase
    {
        /// <summary>
        /// 获取quantumultx配置
        /// </summary>
        /// <param name="url">surge订阅地址</param>
        /// <param name="days">账号有效期剩余天数</param>
        /// <returns></returns>
        [HttpGet("quantumultx")]
        public async Task<string> quantumultx(string url, string levels, int days)
        {
            SurgeHelper surgeHelper = new SurgeHelper();
            string result = await surgeHelper.GetSurgeConfig(Request.Host.Value, url, levels, days);
            return $"{result}";
        }

        [HttpGet("GetConf")]
        public async Task<string> GetConf(string url)
        {
            SurgeHelper surgeHelper = new SurgeHelper();
            return await surgeHelper.InitConf(url);
        }
    }
}