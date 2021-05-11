//using Insight.Database;
//using MySql.Data.MySqlClient;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Web;
//using DKR_API.Models;
//using DKR_API.Models.Inbox;
//using DKR_API.Common;

//namespace DKR_API.Repository
//{
//    public class InboxRepository
//    {
//        public static List<InboxInvitationModel> GetInvitation(InboxInvitationModel_Request inboxInvitationModel_request)
//        {
//            List<InboxInvitationModel> model = new List<InboxInvitationModel>();
//            try
//            {
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    var perameter = new
//                    {
//                        Member_Id = inboxInvitationModel_request.MemberId.Decrypt().ValidateGuid(),
//                        InvitationType = inboxInvitationModel_request.Type,
//                        Status = inboxInvitationModel_request.Status,
//                        Startpage = inboxInvitationModel_request.StartPage,
//                        Perpage = inboxInvitationModel_request.PerPage,
//                    };
//                    #region select
//                    model = DB.QuerySql<InboxInvitationModel>(@"CALL Getinvitations(?Member_Id,?InvitationType,?Startpage,?Perpage)", perameter).ToList();
//                    foreach (var item in model)
//                    {
//                        item.MemberId = item.MemberId.Encrypt();
//                        item.InvitationId = item.InvitationId.Encrypt();
//                        item.ShortListId = item.ShortListId.Encrypt();
//                    }
//                    #endregion
//                };
//                return model;
//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//                return model;
//            }
//        }

//        public static List<InboxSMSModel> GetSMS(InboxInvitationModel_Request inboxInvitationModel_request)
//        {
//            List<InboxSMSModel> model = new List<InboxSMSModel>();
//            try
//            {
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    var perameter = new
//                    {
//                        Member_Id = UserIdentity.UserId,
//                        Type = inboxInvitationModel_request.Type,
//                        StartPage = inboxInvitationModel_request.StartPage * inboxInvitationModel_request.PerPage,
//                        PerPage = inboxInvitationModel_request.PerPage, //10,
//                    };
//                    #region select
//                    model = DB.QuerySql<InboxSMSModel>(@"CALL GetSentSMS(?Member_Id,?Type,?StartPage,?PerPage)", perameter).ToList();
//                    model.ForEach(x => x.MemberId = x.MemberId.Encrypt());
//                    #endregion
//                };
//                return model;
//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//                return model;
//            }
//        }

//        public static List<InboxUserModel> GetRequest(InboxInvitationModel_Request inboxInvitationModel_request)
//        {
//            List<InboxRequestModel> model = new List<InboxRequestModel>();
//            List<InboxUserModel> inboxModelList = new List<InboxUserModel>();
//            try
//            {
//                using (MySqlConnection DB = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
//                {
//                    var perameter = new
//                    {
//                        Member_Id = UserIdentity.UserId,
//                        Type = inboxInvitationModel_request.Type,
//                        StartPage = inboxInvitationModel_request.StartPage * inboxInvitationModel_request.PerPage,
//                        PerPage = inboxInvitationModel_request.PerPage,
//                    };
//                    #region select
//                    model = DB.QuerySql<InboxRequestModel>(@"CALL GetRequest(?Member_Id,?Type,?StartPage,?PerPage)", perameter).ToList();
//                    if (model.Count > 0)
//                    {
//                        var ProfileId = model.Select(x => x.ProfileId).Distinct().ToList();
//                        foreach (var id in ProfileId)
//                        {
//                            List<RequestDataModel> RequestModellist = new List<RequestDataModel>();
//                            InboxRequestModel inboxmodelData = model.Where(x => x.ProfileId == id).FirstOrDefault();
//                            List<InboxRequestModel> inboxmodel = new List<InboxRequestModel>();
//                            inboxmodel = model.Where(x => x.ProfileId == id).ToList();
//                            InboxUserModel inboxUserModel = new InboxUserModel();
//                            inboxUserModel.ProfileId = inboxmodelData.ProfileId;
//                            inboxUserModel.FirstName = inboxmodelData.FirstName;
//                            inboxUserModel.LastName = inboxmodelData.LastName;
//                            inboxUserModel.MemberId = inboxmodelData.MemberId.Encrypt();

//                            foreach (var item in inboxmodel)
//                            {
//                                var inboxArr = new RequestDataModel
//                                {
//                                    RequestId = item.RequestId.Encrypt(),
//                                    RequestMemberId = item.RequestMemberId.Encrypt(),
//                                    RequestSubject = item.RequestSubject,
//                                    RequestPartnerId = item.RequestPartnerId.Encrypt(),
//                                    RequestReminder = item.RequestReminder,
//                                    count = item.count,
//                                };
//                                RequestModellist.Add(inboxArr);
//                            }
//                            inboxUserModel.requestDataModels = RequestModellist;
//                            inboxModelList.Add(inboxUserModel);
//                        }
//                    };
//                    #endregion
//                    return inboxModelList;
//                }
//            }
//            catch (Exception ex)
//            {
//                LoggingRepository.SaveException(ex);
//                return inboxModelList;
//            }
//        }
//    }
//}