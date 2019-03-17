﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CakesWebApp.Extensions;
using CakesWebApp.Models;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace CakesWebApp.Controllers
{
    public class CakesController : BaseController
    {
        public IHttpResponse AddCakes()
        {
            return this.View("AddCakes");
        }

        public IHttpResponse DoAddCakes()
        {
            var name = this.Request.FormData["name"].ToString().Trim().UrlDecode();
            var price = decimal.Parse(this.Request.FormData["price"].ToString().UrlDecode());
            var picture = this.Request.FormData["picture"].ToString().Trim().UrlDecode();

            // TODO: Validation

            var product = new Product
            {
                Name = name,
                Price = price,
                Url = picture
            };
            this.Db.Products.Add(product);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception e)
            {
                // TODO: Log error
                return this.ServerError(e.Message);
            }

            // Redirect
            return this.Redirect("/");
        }

        public IHttpResponse ById()
        {
            var id = int.Parse(this.Request.QueryData["id"].ToString());
            var product = this.Db.Products.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return this.BadRequestError("Cake not found.");
            }

            var viewBag = new Dictionary<string, string>
            {
                {"Name", product.Name},
                {"Price", product.Price.ToString(CultureInfo.InvariantCulture)},
                {"ImageUrl", product.Url}
            };
            return this.View("CakeById", viewBag);
        }
    }
}