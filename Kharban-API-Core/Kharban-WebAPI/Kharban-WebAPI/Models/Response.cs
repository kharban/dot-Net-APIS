using System;
using System.Collections.Generic;
using System.Text;

namespace Kharban_WebAPI
{
    public class Response<T> 
    {
        public int status { get; set; }
        public bool success { get; set; }
        public string msg { get; set; }
        public T result { get; set; }
    }
    
    public class ResponseList<T> 
    {
        public int status { get; set; }
        public bool success { get; set; }
        public string msg { get; set; }
        public T result { get; set; }
        public int totalDocs { get; set; }
        public int limit { get; set; }
        public int totalPages { get; set; }
        public int page { get; set; }
    }



}
