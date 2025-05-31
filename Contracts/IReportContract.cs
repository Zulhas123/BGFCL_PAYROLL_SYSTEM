using Entities;
using Entities.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IReportContract
    {
        public Task<List<IncomeTaxMonthlyViewModel>> GetMonthlyIncomeTax(int monthId);
    }
}
