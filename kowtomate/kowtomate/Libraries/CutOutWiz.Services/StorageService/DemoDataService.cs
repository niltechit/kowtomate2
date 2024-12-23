﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.StorageService
{
    public class DemoDataService
    {
        private Data.Response<List<Data.Node>> GetDemoFolders()
        {
            var response = new Data.Response<List<Data.Node>>();

            var images = new List<Data.Node>();

            images.Add(new Data.Node("Brand 1", "testimages.com", false));
            images.Add(new Data.Node("Brand 2", "testimages.com", false));
            images.Add(new Data.Node("Brand 3", "testimages.com", false));

            response.Result = images;
            return response;
        }

        private Data.Response<List<Data.DriveImage>> GetDemoImages()
        {
            var response = new Data.Response<List<Data.DriveImage>>();

            var images = new List<Data.DriveImage>();

            var image = new Data.DriveImage("Image 1", "testimages1.com", "https://static-01.daraz.com.bd/p/mdc/2db32f4030938e97fc6e52378089c95d.jpg", 100, DateTime.Now);
            image.RawImagePath = "/img/2db32f4030938e97fc6e52378089c95d.jpg";
            images.Add(image);

            image = new Data.DriveImage("Image 2", "testimages2.com", "https://static-01.daraz.com.bd/p/f1b6a7f62de7293a31b272c315312deb.jpg", 100, DateTime.Now);
            image.RawImagePath = "/img/f1b6a7f62de7293a31b272c315312deb.jpg";
            images.Add(image);

            image = new Data.DriveImage("Image 1", "testimages3.com", "https://static-01.daraz.com.bd/p/mdc/2db32f4030938e97fc6e52378089c95d.jpg", 100, DateTime.Now);
            image.RawImagePath = "/img/2db32f4030938e97fc6e52378089c95d.jpg";
            images.Add(image);

            image = new Data.DriveImage("Image 2", "testimages4.com", "https://static-01.daraz.com.bd/p/f1b6a7f62de7293a31b272c315312deb.jpg", 100, DateTime.Now);
            image.RawImagePath = "/img/f1b6a7f62de7293a31b272c315312deb.jpg";
            images.Add(image);


            image = new Data.DriveImage("Image 1", "testimages5.com", "https://static-01.daraz.com.bd/p/mdc/2db32f4030938e97fc6e52378089c95d.jpg", 100, DateTime.Now);
            image.RawImagePath = "/img/2db32f4030938e97fc6e52378089c95d.jpg";
            images.Add(image);

            image = new Data.DriveImage("Image 2", "testimages6.com", "https://static-01.daraz.com.bd/p/f1b6a7f62de7293a31b272c315312deb.jpg", 100, DateTime.Now);
            image.RawImagePath = "/img/f1b6a7f62de7293a31b272c315312deb.jpg";
            images.Add(image);

            //image = new Data.DriveImage("Image 1", "testimages7.com", "https://static-01.daraz.com.bd/p/mdc/2db32f4030938e97fc6e52378089c95d.jpg", 100);
            //image.RawImagePath = "/img/2db32f4030938e97fc6e52378089c95d.jpg";
            //images.Add(image);

            //image = new Data.DriveImage("Image 2", "testimages8.com", "https://static-01.daraz.com.bd/p/f1b6a7f62de7293a31b272c315312deb.jpg", 100);
            //image.RawImagePath = "/img/f1b6a7f62de7293a31b272c315312deb.jpg";
            //images.Add(image);

            response.Result = images;
            return response;
        }
    }
}
