using Contracts;
using Dapper;
using Entities;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.ViewModels;

namespace Repositories
{
    public class GradeRepository:IGradeContract
    {
        private readonly BgfclContext _context;

        public GradeRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<int> CreateGrade(Grade grade)
        {
            int result = 0;
            var query = "INSERT INTO Grades (GradeName,Description,EmployeeTypeId,IsActive,CreatedBy,CreatedDate) VALUES (@gradeName,@description,@employeeTypeId,@isActive,@createdBy,@createdDate)";
            var parameters = new DynamicParameters();
            parameters.Add("gradeName", grade.GradeName, DbType.String);
            parameters.Add("description", grade.Description, DbType.String);
            parameters.Add("employeeTypeId", grade.EmployeeTypeId, DbType.Int32);
            parameters.Add("isActive", grade.IsActive, DbType.Boolean);
            parameters.Add("createdBy", grade.CreatedBy, DbType.String);
            parameters.Add("createdDate", grade.CreatedDate, DbType.DateTime);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteScalarAsync<int>(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<List<GradeViewModel>> GetGradesByEmployeeType(int employeeTypeId)
        {
            var query = "select Grades.Id, Grades.GradeName, Grades.IsActive, Grades.EmployeeTypeId,EmployeeTypes.EmployeeTypeName from Grades join EmployeeTypes on Grades.EmployeeTypeId = EmployeeTypes.Id where Grades.IsActive=1 and EmployeeTypeId=@employeeTypeId";
            using (var connection = _context.CreateConnection())
            {
                var grades = await connection.QueryAsync<GradeViewModel>(query,new { employeeTypeId });
                return grades.ToList();
            }
        }
        public async Task<List<GradeViewModel>> GetGrades()
        {
            var query = "select Grades.Id, Grades.GradeName, Grades.IsActive, Grades.EmployeeTypeId,EmployeeTypes.EmployeeTypeName from Grades join EmployeeTypes on Grades.EmployeeTypeId = EmployeeTypes.Id where Grades.IsActive=1";
            using (var connection = _context.CreateConnection())
            {
                var grades = await connection.QueryAsync<GradeViewModel>(query);
                return grades.ToList();
            }
        }

        public async Task<Grade> GetGradeById(int gradeId)
        {
            var query = "SELECT * FROM Grades where isactive=1 and id=@id";
            using (var connection = _context.CreateConnection())
            {
                var grade = await connection.QuerySingleOrDefaultAsync<Grade>(query, new { id = gradeId });
                return grade;
            }
        }

        public async Task<int> UpdateGrade(Grade grade)
        {
            var query = "update Grades set GradeName = @gradeName,Description=@description,EmployeeTypeId=@employeeTypeId,updatedby = @updatedby, updateddate = @updateddate where id = @id";
            var parameters = new DynamicParameters();
            parameters.Add("gradeName", grade.GradeName, DbType.String);
            parameters.Add("description", grade.Description, DbType.String);
            parameters.Add("employeeTypeId", grade.EmployeeTypeId, DbType.Int32);
            parameters.Add("updatedby", grade.UpdatedBy, DbType.String);
            parameters.Add("updateddate", grade.UpdatedDate, DbType.DateTime);
            parameters.Add("id", grade.Id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                return result;
            }
        }

        public async Task<int> RemoveGrade(int id)
        {
            var query = "update Grades set isactive = 0 where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }
        public async Task<int> RemoveBasic(int id)
        {
            var query = "Delete from  Basics where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }
        public async Task<List<BasicViewModel>> GetBasics()
        {
            var query = "  Select Basics.id,Basics.GradeId,GradeName,BasicAmount,EmployeeTypeId from Basics Inner join Grades on Basics.GradeId=Grades.Id where Grades.IsActive=1";
            using (var connection = _context.CreateConnection())
            {
                var basics = await connection.QueryAsync<BasicViewModel>(query);
                return basics.ToList();
            }
        }
        public async Task<int> CreateBasic(Basic basic)
        {
            int result = 0;
            var query = "INSERT INTO Basics (BasicAmount,GradeId,CreatedBy,CreatedDate) VALUES (@basicAmount,@gradeId,@createdBy,@createdDate)";
            var parameters = new DynamicParameters();
            parameters.Add("basicAmount", basic.BasicAmount, DbType.Double);
            parameters.Add("gradeId", basic.GradeId, DbType.Int32);
            parameters.Add("createdBy", basic.CreatedBy, DbType.String);
            parameters.Add("createdDate", basic.CreatedDate, DbType.DateTime);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteScalarAsync<int>(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }
        public async Task<int> UpdateBasic(Basic basic)
        {
            var query = "update Basics set GradeId = @gradeid,BasicAmount=@basicAmount where id = @id";
            var parameters = new DynamicParameters();
            parameters.Add("gradeid", basic.GradeId, DbType.Int32);
            parameters.Add("basicAmount", basic.BasicAmount, DbType.Decimal);
            parameters.Add("id", basic.Id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                return result;
            }
        }
        public async Task<List<Basic>> GetBasicsByGradeId(int gradeId)
        {
            var query = "select * from basics where gradeid = @gradeId";
            using (var connection = _context.CreateConnection())
            {
                var basics = await connection.QueryAsync<Basic>(query, new { gradeId });
                return basics.ToList();
            }
        }
        public async Task<Basic> GetBasicById(int basicId)
        {
            var query = "select * from basics where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var basic = await connection.QuerySingleOrDefaultAsync<Basic>(query, new { id = basicId });
                return basic;
            }
        }
    }
}
