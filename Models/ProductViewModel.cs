using System.ComponentModel.DataAnnotations;

namespace uploadphoto.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Display(Name = "Tải lên hình ảnh (chọn 1 hoặc nhiều ảnh jpg/png)")]
        public List<IFormFile>? ImageFiles { get; set; }

        public List<string> ExistingImageUrls { get; set; } = new List<string>();
    }
}
