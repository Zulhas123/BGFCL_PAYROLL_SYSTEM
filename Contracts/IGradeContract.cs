using Entities.ViewModels;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IGradeContract
    {
        public Task<int> CreateGrade(Grade grade);

        public Task<List<GradeViewModel>> GetGradesByEmployeeType(int employeeTypeId);
        public Task<List<GradeViewModel>> GetGrades();

        public Task<Grade> GetGradeById(int gradeId);

        public Task<int> UpdateGrade(Grade grade);

        public Task<int> RemoveGrade(int id);
        public Task<int> RemoveBasic(int id);
        public Task<int> CreateBasic(Basic basic);
        public  Task<int> UpdateBasic(Basic basic);
        public Task<List<BasicViewModel>> GetBasics();
        public Task<List<Basic>> GetBasicsByGradeId(int gradeId);
        public Task<Basic> GetBasicById(int gradeId);
    }
}
