using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UTB.Eshop.Domain.Abstraction;
using UTB.Eshop.Web.Models.Database;
using UTB.Eshop.Web.Models.Entities;

namespace UTB.Eshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CarouselController : Controller
    {
        readonly EshopDbContext _eshopDbContext;
        IFileUpload _fileUpload;
        ICheckFileContent _checkFileContent;
        ICheckFileLength _checkFileLength;
        public CarouselController(EshopDbContext eshopDbContext,
                                    IFileUpload fileUpload,
                                    ICheckFileContent checkFileContent,
                                    ICheckFileLength checkFileLength)
        {
            _eshopDbContext = eshopDbContext;
            _fileUpload = fileUpload;
            _checkFileContent = checkFileContent;
            _checkFileLength = checkFileLength;
        }

        public IActionResult Select()
        {
            List<CarouselItem> carouselItems = _eshopDbContext.CarouselItems.ToList();
            return View(carouselItems);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarouselItem carouselItemFromForm)
        {
            if (carouselItemFromForm.Image != null)
            { 
                if(_checkFileContent.CheckFileContent(carouselItemFromForm.Image, "image")
                    && _checkFileLength.CheckFileLength(carouselItemFromForm.Image, 4_000_000))
                {
                    //<><>*
                    _fileUpload.ContentType = "image";
                    _fileUpload.FileLength = 4_000_000;
                    //*<><>
                    carouselItemFromForm.ImageSrc = await _fileUpload.FileUploadAsync(carouselItemFromForm.Image, Path.Combine("img", "carousel"));

                    _eshopDbContext.CarouselItems.Add(carouselItemFromForm);
                    _eshopDbContext.SaveChanges();
                    return RedirectToAction(nameof(Select));
                }
            }

            return View(carouselItemFromForm);
        }

        public IActionResult Edit(int ID)
        {
            CarouselItem carouselItem = _eshopDbContext.CarouselItems.FirstOrDefault(carItem => carItem.ID == ID);

            if (carouselItem != null)
            {
                return View(carouselItem);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CarouselItem carouselItemFromForm)
        {
            CarouselItem carouselItem = _eshopDbContext.CarouselItems.FirstOrDefault(carItem => carItem.ID == carouselItemFromForm.ID);

            if (carouselItem != null)
            {
                if (carouselItemFromForm.Image != null)
                {
                    if (_checkFileContent.CheckFileContent(carouselItemFromForm.Image, "image")
                        && _checkFileLength.CheckFileLength(carouselItemFromForm.Image, 4_000_000))
                    {
                        //<><>*
                        _fileUpload.ContentType = "image";
                        _fileUpload.FileLength = 4_000_000;
                        //*<><>
                        carouselItemFromForm.ImageSrc = await _fileUpload.FileUploadAsync(carouselItemFromForm.Image, Path.Combine("img", "carousel"));

                        if (String.IsNullOrEmpty(carouselItemFromForm.ImageSrc) == false)
                        {
                            carouselItem.ImageSrc = carouselItemFromForm.ImageSrc;
                        }
                        else
                            return View(carouselItemFromForm);
                    }
                    else
                        return View(carouselItemFromForm);
                }
                
                carouselItem.ImageAlt = carouselItemFromForm.ImageAlt;

                _eshopDbContext.SaveChanges();

                return RedirectToAction(nameof(Select));
            }

            return NotFound();
        }


        public IActionResult Delete(int ID)
        {
            CarouselItem carouselItem = _eshopDbContext.CarouselItems.FirstOrDefault(carItem => carItem.ID == ID);

            if (carouselItem != null)
            {
                _eshopDbContext.CarouselItems.Remove(carouselItem);

                _eshopDbContext.SaveChanges();

                return RedirectToAction(nameof(Select));
            }

            return NotFound();
        }
    }
}
