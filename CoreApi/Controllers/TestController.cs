﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreApi.Entities;

namespace CoreApi.Controllers
{
    [Route("api/[controller]")]
    public class TestController:Controller
    {
        private MyContext _context;

        public TestController(MyContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get() {
            return Ok();
        }

    }
}
