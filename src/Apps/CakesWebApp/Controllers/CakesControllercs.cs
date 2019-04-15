using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CakesWebApp.Extensions;
using CakesWebApp.Models;
using CakesWebApp.ViewModels.Cakes;
using MvcFramework;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace CakesWebApp.Controllers
{
    public class CakesController : BaseController
    {
        [HttpGet("/cakes/add")]
        public IHttpResponse AddCakes()
        {
            return this.View("AddCakes");
        }

        [HttpPost("/cakes/add")]
        public IHttpResponse DoAddCakes(CakeVM model)
        {
            //var name = this.Request.FormData["name"].ToString().Trim().UrlDecode();
            //var price = decimal.Parse(this.Request.FormData["price"].ToString().UrlDecode());
            //var picture = this.Request.FormData["picture"].ToString().Trim().UrlDecode();

            // TODO: Validation

            var product = new Product
            {
                Name = model.Name,
                Price = model.Price,
                Url = model.Picture.Trim().UrlDecode()
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

        [HttpGet("/cakes/view")]
        public IHttpResponse ById()
        {
            var id = int.Parse(this.Request.QueryData["id"].ToString());
            var product = this.Db.Products.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return this.BadRequestError("Cake not found.");
            }

            var viewModel = new ByIdViewModel
            {
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.Url

            };

            return this.View("CakeById", viewModel);
        }

        [HttpGet("/cakes/search")]
        public IHttpResponse Search(string searchText)
        {
            //var searchText = "cake";
            ByIdViewModel[] cakes = this.Db.Products.Where(x => x.Name.Contains(searchText))
                .Select(x => new ByIdViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    ImageUrl = x.Url
                }).ToArray();

            var searchModel = new SearchViewModel
            {
                Cakes = cakes,
                SearchText = searchText
            };

            return this.View("Search", searchModel);
        }
        
    }

}