using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioGateway.Models
{
    public class AssetSaleResponse
    {
        public int Networth { get; set; }
        public string Message { get; set; }
        public bool SaleStatus { get; set; }
    }
}
