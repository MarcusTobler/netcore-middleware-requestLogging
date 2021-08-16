using System;
using Lab.Middleware.WebAPI.Architectures.Logging.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Middleware.WebAPI.Controllers
{
    [ApiController]
    [Route("teste")]
    public class TesteController : ControllerBase
    {
        public class Filtro
        {
            public string Nome { get; set; }
        }
        public TesteController()
        {
            
        }

        [HttpGet]
        [LoggingRequest(AttributeLogging.Request)]
        public IActionResult Get()
        {
            try
            {
                return Ok(new
                {
                    Versao = 1.0,
                    Result = "Check"
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    Versao = 1.0,
                    Result = "No Check"
                });
            }
        }
        
        [HttpGet]
        [Route("error")]
        [LoggingRequest(AttributeLogging.All)]
        public IActionResult Error()
        {
            try
            {
                return Ok(new
                {
                    Versao = 1.0,
                    Result = Divide(2, 0)
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private int Divide(int a, int b)
        {
            return (a / b);
        }
    }
}