using Contracts;
using Dapper;
using Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class SchoolRepository: ISchoolContract
    {
        private readonly BgfclContext _context;

        public SchoolRepository(BgfclContext context)
        {
            _context = context;
        }
        public async Task<int> CreateSchool(Schools school)
        {

            int result = 0;
            var query = "INSERT INTO Schools (user_id,guest_pk_id,short_id,title,notes,has_erp,has_payroll,Is_Active,CreatedBy,created_at) VALUES (@user_id,@guest_pk_id,@short_id,@title,@notes,@has_erp,@has_payroll,@is_Active,@createdBy,@created_at)";
            var parameters = new DynamicParameters();
            parameters.Add("user_id", school.UserId, DbType.Int32);
            parameters.Add("guest_pk_id", school.GuestPkId, DbType.Int32);
            parameters.Add("short_id", school.ShortId, DbType.Int32);
            parameters.Add("title", school.Title, DbType.String);
            parameters.Add("notes", school.Notes, DbType.String);
            parameters.Add("has_erp", school.has_erp, DbType.Int32);
            parameters.Add("has_payroll", school.has_payroll, DbType.Int32);
            parameters.Add("is_Active", 1);
            parameters.Add("createdBy", school.UserId, DbType.Int32);
            parameters.Add("created_at", DateTime.Now, DbType.DateTime);
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
        public async Task<int> DeleteSchool(int id)
        {
            var query = "update Schools set is_active = 0 where Id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }
        public async Task<List<Schools>> GetSchools()
        {
            var query = "SELECT Id, Title,notes, short_id,COALESCE(is_active, 0) AS isActive,COALESCE(has_erp, 0) AS has_erp,COALESCE(has_payroll, 0) AS has_payroll FROM Schools";
            using (var connection = _context.CreateConnection())
            {
                var schools = await connection.QueryAsync<Schools>(query);
                return schools.ToList();
            }
        }
        public async Task<Schools> GetSchoolById(int id)
        {
            var query = "SELECT Id, Title,notes, short_id,COALESCE(is_active, 0) AS isActive,COALESCE(has_erp, 0) AS has_erp,COALESCE(has_payroll, 0) AS has_payroll FROM Schools where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var school = await connection.QueryFirstOrDefaultAsync<Schools>(query, new { id });

                return school;
            }
        }
        public async Task<int> UpdateSchool(Schools school)
        {
            var query = " UPDATE Schools SET user_id = @user_id,Guest_pk_Id=@guest_pk_Id,short_id=@short_id,Title=@title,Notes = @notes,has_erp=@has_erp,has_payroll=@has_payroll,Is_Active = @is_Active, updatedby = @updatedby, updated_at = @updated_at WHERE id = @id";
            var parameters = new DynamicParameters();
            parameters.Add("user_id", school.UserId, DbType.Int32);
            parameters.Add("guest_pk_id", school.GuestPkId, DbType.Int32);
            parameters.Add("short_id", school.ShortId, DbType.Int32);
            parameters.Add("title", school.Title, DbType.String);
            parameters.Add("notes", school.Notes, DbType.String);
            parameters.Add("has_erp", school.has_erp, DbType.Int32);
            parameters.Add("has_payroll", school.has_payroll, DbType.Int32);
            parameters.Add("is_Active", school.IsActive, DbType.Boolean);
            parameters.Add("updatedby", school.UserId, DbType.Int32);
            parameters.Add("updated_at", DateTime.Now);
            parameters.Add("id", school.Id, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return result; 
            }
        }

    }
}
