﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;
using System.IO;

namespace WebBanHang.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext _db;
        private IWebHostEnvironment _hosting;
        public ProductController(ApplicationDbContext db , IWebHostEnvironment hosting)
        {
            _db = db;
            _hosting=hosting;
        }
        public IActionResult Index()
        {
            var dsProduct = _db.Products.Include(x =>x.Category).ToList();
            return View(dsProduct);
        }
        public IActionResult Delete(int id)
        {
            var sp = _db.Products.Find(id);
            return View(sp);
        }
        public IActionResult DeleteConfirmed(int id)
        {
            var sp = _db.Products.Find(id);
            _db.Products.Remove(sp);
            _db.SaveChanges();
            TempData["success"] = "Product deleted success";            
            return RedirectToAction("Index");
        }   
        public IActionResult Add()
        {
            ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View();
        }
        [HttpPost]
        public IActionResult Add(Product product, IFormFile ImageUrl)
        {
                if (ImageUrl != null)
                {               
                    product.ImageUrl = SaveImage(ImageUrl);
                }               
                _db.Products.Add(product);
                _db.SaveChanges();
                TempData["success"] = "Product inserted success";
                return RedirectToAction("Index");                      
        }
        private string SaveImage(IFormFile image)
        {
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var path = Path.Combine(_hosting.WebRootPath, @"images/products");
            var saveFile = Path.Combine(path, filename);
            using (var filestream = new FileStream(saveFile, FileMode.Create))
            {
                image.CopyTo(filestream);
            }
            return @"images/products/" + filename;
        }
        public IActionResult Update(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            //truyền danh sách thể loại cho View để sinh ra điều khiển DropDownList
            ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
            {

                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View(product);
        }
        //Xử lý cập nhật sản phẩm
        [HttpPost]
        public IActionResult Update(Product product, IFormFile ImageUrl)
        {
            if (ModelState.IsValid) //kiem tra hop le
            {
                var existingProduct = _db.Products.Find(product.Id);
                if (ImageUrl != null)
                {
                    //xu ly upload và lưu ảnh đại diện mới
                    product.ImageUrl = SaveImage(ImageUrl);
                    //xóa ảnh cũ (nếu có)
                    if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                    {
                        var oldFilePath = Path.Combine(_hosting.WebRootPath, existingProduct.ImageUrl);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                }
                else
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                //cập nhật product vào table Product
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;
                _db.SaveChanges();
                TempData["success"] = "Product updated success";
                return RedirectToAction("Index");
            }
            ViewBag.CategoryList = _db.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View();
        }
    }
}
