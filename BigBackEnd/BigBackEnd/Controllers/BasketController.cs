﻿using BigBackEnd.DataAccessLayer;
using BigBackEnd.Models;
using BigBackEnd.ViewModels.BasketViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;

namespace BigBackEnd.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            string cookie = HttpContext.Request.Cookies["basket"];
            List<BasketVM> basketVMs= null;

            if (!string.IsNullOrEmpty(cookie))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);

                foreach (BasketVM basketVM in basketVMs)
                {
                    Product product = await _context.Products.FirstOrDefaultAsync(p => p.isDeleted == false && p.Id == basketVM.Id);

                    if (product != null)
                    {
                        basketVM.Title = product.Title;
                        basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                        basketVM.Image = product.MainImage;
                        basketVM.ExTax = product.ExTax;
                    }
                }
            }
            

            return View(basketVMs);
        }

        public async Task<IActionResult> RemoveBasket(int? Id)
        {
            if (Id == null) return BadRequest();
            string cookie = HttpContext.Request.Cookies["basket"];
            if (cookie == null) return BadRequest();
            List<BasketVM> basketVMs = null;
            if (!string.IsNullOrWhiteSpace(cookie))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
                if (basketVMs.Exists(p => p.Id == Id))
                {
                    basketVMs.RemoveAll(p=>p.Id == Id);
                }
                cookie = JsonConvert.SerializeObject(basketVMs);
                HttpContext.Response.Cookies.Append("basket", cookie);

                foreach (BasketVM basketVM in basketVMs)
                {
                    Product product = await _context.Products.FirstOrDefaultAsync(p => p.isDeleted == false && p.Id == basketVM.Id);
                    if (product != null)
                    {
                        basketVM.Title = product.Title;
                        basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                        basketVM.Image = product.MainImage;
                        basketVM.ExTax = product.ExTax;
                    }

                }

                
            }

            return PartialView("_BasketCartPartial", basketVMs);
        }

        public async Task<IActionResult> MainBasket()
        {
            string cookie = HttpContext.Request.Cookies["basket"];
            List<BasketVM> basketVMs = null;

            if (!string.IsNullOrEmpty(cookie))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);

                foreach (BasketVM basketVM in basketVMs)
                {
                    Product product = await _context.Products.FirstOrDefaultAsync(p => p.isDeleted == false && p.Id == basketVM.Id);

                    if (product != null)
                    {
                        basketVM.Title = product.Title;
                        basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                        basketVM.Image = product.MainImage;
                        basketVM.ExTax = product.ExTax;
                    }
                }
            }

            return PartialView("_BasketIndexPartial", basketVMs);
        }
        public async Task<IActionResult> AddBasket(int? Id)
        {
            if (Id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.isDeleted == false && p.Id == Id)) return NotFound();

            //Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == Id && p.isDeleted == false);

            //if (product == null) return NotFound();

            string cookie = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;


            if (string.IsNullOrWhiteSpace(cookie))
            {
                basketVMs = new List<BasketVM> 
                {
                    new BasketVM {Id = (int)Id, Count= 1}
                };

                
            }
            else
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
                if (basketVMs.Exists(p=>p.Id == Id))
                {
                    basketVMs.Find(b => b.Id == Id).Count += 1;
                }
                else
                {
                    basketVMs.Add(new BasketVM { Id = (int)Id,Count = 1});
                };

            }

            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.Baskets.Where(b => b.isDeleted == false))
                    .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

                if (appUser.Baskets != null && appUser.Baskets.Count() > 0)
                {
                    if (appUser.Baskets.Any(b=>b.ProductId == Id))
                    {
                        appUser.Baskets.FirstOrDefault(b => b.ProductId == Id).Count = basketVMs.FirstOrDefault(b => b.Id == Id).Count;
                    }
                    else
                    {
                        Basket basket = new Basket
                        {
                            ProductId = Id,
                            Count = 1
                        };
                        appUser.Baskets.Add(basket);

                    }
                }
                else
                {
                    Basket basket = new Basket
                    {
                        ProductId = Id,
                        Count = 1
                    };

                    appUser.Baskets.Add(basket);
                }

                await _context.SaveChangesAsync();
            }


            cookie = JsonConvert.SerializeObject(basketVMs);
            HttpContext.Response.Cookies.Append("basket", cookie);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.isDeleted == false && p.Id == basketVM.Id);
                if (product != null)
                {
                    basketVM.Title = product.Title;
                    basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                    basketVM.Image = product.MainImage;
                    basketVM.ExTax = product.ExTax;
                }

            }

            return PartialView("_BasketCartPartial",basketVMs);
        }

        public async Task<IActionResult> GetBasket()
        {
            string basket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

            return Json(basketVMs);
        }

        public async Task<IActionResult> RefreshBasketMain()
        {
            string cookie = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;


            if (string.IsNullOrWhiteSpace(cookie))
            {
                return BadRequest();
            }

            basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.isDeleted == false && p.Id == basketVM.Id);
                if (product != null)
                {
                    basketVM.Title = product.Title;
                    basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                    basketVM.Image = product.MainImage;
                    basketVM.ExTax = product.ExTax;
                }

            }

            return PartialView("_BasketIndexPartial", basketVMs);
        }

        public async Task<IActionResult> DecreaseBasket(int? Id)
        {
            if (Id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.isDeleted == false && p.Id == Id)) return NotFound();

            //Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == Id && p.isDeleted == false);

            //if (product == null) return NotFound();

            string cookie = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;


            if (string.IsNullOrWhiteSpace(cookie))
            {
                return BadRequest();
            }
            else
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
                if (basketVMs.Exists(p => p.Id == Id) && basketVMs.Find(b => b.Id == Id).Count > 1)
                {
                    basketVMs.Find(b => b.Id == Id).Count -= 1;
                }
                else if(basketVMs.Exists(p => p.Id == Id))
                {
                    basketVMs.RemoveAll(p => p.Id == Id);
                }

            }
            cookie = JsonConvert.SerializeObject(basketVMs);
            HttpContext.Response.Cookies.Append("basket", cookie);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.isDeleted == false && p.Id == basketVM.Id);
                if (product != null)
                {
                    basketVM.Title = product.Title;
                    basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                    basketVM.Image = product.MainImage;
                    basketVM.ExTax = product.ExTax;
                }

            }

            return PartialView("_BasketCartPartial", basketVMs);
        }
    }
}
