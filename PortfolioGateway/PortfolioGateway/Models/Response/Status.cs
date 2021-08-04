using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioGateway.Models.Response
{
    public class Status
    {
        public PortfolioDetails data { get; set; }
        public string Message { get; set; }
        public bool SaleStatus { get; set; }
    }
}
