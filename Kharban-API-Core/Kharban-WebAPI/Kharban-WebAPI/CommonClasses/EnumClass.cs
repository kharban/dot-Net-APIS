using System;
using System.Collections.Generic;
using System.Text;

namespace Kharban_WebAPI.Models
{
    #region Points
    public static class PointBoard
    {
        public static class Name
        {
            public static double Matched = 0.040;
            public static double UnMatched = 0.013;
        }
        public static class Designation
        {
            public static double Matched = 0.040;
            public static double UnMatched = 0.013;
        }
        public static class CompanyName
        {
            public static double Matched = 0.000;
            public static double UnMatched = 0.013;
        }
        public static class Department
        {
            public static double Matched = 0.000;
            public static double UnMatched = 0.012;
        }
        public static class ReportingTo
        {
            public static double Matched = 0.020;
            public static double UnMatched = 0.000;
        }
        public static class Cluster
        {
            public static double Matched = 0.020;
            public static double UnMatched = 0.000;
        }
        public static class City
        {
            public static double Matched = 0.020;
            public static double UnMatched = 0.012;
        }
        public static class Branch
        {
            public static double Matched = 0.020;
            public static double UnMatched = 0.000;
        }
        public static class PhoneNo
        {
            public static double Matched = 0.040;
            public static double UnMatched = 0.013;
        }
        public static class Email
        {
            public static double Matched = 0.020;
            public static double UnMatched = 0.00;
        }
        public static class PresentCTC
        {
            public static double Matched = 0.020;
            public static double UnMatched = 0.00;
        }
        public static class HomeTown
        {
            public static double Matched = 0.020;
            public static double UnMatched = 0.000;
        }
        public static class FilePath
        {
            public static double Matched = 0.040;
            public static double UnMatched = 0.013;
        }
        public static class KRALocationCount
        {
            public static double Matched = 0.020;
            public static double UnMatched = 0.00;
        }
        public static class KRATeamSizeCount
        {
            public static double Matched = 0.020;
            public static double UnMatched = 0.00;
        }
        public static class KRARole
        {
            public static double Matched = 0.020;
            public static double UnMatched = 0.00;
        }
        public static double LineUp = 0.90;
        public static double CVSubmitted = 0.70;
        public static double Metup = 1.18;

    }
    #endregion
    public class EnumClass
    {
        public enum ResponseState
        {
            Success = 200,
            ResposityError = 400,
            ServiceError = 2,
            ConnectionStringError = 3,
            ApiCallError = 4,
            ApiHelper = 5,
            AlreadyExist = 6,
            UpdateSuccessfully = 7,
            InvalidRequest = 8,
            IncompParameter = 9,
            SendingMailError = 10,
            OTPNotVarifies = 11,
            NotFound = 12,
            NotProperFile = 13,
            NotAssigned = 14,
            ResumeAlreadyUploaded = 15,
            NotAuthorisedPerson = 16,
            JobExpired = 17,
            TokenExpired = 401,
            Failure= 400
        }

        public enum JobBoardAssignedType
        {
            TeamLeader = 1,
            Recruitment = 2,
            Self = 3
        }

        public enum RecruitmentType
        {
            InternalRecuitment = 1,
            ExternalRecuitment = 2
        }

        public enum RecordStatus
        {
            Active = 1,
            Passive = 0
        }

        public enum JobPriority
        {
            Urgent = 3,
            High = 6,
            Normal = 9,
            Low = 12
        }

        public enum AccountContactType
        {
            HIRING_MANAGER = 1,
            BUSINESS_MANAGER = 2,
            HR_OPS_OR_PAYMENT = 3,
            PROCUREMENT_OR_AGREMENT = 4,
            BUSINESS_PARTNER_HR = 5,
            HR_SUPPORT = 6
        }

        public enum SchedularStatus
        {
            ForwardToClient = 1,
            Scheduled_Interview = 2,
            Not_Reached_For_Interview = 3,
            Rescheduled_Interview = 4,
            CV_Rejected = 5,
            Met_Up = 6,
            Eligible_for_Next_Round = 7,
            Selected = 8,
            Not_Selected = 9,
            Offered = 10,
            Offer_Canceled = 11,
            Joined = 12,
            Not_Joined = 13,
            Left = 14,
            Offer_Accepted = 15,
            Offer_Not_Accepted = 16,
            Not_Going_For_Interview = 18,
            Going_For_Interview = 17,
            Required_Document = 19,
            Documents_Submitted = 20,
            Eligible_For_Test = 21,
            Test_Not_Qualified = 22,
            Test_Qualified = 23,
            LineUp_For_Interview = 24,
            Rejected = 25,
            Interview_Postponed = 26,
            Documents_Not_Submitted = 27
        }

        public enum MailNotification
        {
            Rescheduleinterview = 4,
            InterViewPostPoned = 5,
            Selected = 8,
            Offered = 10,
            Offer_Accepted = 15,
            DocumentRequired = 19,
            LineUp = 24,
            Eligible_for_Next_Round = 7,
        }

        public enum PropectiveCandidateTaskState
        {
            Active = 1,
            Archive = 2,
            OtherTeamActive = 3,
            OtherTeamArchive = 4
        }
        public const string PROSPACTIVE_CANDIDATE_REFERENCE_TASK = "bbb73a93-a5a1-11ea-b98b-f4939fee1adf";
        public const string WEBSITE_USER = "fc354605-abc7-11ea-9be7-021577426974";
        public enum JobSkills
        {
            PrimarySkills = 0,
            SecondarySkills = 1
        }

        public enum FormulaMethod
        {
            Incentive = 1,
            CreditPoints = 2
        }

        public enum DayType
        {
            Day = 1,
            Week = 2,
            Month = 3,
            Quarter = 4,
            HalfYear = 5,
            Annual = 6
        }

        public enum MailStatus
        {
            Draft = 1,
            Sent = 2
        }

        public enum MailType
        {
            Direct = 1,
            CC = 2,
            BCC = 3
        }

        public enum Can_Resume
        {
            Attach = 1,
            NotAttach = 0
        }

        public enum RecuiterType
        {
            NoRecuiter = 0,
            Internal = 1,
            External = 2,
            SelfRecuiter = 3
        }

        public enum CV_Action
        {
            rejected = 2

        }

        public enum Notification
        {
            AssignedTo = 1,
            BusinessNote = 2,
            QualifyLead = 3,
            ForwardCV = 4,
            RejectedCV = 5,
            RevertCV = 6,
            RequestJob = 7
        }

        public enum ReadNotify
        {
            JobBaord = 1,
            BusinessNote = 2

        }

        public enum CandidateTools
        {
            ForwardStatus = 1
        }

        public enum UserSlab
        {
            A = 78,
            B = 100,
            C = 140
        }

        public const string Notify_Success = "alert-success";
        public const string Notify_Info = "alert-info";
        public const string Notify_Error = "alert-danger";
        public const string Notify_Warning = "alert-warning";

    }



    public static class MetaDataParmeter
    {
        public const string InternalRecruiter = "Internal";
        public const string ExternalRecruiter = "External";
        public const string SelfRecruiter = "Self";
        public const string ErrorFile = @"/Log.txt";
        public const string RecuiterCandidateCV = "/RecruiterCandidate/create";
        public const string ResumeUploadFolder = "/Assest/files/resume/";
        public const string RecruiterCVFolder = @"/recruitercv/";
    }

    public static class DefaultData
    {
        public static int Formula_BillingAmount = 0;
        public static decimal Formula_CreditPointPerc = 1 / 100;
    }

    public enum JobListType
    {
        Active = 1,
        Archive = 2,
        Rework = 3,
    }


    public enum ActionTodoList
    {
        Add = 1,
        Edit = 2,
        Delete = 3
    }

    public enum MonthNameList
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public enum WeekNameList
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }
}
