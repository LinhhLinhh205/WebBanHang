﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHang.Areas.Customer.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string  Name { get; set; }
        public int TotalProduct { get; set; }
    }
}
