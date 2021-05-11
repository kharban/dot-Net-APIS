using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Kharban_WebAPI.Helper
{
    public static class APIBind
    {
        public static string CasePOST(dynamic model,string URldata)
        {
            var jsonData = JsonConvert.SerializeObject(model);

            //var client = new RestClient("https://kharban.net:2096/api/v1/providerImagesUpload");
            var client = new RestClient(URldata);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("image_extension", model.image_extension);
            request.AddParameter("image", model.image);
            request.AddParameter("ImageType", model.ImageType);
            request.AddParameter("id", model.id);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}
