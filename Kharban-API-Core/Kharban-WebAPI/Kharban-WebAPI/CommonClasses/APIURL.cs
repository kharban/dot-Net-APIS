using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kharban_WebAPI
{
    public class APIURL
    {
        public const string GET_CATEGORY_LIST = "getCategoryList";
        public const string GET_OTP = "Getotp";
        public const string SAVE_DEVICE_INFO = "saveDeviceInfo";
        public const string ADD_PROVIDER = "addProvider";
        public const string ADD_CUSTOMER = "addCustomer";
        public const string UPDATE_CUSTOMER = "updateCustomer";
        public const string GET_CUSTOMER = "getCustomer";
        public const string GET_PROVIDERS = "getProviders";
        public const string GET_LOGIN_USER_INFO = "getLoginUserInfo";
        public const string GET_PROVIDER_INFORMATION = "getProviderInformation";
        public const string UPDATE_PROVIDER_INFORMATION = "updateProviderInformation";
        public const string UPDATE_PROVIDER_LOCATION = "updateProviderLocation";
        public const string UPDATE_PROVIDER_ONLINE_STATUS = "updateProviderOnlineStatus";
        public const string GET_BOOKINGS = "getBooking";
        public const string GET_COMPLETE_BOOKINGS = "getCompleteBookings";
        public const string SAVE_BOOKING_REQUEST = "saveBookingRequest";
        public const string GET_CONTENT = "getContent";
        public const string GET_FAQ_LIST = "getFaqList";
        public const string GET_FAQ = "getFaq";
        public const string GET_COUNTRY = "getCountry";

        public const string CUSTOMER_CANCEL_JOB = "customerCancelJob";
        public const string GET_CONNECTED_PROVIDERS = "getConnectedProviders";
        public const string GET_ACCEPTED_PROVIDER = "getAcceptedProvider";
        public const string GET_PENDING_RATING_BOOKING = "getPendingRatingBooking";
        public const string SUBMIT_RATING = "submitRating";
        public const string RECEIPT_ACCEPT_DECLINE = "receiptAcceptDecline";
        public const string SEND_SUPPORT_EMAIL = "sendSupportEmail";
        public const string GET_JOBREQUEST = "getJobRequest";
        public const string GET_JOBDETAIL = "getJobDetail";
        public const string JOB_ACCEPT_DECLINE = "jobAcceptDecline";
        public const string GENERATE_RECEIPT = "generateReceipt";
        public const string GET_ACTIVE_PROVIDER_BOOKING = "getActiveProviderBooking";
        public const string GET_ACTIVE_CUSTOMER_BOOKING = "getActiveCustomerBooking";
        public const string GET_BOOKING_DETAIL = "getBookingDetail";
        public const string GET_PAST_PROVIDER_BOOKING = "getPastProviderBooking";
        public const string GET_PAST_CUSTOMER_BOOKING = "getPastCustomerBooking";
        public const string GET_EARNING = "getEarning";
        public const string GET_TOTAL_EARNING = "getTotalEarning";

        public const string INSERT_CUSTOMER_CARD = "InsertCustomerCard";
        public const string VIEW_CUSTOMER_CARD = "ViewCustomerCard";
        public const string DELETE_CUSTOMER_CARD = "DeleteCustomerCard";


        public const string DELETE_NOTIFICATION = "deleteNotification";
        public const string GET_NOTIFICATION = "getNotification";

        public const string AUTO_CANCEL_BOOKINGS = "autoCancelBookings";
        public const string SEND_BOOKING_REMINDER_TO_PROVIDER = "sendBookingReminderToProvider";

        public const string GET_CUS_PROV_INFO = "getCusProvInfo";
        public const string COMPLETE_BY_PROVIDER = "completeByProvider";

        public const string RECENT_LOCATION = "getRecentLocation";

        public const string GET_PLATFORM_FEE = "getPlatformFee";

        public const string GET_SAVERECENT_LOCATION = "saveRecentLocation";
        public const string Mark_FAVOURITE_LOCATION = "markFavouriteLocation";
        public const string All_PROVIDER_CANCEL_BOOKING = "saveBookingCancelByAllProvider";


    }
}