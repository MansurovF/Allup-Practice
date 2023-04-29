﻿using System.ComponentModel.DataAnnotations;

namespace Pustok_BackProject.Models
{
    public class Author:BaseEntity
    {
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]

        public string Surname { get; set; }
    }
}