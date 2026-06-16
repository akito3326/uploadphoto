using Microsoft.AspNetCore.Mvc;
using uploadphoto.Data;
using uploadphoto.Models;

namespace uploadphoto.Controllers
{
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Product
        public IActionResult Index()
        {
            var products = ProductRepository.GetAll();
            return View(products);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            return View(new ProductViewModel());
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            // Validate file formats
            ValidateUploadedFiles(model.ImageFiles);

            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = model.Name,
                    Price = model.Price,
                    ImageUrls = new List<string>()
                };

                // Handle file upload
                if (model.ImageFiles != null && model.ImageFiles.Count > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    foreach (var file in model.ImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            product.ImageUrls.Add("/uploads/" + uniqueFileName);
                        }
                    }
                }

                ProductRepository.Add(product);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Product/Edit/5
        public IActionResult Edit(int id)
        {
            var product = ProductRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ExistingImageUrls = product.ImageUrls
            };

            return View(model);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var product = ProductRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            // Validate file formats
            ValidateUploadedFiles(model.ImageFiles);

            if (ModelState.IsValid)
            {
                product.Name = model.Name;
                product.Price = model.Price;

                // Handle file upload
                if (model.ImageFiles != null && model.ImageFiles.Count > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    foreach (var file in model.ImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            product.ImageUrls.Add("/uploads/" + uniqueFileName);
                        }
                    }
                }

                ProductRepository.Update(product);
                return RedirectToAction(nameof(Index));
            }

            model.ExistingImageUrls = product.ImageUrls;
            return View(model);
        }

        // POST: Product/DeleteImage
        [HttpPost]
        public IActionResult DeleteImage(int productId, string imageUrl)
        {
            var product = ProductRepository.GetById(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            if (product.ImageUrls.Contains(imageUrl))
            {
                product.ImageUrls.Remove(imageUrl);

                // Optionally, delete the physical file as well
                string physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
                if (System.IO.File.Exists(physicalPath))
                {
                    try
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                    catch
                    {
                        // Ignore deletion errors
                    }
                }

                ProductRepository.Update(product);
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Image not found on product" });
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = ProductRepository.GetById(id);
            if (product != null)
            {
                // Delete physical images first
                foreach (var imageUrl in product.ImageUrls)
                {
                    string physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(physicalPath))
                    {
                        try
                        {
                            System.IO.File.Delete(physicalPath);
                        }
                        catch
                        {
                            // Ignore deletion errors
                        }
                    }
                }

                ProductRepository.Delete(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private void ValidateUploadedFiles(List<IFormFile>? files)
        {
            if (files != null && files.Count > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                foreach (var file in files)
                {
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("ImageFiles", $"Tập tin '{file.FileName}' không đúng định dạng. Chỉ hỗ trợ .jpg và .png.");
                    }
                }
            }
        }
    }
}
