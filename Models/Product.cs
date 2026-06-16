using System.ComponentModel.DataAnnotations;

namespace uploadphoto.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Display(Name = "Danh sách hình ảnh")]
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
