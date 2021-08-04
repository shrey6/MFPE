using PortfolioGateway.Models;
using PortfolioGateway.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortfolioGateway.Context;
using PortfolioGateway.Routing;
using Microsoft.AspNetCore.Http;
using PortfolioGateway.Models.Response;
using PortfolioGateway.Loggers;

namespace PortfolioGateway.Controllers
{
    [Route(ConstRouting.baseRoute)]
    [ApiController]
    public class PortfolioController : Controller
    {
        private readonly IPortfolioService _portfolioService;
        private readonly ApplicationDbContext context;
        private readonly ILoggerManager _logger;
        public PortfolioController(IPortfolioService portfolioService,ApplicationDbContext _context, ILoggerManager logger)//ILoggerManager logger
        {
            context = _context;
            _logger = logger;
            _portfolioService = portfolioService;
        }
        [HttpPost]
        [Route(ConstRouting.sellAssetRoute)]
        public ActionResult<Status> SellAssets(int id,string assetType,string assetName, int quantity)
        {
            try
            {


                PortfolioDetails portfolioDetails = _portfolioService.GetCustomerPortfolio(id);
                AssetSaleResponse assetSaleResponse = _portfolioService.SellAsset(portfolioDetails, assetName, assetType, quantity);
                if (assetSaleResponse != null)
                {
                    if (assetSaleResponse.SaleStatus == true)
                    {
                        portfolioDetails.NetWorth = assetSaleResponse.Networth;
                        RemoveAsset(portfolioDetails, assetType, assetName, quantity);
                        portfolioDetails = _portfolioService.GetCustomerPortfolio(id);
                        _logger.LogInformation($"{assetName} sold from portfolio with portfolioId {portfolioDetails.PortfolioId}");
                        Response.StatusCode = StatusCodes.Status200OK;
                        return new Status { data = portfolioDetails, Message = "Success", SaleStatus = true };
                        //return Ok( portfolioDetails);
                    }
                    
                }
                _logger.LogInformation($"{assetName} from portfolio with portfolioId {portfolioDetails.PortfolioId} not found");
                Response.StatusCode = StatusCodes.Status404NotFound;
                return new Status { Message = "Not Found", SaleStatus = false };
            }
            catch
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                _logger.LogInformation($"Error while selling assets");
                return new Status { Message = "Internal server error", SaleStatus = false };
            }
            
        }
   
        private void RemoveAsset(PortfolioDetails portfolioDetails, string assetType, string assetName,int quantity)
        {
            if(assetType == AssetType.Stock)
            {
                var stock = (from s1 in context.StockDetails
                             where s1.PortfolioId == portfolioDetails.PortfolioId && s1.StockName == assetName
                             select s1).SingleOrDefault();
                stock.StockCount = stock.StockCount - quantity;
                if(stock.StockCount == 0)
                {
                    context.StockDetails.Remove(stock);
                    context.SaveChanges();
                }
                else
                {
                    context.StockDetails.Update(stock);
                    context.SaveChanges();
                }
            }
            else
            {
                var mutualFund = (from s1 in context.MutualFundDetails
                                  where s1.PortfolioId == portfolioDetails.PortfolioId && s1.MutualFundName == assetName
                                  select s1).SingleOrDefault();
                mutualFund.MutualFundUnits = mutualFund.MutualFundUnits - quantity;
                if(mutualFund.MutualFundUnits == 0)
                {
                    context.MutualFundDetails.Remove(mutualFund);
                    context.SaveChanges();
                }
                else
                {
                    context.MutualFundDetails.Update(mutualFund);
                    context.SaveChanges();
                }
            }
        }
        [HttpGet]
        [Route(ConstRouting.getPortfolioRoute)]
        public ActionResult<Status> GetCustomerPortfolio(int portfolioId)
        {
            try
            {
                PortfolioDetails portfolioDetails = _portfolioService.GetCustomerPortfolio(portfolioId);
                _logger.LogInformation($"retrieving portfolio details of user with portfolioId {portfolioDetails.PortfolioId}");
                Response.StatusCode = StatusCodes.Status200OK;
                return new Status { data = portfolioDetails, Message = "Success", SaleStatus = true }; ;
            }
            catch(Exception e)
            {
                _logger.LogInformation($"not able to fetch portfolio details of user with portfolioId {portfolioId}");
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return new Status { Message = "Internal server error", SaleStatus = false };
            }
            
            
            
        }
    }
}
