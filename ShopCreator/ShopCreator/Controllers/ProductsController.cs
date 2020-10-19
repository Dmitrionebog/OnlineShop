using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopCreator.BL;
using ShopCreator.Models;
using ShopCreator.Models.DTOs;
using ShopCreator.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Authorization;

namespace ShopCreator.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly DataContext _context;
        private readonly IHostingEnvironment _env;

        public ProductsController(DataContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Products
        
        public async Task<IActionResult> Index(int? id)
        {
            List<Product> products = new List<Product>();
            List<Category_Id_and_Name_and_HasChilds_and_CheckedId> CategoriesOfOneSelect = new List<Category_Id_and_Name_and_HasChilds_and_CheckedId>();
            List<List<Category_Id_and_Name_and_HasChilds_and_CheckedId>> CategoriesOfAllSelects = new List<List<Category_Id_and_Name_and_HasChilds_and_CheckedId>>();
            List<int> childCategoriesIds = new List<int>();
            if (id == null)
            {
                CategoriesOfOneSelect = await _context.Categories.Where(category => category.ParentCategoryId == null).Select(cat => new Category_Id_and_Name_and_HasChilds_and_CheckedId
                {
                    Id = cat.Id,
                    Name = cat.Name,
                    HasChilds = (cat.ChildCategories.Count() != 0) ? true : false

                }).ToListAsync();
                CategoriesOfAllSelects.Add(CategoriesOfOneSelect);
                products = await _context.Products.Include(p => p.Image).ToListAsync();
            }
            else
            {
                //получаем категории последнего select(дети выбранного id)
                CategoriesOfOneSelect = await _context.Categories.Where(category => category.ParentCategoryId == id).Select(cat => new Category_Id_and_Name_and_HasChilds_and_CheckedId
                {
                    Id = cat.Id,
                    Name = cat.Name,
                    HasChilds = (cat.ChildCategories.Count() != 0) ? true : false

                }).ToListAsync();


                if (CategoriesOfOneSelect.Count() != 0)
                {
                    CategoriesOfAllSelects.Add(CategoriesOfOneSelect);
                    var commandText = @"WITH sub_tree  AS  ( 
                        SELECT id AS Id 
                        FROM dbo.Categories 
                        WHERE Id = {0} 
                        UNION ALL 
                        SELECT cat.id 
                        FROM dbo.Categories cat, sub_tree st  
                        WHERE cat.ParentCategoryId = st.Id ) 
                        SELECT * FROM sub_tree; ";
                    childCategoriesIds = _context.Categories.FromSqlRaw(commandText , id).Select(c => c.Id).ToList();
                }

                //получаем соседние категории и родительские selects
                var oneCategory = await _context.Categories.Where(cat => cat.Id == id).FirstOrDefaultAsync();
                while (oneCategory != null)
                {
                    CategoriesOfOneSelect = await _context.Categories.Where(category => category.ParentCategoryId == oneCategory.ParentCategoryId).Select(cat => new Category_Id_and_Name_and_HasChilds_and_CheckedId
                    {
                        Id = cat.Id,
                        Name = cat.Name,
                        HasChilds = (cat.ChildCategories.Count() != 0) ? true : false,
                        CheckedId = oneCategory.Id

                    }).ToListAsync();
                    CategoriesOfAllSelects.Add(CategoriesOfOneSelect);
                    oneCategory = await _context.Categories.Where(cat => cat.Id == oneCategory.ParentCategoryId).FirstOrDefaultAsync();
                }
                CategoriesOfAllSelects.Reverse();

                if (childCategoriesIds.Count() == 0)
                {
                    var productsIds = await _context.ProductCategories.Where(pc => pc.CategoryId == id).Select(pc => pc.ProductId).ToListAsync();
                    foreach (var productId in productsIds)
                    {
                        var product = await _context.Products.Where(pr => pr.Id == productId).FirstOrDefaultAsync();
                        products.Add(product);
                    }
                }
                else
                {
                    var productsIds = await _context.ProductCategories.Where(pc => childCategoriesIds.Contains(pc.CategoryId)).Select(pc => pc.ProductId).ToListAsync();
                    foreach (var productId in productsIds)
                    {
                        var product = await _context.Products.Where(pr => pr.Id == productId).FirstOrDefaultAsync();
                        products.Add(product);
                    }
                }
            }
            
            ViewBag.Categories = CategoriesOfAllSelects;
            return View(products);
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();  
            }

            var product = await _context.Products.Include(p => p.Image).Include(p => p.ProductProperties)
                .ThenInclude(prodProp => prodProp.Property)
                .Include(c => c.ProductCategories)
                .ThenInclude(prodCat => prodCat.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            //var p = Enum.GetNames(typeof(Models.PropertyType));
            //ViewBag.testenum = p;

            

            var propertiesDTOs = _context.Properties.Select(prop => new Property_Id_and_Name_and_Type_and_Values
            {
                Id = prop.Id,
                Name = prop.Name,
                Type = (int)prop.PropertyType,
                ValidValues = prop.ValidValues

            });

            ViewBag.Properties = propertiesDTOs;
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,PropertiesOfProduct,IdsOfCategories")] ProductViewModel product, IFormFile uploadedFile)
        {

            var propertiesDTOs = _context.Properties.Select(prop => new Property_Id_and_Name_and_Type_and_Values
            {
                Id = prop.Id,
                Name = prop.Name,
                Type = (int)prop.PropertyType,
                ValidValues = prop.ValidValues
            });

            ViewBag.Properties = propertiesDTOs;

            if (ModelState.IsValid)
            {

                var guid = Guid.NewGuid().ToString();
                var dbProduct = new Product();
                if (uploadedFile != null)
                {
                    FileInfo fi = new FileInfo(uploadedFile.FileName);
                    string ext = fi.Extension;
                    // путь к папке Files
                    string path = "/Files/" + guid + ext;
                    // сохраняем файл в папку Files в каталоге wwwroot
                    using (var fileStream = new FileStream(_env.WebRootPath + path, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                        fileStream.Close();
                    }
                    Bitmap myBitmap = new Bitmap(_env.WebRootPath + path);
                    Image myThumbnail = myBitmap.GetThumbnailImage(
                    64, 36, null, IntPtr.Zero);

                    string path2 = "/Files/" + guid + ".min.png";
                    // сохраняем файл2 в папку Files в каталоге wwwroot

                    using (var fileStream2 = new FileStream(_env.WebRootPath + path2, FileMode.Create))
                    {
                        myThumbnail.Save(fileStream2, ImageFormat.Png);
                        myThumbnail.Dispose();
                        myBitmap.Dispose();
                        await fileStream2.FlushAsync();
                        fileStream2.Close();
                    }

                    FileModel file = new FileModel { Name = guid, Path = path,MinPath = path };

                    dbProduct.Image = file;
                }

                Converters.ConvertToDBProduct(product,dbProduct);

                _context.Add(dbProduct);

                await _context.SaveChangesAsync();

                //dbProduct.Image.Name = dbProduct.Image.Id + dbProduct.Image.Name;
                //await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            


            return View(product);
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Image)
                .Include(p => p.ProductCategories)
                  .Include(p => p.ProductProperties)
                  .ThenInclude(prodProp => prodProp.Property)

                  .FirstOrDefaultAsync(m => m.Id == id);

            var propertiesDTOs = _context.Properties.Select(prop => new Property_Id_and_Name_and_Type_and_Values
            {
                Id = prop.Id,
                Name = prop.Name,
                Type = (int)prop.PropertyType,
                ValidValues = prop.ValidValues
            });

            ViewBag.Properties = propertiesDTOs;

            //int[][] IdsCategories = new int[product.ProductCategories.Count][];
            //for (int index = 0; index < product.ProductCategories.Count; index++)
            //{
            //    var oneCategoryId = product.ProductCategories[index].CategoryId;
            //    var OneCategory = await _context.Categories.FirstOrDefaultAsync(cat => cat.Id == oneCategoryId);
            //    List<int> oneChain = new List<int>();
            //    oneChain.Add(oneCategoryId);
            //    while (OneCategory.ParentCategoryId != null)
            //    {
            //        OneCategory = await _context.Categories.FirstOrDefaultAsync(cat => cat.Id == OneCategory.ParentCategoryId);
            //        oneChain.Add(OneCategory.Id);
            //    }
            //    var oneChainArray = oneChain.ToArray();
            //    IdsCategories[index] = oneChainArray;
            //}

            //ViewBag.IdsOfSelectedCategories = IdsCategories;

            int[][] allIdsCategories = new int[product.ProductCategories.Count][];
            //bool [][][] allChainshasChilds = new bool[product.ProductCategories.Count][][];
            Category_Id_and_Name_and_HasChilds_and_CheckedId[][][] allChainsOfSelects = new Category_Id_and_Name_and_HasChilds_and_CheckedId[product.ProductCategories.Count][][];
            for (int index = 0; index < product.ProductCategories.Count; index++)
            {
                var oneCategoryId = product.ProductCategories[index].CategoryId;
                var oneCategory = await _context.Categories.FirstOrDefaultAsync(cat => cat.Id == oneCategoryId);
                var allOptionsOfOneSelect = await _context.Categories
                    .Where(cat => cat.ParentCategoryId == oneCategory.ParentCategoryId)
                    .Include(c => c.ChildCategories)
                    .Select(cat => new Category_Id_and_Name_and_HasChilds_and_CheckedId { Id = cat.Id, Name = cat.Name, HasChilds = (cat.ChildCategories.Count() != 0) ? true : false })
                    .ToArrayAsync();
                List<Category_Id_and_Name_and_HasChilds_and_CheckedId[]> allOptionsOfOneChain = new List<Category_Id_and_Name_and_HasChilds_and_CheckedId[]>();

                List<int> oneChain = new List<int>();
                oneChain.Add(oneCategoryId);
                allOptionsOfOneChain.Add(allOptionsOfOneSelect);

                while (oneCategory.ParentCategoryId != null)
                {
                    oneCategory = await _context.Categories.FirstOrDefaultAsync(cat => cat.Id == oneCategory.ParentCategoryId);
                    allOptionsOfOneSelect = await _context.Categories
                        .Where(cat => cat.ParentCategoryId == oneCategory.ParentCategoryId)
                        .Include(c => c.ChildCategories)
                        .Select(cat => new Category_Id_and_Name_and_HasChilds_and_CheckedId  { Id = cat.Id, Name = cat.Name, HasChilds = (cat.ChildCategories.Count() != 0) ? true : false })
                        .ToArrayAsync();
                    oneChain.Add(oneCategory.Id);
                    allOptionsOfOneChain.Add(allOptionsOfOneSelect);
                }
                var oneChainArray = oneChain.ToArray();
                Array.Reverse(oneChainArray);
                var oneChainAllOptionsArray = allOptionsOfOneChain.ToArray();
                Array.Reverse(oneChainAllOptionsArray);
                allIdsCategories[index] = oneChainArray;
                allChainsOfSelects[index] = oneChainAllOptionsArray;
            } 

            ViewBag.IdsOfSelectedCategories = allIdsCategories;
            ViewBag.AllChainSelects = allChainsOfSelects;

            //Category[][] AllChainSelects = new Category[product.ProductCategories.Count][];
            //for (int index = 0; index < product.ProductCategories.Count; index++)
            //{

            //}


            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }


            // POST: Products/Edit/5
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,PropertiesOfProduct,IdsOfCategories")] ProductViewModel product, IFormFile uploadedFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
                try
                {
                    var dbProduct = await _context.Products
                        .Include(p => p.Image)
                        .Include(p => p.ProductCategories)
                        .Include(p => p.ProductProperties)
                        .ThenInclude(prodProp => prodProp.Property)
                        .FirstOrDefaultAsync(pr => pr.Id == product.Id);

                    //удаление старых картинок из папуи Files и базы
                    if (dbProduct.Image != null)
                    {

                        string path = dbProduct.Image.Path;
                        FileInfo fileInf = new FileInfo(_env.WebRootPath + path);
                        if (fileInf.Exists)
                        {
                            fileInf.Delete();
                        }

                        string path2 = dbProduct.Image.MinPath;
                        FileInfo fileInf2 = new FileInfo(_env.WebRootPath + path2);
                        if (fileInf2.Exists)
                        {
                            fileInf2.Delete();
                        }

                        _context.Files.Remove(dbProduct.Image);
                    }

                    var guid = Guid.NewGuid().ToString();

                    if (uploadedFile != null)
                    {
                        FileInfo fi = new FileInfo(uploadedFile.FileName);
                        string ext = fi.Extension;
                        // путь к папке Files
                        string path = "/Files/" + guid + ext;
                        // сохраняем файл в папку Files в каталоге wwwroot
                        using (var fileStream = new FileStream(_env.WebRootPath + path, FileMode.Create))
                        {
                            await uploadedFile.CopyToAsync(fileStream);
                        }

                        Bitmap myBitmap = new Bitmap(_env.WebRootPath + path);
                        Image myThumbnail = myBitmap.GetThumbnailImage(
                        64, 36, null, IntPtr.Zero);

                        string path2 = "/Files/" + guid + ".min.png";
                        // сохраняем файл2 в папку Files в каталоге wwwroot

                        using (var fileStream2 = new FileStream(_env.WebRootPath + path2, FileMode.Create))
                        {
                            myThumbnail.Save(fileStream2, ImageFormat.Png);
                            myThumbnail.Dispose();
                            myBitmap.Dispose();
                        }
                        FileModel file = new FileModel { Name = guid, Path = path,MinPath = path2};


                        dbProduct.Image = file;
                    }

                        Converters.ConvertToDBProduct(product, dbProduct);
                    _context.Update(dbProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(p => p.Image)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var product = await _context.Products.Include(p => p.Image).FirstOrDefaultAsync(m => m.Id == id);

            if (product.Image != null)
            {

                string path = product.Image.Path;
                FileInfo fileInf = new FileInfo(_env.WebRootPath + path);
                if (fileInf.Exists)
                {
                    fileInf.Delete();
                }

                string path2 = product.Image.MinPath;
                FileInfo fileInf2 = new FileInfo(_env.WebRootPath + path2);
                if (fileInf2.Exists)
                {
                    fileInf2.Delete();
                }

                _context.Files.Remove(product.Image);
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

           

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
