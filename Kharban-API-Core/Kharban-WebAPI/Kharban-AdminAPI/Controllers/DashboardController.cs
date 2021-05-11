using Microsoft.AspNetCore.Mvc;
using Kharban_AdminAPI.Repository;
using System.Collections.Generic;

namespace Kharban_AdminAPI.Controllers
{
    [Authorize]
    [Route("api/v1")]
    public class DashboardController : BaseController
    {
        [HttpPost, Route(APIURL.GET_EARNING_CHART)]
        public IActionResult GetTotalEarn([FromBody]DashBoardRequestModel model)
        {
            if (model == null)
                model = new DashBoardRequestModel();
            Response<DashBoardChartModel> returnModel = new DashboardRepository().GetTotalEarnChart(model);
            return Ok(returnModel);
        }        
        
        [HttpPost, Route(APIURL.GET_CUSTOMER_CHART)]
        public IActionResult GetCustomerChart([FromBody]DashBoardRequestModel model)
        {
            if (model == null)
                model = new DashBoardRequestModel();
            Response<DashBoardChartModel> returnModel = new DashboardRepository().GetCustomerChart(model);
            return Ok(returnModel);
        }
        
        [HttpPost, Route(APIURL.GET_PROVIDER_CHART)]
        public IActionResult GetProviderChart([FromBody]DashBoardRequestModel model)
        {
            if (model == null)
                model = new DashBoardRequestModel();
            Response<DashBoardChartModel> returnModel = new DashboardRepository().GetProviderChart(model);
            return Ok(returnModel);
        }
        
        [HttpPost, Route(APIURL.GET_GROWTH_CHART)]
        public IActionResult GetGrowthChart([FromBody]DashBoardRequestModel model)
        {
            if (model == null)
                model = new DashBoardRequestModel();
            Response<DashBoardChartModel> returnModel = new DashboardRepository().GetGrowthChart(model);
            return Ok(returnModel);
        }

       [HttpPost, Route(APIURL.GET_PIE_CHART)]
        public IActionResult GetPieChart([FromBody]DashBoardRequestModel model)
        {
            if (model == null)
                model = new DashBoardRequestModel();
            Response<DashBoardChartModel> returnModel = new DashboardRepository().GetPieChart(model);
            return Ok(returnModel);
        }
        
        [HttpPost, Route(APIURL.GET_AVGRATE_CHART)]
        public IActionResult GetAvgRateChart([FromBody]DashBoardRequestModel model)
        {
            if (model == null)
                model = new DashBoardRequestModel();
            Response<DashBoardChartModel> returnModel = new DashboardRepository().GetAvgRateChart(model);
            return Ok(returnModel);
        }
    }
}