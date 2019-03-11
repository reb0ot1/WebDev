using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CakesWebApp.Data;
using MvcFramework;
using MvcFramework.Services;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace CakesWebApp.Controllers
{
    public abstract class BaseController : Controller
    {
        
        protected BaseController()
        {
            this.Db = new CakesDbContext();
        }

        protected CakesDbContext Db;

    }
}
