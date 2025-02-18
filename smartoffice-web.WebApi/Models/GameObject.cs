using System;
using System.ComponentModel.DataAnnotations;

namespace smartoffice_web.WebApi.Models
{
    public class GameObject
    {
        public Guid Id { get; set; }

        [Required]
        public Guid PrefabId { get; set; }

        [Required]
        public int PositionX { get; set; }

        [Required]
        public int PositionY { get; set; }

        [Required]
        public int ScaleX { get; set; }

        [Required]
        public int ScaleY { get; set; }

        [Required]
        public int RotationZ { get; set; }

        [Required]
        public int SortingLayer { get; set; }

        public Guid? WorldId { get; set; } // Nullable because `worldId` allows NULL in your schema
    }
}