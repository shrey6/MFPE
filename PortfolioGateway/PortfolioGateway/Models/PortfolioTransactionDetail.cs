using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioGateway.Models
{
    public class PortfolioTransactionDetail
    {
        [Key]
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public int? StockDetailId { get; set; }
        public int? MutualFundDetailId { get; set; }
    }
}
