using AbpBase.Application;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectMapping;

namespace ApbBase.HttpApi.Controllers
{
    [ApiController]
    public class TestController : AbpController
    {
        // 
        private readonly IObjectMapper<AbpBaseApplicationModule> _mapper;
        public TestController(IObjectMapper<AbpBaseApplicationModule> mapper)
        {
        }

        [HttpGet("/T")]
        public string MyWebApi()
        {
            return "应用启动成功！";
        }

        public class TestModel
        {
            [Required]
            public int Id { get; set; }

            [MaxLength(11)]
            public int Iphone { get; set; }

            [Required]
            [MinLength(5)]
            public string Message { get; set; }
        }


        [HttpPost("/T2")]
        public string MyWebApi2([FromBody] TestModel model)
        {
            return "请求完成";
        }

        [HttpGet("/T4")]
        public string MyWebApi4()
        {
            int a = 1;
            int b = 0;
            int c = a / b;
            return c.ToString();
        }
    }
}
