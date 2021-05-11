using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI.Common
{
    public class PRESETS
    {
        public enum PAYMENT_STATUS {
            UNPAID=1,
            PAID=0,
            FAILED = 2,
            CANCELLED = 3
        }

        public enum PROMOTYPE {
            PERCENTAGE=0,
            FIX=1

        }


    }
    public class APPLICATION_PAGE_NAME
    {
        public const string ORDER_TRACKING = "OrderTracking";
        public const string INVOICE_ISSUE = "InvoiceIssue";
        public const string HOME = "Home";
        public const string RECEIPT_REJECT = "ReceiptReject";
        public const string RECEIPT_ACCEPT = "ReceiptAccept";
    }
}