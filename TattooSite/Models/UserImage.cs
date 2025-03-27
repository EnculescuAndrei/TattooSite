using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TattooSite.Models
{
    public class UserImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // Foreign key to the User table

        [Required]
        public byte[] ImageData { get; set; } // Store image as byte array

        public string ImageType { get; set; } // Store the image type (e.g., "image/jpeg")

        public string ImageName { get; set; } // Store the original image name

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    }
}