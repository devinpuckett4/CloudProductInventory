using CloudProductInventory.Data;
using CloudProductInventory.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudProductInventory.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductsDAO productsDAO;

        public ProductController(ProductsDAO productsDAO)
        {
            this.productsDAO = productsDAO;
        }

        // READ - Show all products
        public IActionResult Index()
        {
            var products = productsDAO.GetAllProducts();

            return View(products);
        }

        // CREATE - Show the form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE - Save the new product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                productsDAO.AddProduct(product);

                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // EDIT - Show the current product information
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Product? product = productsDAO.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // EDIT - Save the changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                productsDAO.UpdateProduct(product);

                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // DELETE - Show the confirmation page
        [HttpGet]
        public IActionResult Delete(int id)
        {
            Product? product = productsDAO.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // DELETE - Remove the product
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            productsDAO.DeleteProduct(id);

            return RedirectToAction(nameof(Index));
        }
    }
}