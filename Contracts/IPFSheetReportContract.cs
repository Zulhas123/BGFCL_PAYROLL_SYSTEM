using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IPFSheetReportContract
    {
        public Task<List<PFSheetReport>> GetPFSheetOF(List<string> jobCodes, int monthId, string? department);
        public Task<List<PFSheetReport>> GetPFSheet(string jobCode,int monthid, string? departmentId);
    }
}
