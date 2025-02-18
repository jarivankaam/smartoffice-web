using System;
using System.ComponentModel.DataAnnotations;

namespace smartoffice_web.WebApi.Models
{
    public class World
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int MaxHeight { get; set; }

        [Required]
        public int MaxWidth { get; set; }
    }
}