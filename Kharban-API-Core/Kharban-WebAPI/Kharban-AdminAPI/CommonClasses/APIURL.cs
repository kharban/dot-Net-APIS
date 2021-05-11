using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_AdminAPI
{
    public class APIURL
    {
        public const string GET_LOGIN_USER_INFO = "getloginuserinfo";
        public const string ADMIN_FORGOT_PASSWORD = "adminForgotPassword";
        public const string ADMIN_CHANGE_PASSWORD = "adminChangePassword";
        public const string GET_COUNTRIES = "getCountries";
        public const string GET_NOTIFICATIONS = "getNotifications";
        public const string SEND_PUSH_NOTIFICATIONS = "sendpushNotifications";


        public const string GET_CUSTOMERS = "getCustomers";
        public const string DELETE_CUSTOMER = "deleteCustomer";
        public const string EDIT_CUSTOMER = "editCustomer";
        public const string UPDATE_STATUS_CUSTOMER = "updateStatusCustomer";


        public const string PROVIDER_LIST = "providerList";
        public const string DELETE_PROVIDER = "deleteProvider";
        public const string VIEW_PROVIDER = "viewProvider";
        public const string EDIT_PROVIDER = "editProvider";
        public const string UPDATE_STATUS_PROVIDER = "updateStatusProvider";
        public const string APPROVE_RETAILER = "approveRetailer";
        public const string GET_PROVIDER_REQUEST_LIST = "getproviderrequestlist";


        public const string GET_CATEGORIES = "getCategories";
        public const string GET_CATEGORY_LIST = "getCategoryList";
        public const string GET_CATEGORY = "getCategory";
        public const string DELETE_CATEGORY = "deleteCategory";
        public const string ADD_CATEGORY = "addCategory";
        public const string EDIT_CATEGORY = "editCategory";
        public const string UPDATE_STATUS_CATEGORY = "updateStatusCategory";


        public const string GET_REPORT_DATA_COUNT = "getreportdatacount";
        public const string GET_REPORT_CUSTOMER_LIST = "getreportcustomerlist";
        public const string GET_REPORT_PROVIDER_LIST = "getReportProviderList";
        public const string GET_REPORT_BOOKING_LIST = "getreportbookinglist";
        public const string GET_REPORT_TRANSICTION_LIST = "getreporttransictionlist";
        public const string PROVIDER_PAYMENT_SETTLE = "providerPaymentSettle";

        public const string GET_SETTING = "getsetting";
        public const string UPDATE_SETTING = "updatesetting";

        public const string GET_BOOKINGS = "getBookings";
        public const string GET_PAYMENTS = "getPayments";
        public const string GET_PAYMENT_EXPORT = "getPaymentExport";

        public const string GET_CONTENT = "getContent";
        public const string UPDATE_CONTENT = "updateContent";

        public const string GET_FAQ_LIST = "getFaqList";
        public const string GET_FAQ = "getFaq";
        public const string DELETE_FAQ = "deleteFaq";
        public const string ADD_FAQ = "addFaq";
        public const string EDIT_FAQ = "editFaq";
        public const string UPDATE_STATUS_FAQ = "updateStatusFaq";


        public const string GET_EARNING_CHART = "getEarningChart";
        public const string GET_CUSTOMER_CHART = "getCustomerChart";
        public const string GET_PROVIDER_CHART = "getProviderChart";
        public const string GET_GROWTH_CHART = "getGrowthChart";
        public const string GET_PIE_CHART = "getPieChart";
        public const string GET_AVGRATE_CHART = "getAvgRateChart";

        public const string UPLOAD_PROVIDER_IMAGE_BY_MOBILE = "uploadProviderImageByMobile";
        public const string UPLOAD_CUSTOMER_IMAGE_BY_MOBILE = "uploadCustomerImageByMobile";
    }
}