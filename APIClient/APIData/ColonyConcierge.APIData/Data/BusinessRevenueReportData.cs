using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    /// <summary>
    /// Class that allows for businesses and administrators to collect business statistics
    /// </summary>
    public class BusinessRevenueReportData
    {

        public TimeStamp ReportPeriodStart { get; set; }

        public TimeStamp ReportPeriodEnd { get; set; }

        public int TotalOrderCount { get; set; }

        public Dictionary<string, int> CategorizedOrderTotals { get; set; }

        public decimal Subtotals { get; set; }

        public decimal FeeTotals { get; set; }

        public decimal TotalTaxes { get; set; }

    }
}
