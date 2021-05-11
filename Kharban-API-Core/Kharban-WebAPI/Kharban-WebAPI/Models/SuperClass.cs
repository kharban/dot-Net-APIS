using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI
{
    public interface iDestruct
    {

    }
    public class destructorclass : iDestruct
    {
       public destructorclass()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        ~destructorclass()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
    public class SuperClass : destructorclass
    {

    }
}