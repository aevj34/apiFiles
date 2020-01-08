using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace apiFiles.Controllers
{
    public class FilesController : ApiController
    {

        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IEnumerable<FilePath> Get()
        {
            const string PATH_FILES = "~/Files/";

            string filePath = HttpContext.Current.Server.MapPath(PATH_FILES);

            List<FilePath> files = new List<FilePath>();
            DirectoryInfo dirInfo = new DirectoryInfo(filePath);

            foreach (FileInfo fInfo in dirInfo.GetFiles())
                files.Add(new FilePath() { nombre = fInfo.Name});

            return files.ToList();
        }


        public class FilePath
        {
            public string nombre { get; set; }
        }


        [HttpGet]
        [Route("api/files/GetFile")]
        public HttpResponseMessage GetFile(string fileName)
        {
            const string PATH_FILES = "~/Files/";
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            string filePath = HttpContext.Current.Server.MapPath(PATH_FILES) + fileName;

            if (!File.Exists(filePath))
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("No se encuentra el archivo: {0} .", fileName);
                return response;
            }

            byte[] bytes = File.ReadAllBytes(filePath);
            response.Content = new ByteArrayContent(bytes);
            response.Content.Headers.ContentLength = bytes.LongLength;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
            return response;
        }

    }
}
