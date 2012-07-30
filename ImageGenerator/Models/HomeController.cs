using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Drawing;

namespace ImageGenerator.Models
{
    public class HomeController : Controller
    {
        readonly string[] validextensions = new string[] {".png",".jpg",".jpeg",".gif"};

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Process(HttpPostedFileBase file, string text)
        {
            string error = string.Empty;

            //get file
            string filename = string.Empty;
            Stream filecontents = null;

            if ((file == null) || (file.ContentLength == 0))
                error = "No file uploaded";
            else
            {
                //get filename
                filename = file.FileName;
                filecontents = file.InputStream;
            }

            Bitmap uploadedImage = new Bitmap(200, 200);

            //check valid extensions
            if (!string.IsNullOrEmpty(filename))
            {
                if (!validextensions.Contains(Path.GetExtension(filename)))
                    error = "File is not an image";
                else
                {
                    //attempt to load image
                    uploadedImage = new Bitmap(filecontents);
                }
            }

            //if there's an error, draw it on the image
            if (!string.IsNullOrEmpty(error))
            {
                var g = Graphics.FromImage(uploadedImage);
                g.DrawString(error, new Font("Arial", 8), new SolidBrush(Color.Red), new PointF(1, 1));
            }

            //create new image
            Bitmap newimage = new Bitmap(400, 200);
            var graphics = Graphics.FromImage(newimage);

            //blend into image instead of overriding, this way we can use trasnparent PNGs instead of ignoring the transparency.
            graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            //lets get the relative size so it will keep its ratio
            var width = 200;
            var height = 200;

            float ratio = (float)uploadedImage.Width / (float)uploadedImage.Height;
            if (ratio < 1)
                width = (int)(width * ratio);
            else
                height = (int)(height / ratio);

            //lets center the image
            var top = (200 - height) / 2;
            var left = (200 - width) / 2;

            //draw original file
            graphics.DrawImage(uploadedImage, new Rectangle(left, top, width, height));

            //lets determine the size of the string;
            var sizeofString = graphics.MeasureString(text, new Font("Arial", 10));

            //lets center the text
            var texttop = (200 - sizeofString.Height) / 2;
            var textleft = (200 - sizeofString.Width) / 2;

            //add text
            graphics.DrawString(text, new Font("Arial", 10), new SolidBrush(Color.Black), new PointF(textleft + 200, texttop));


            //send to client
            Response.ContentType = "image/png"; 
            newimage.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);

            return new EmptyResult();
        }
    }
}
