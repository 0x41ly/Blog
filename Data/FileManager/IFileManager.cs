using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.FileManager
{
    public interface IFileManager
    {

        FileStream ImageStream(string type,string image);
        Task<string> SaveImage(string type, IFormFile image);
        bool RemoveImage(string type, string image);
    }
}