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
    public class RolesRepository:IRolesContract
    {
        private readonly BgfclContext _context;

        public RolesRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<int> CreateRole(Roles role)
        {

            int result = 0;
            var query = "INSERT INTO Roles (user_id,guest_pk_id,school_id,title,slug,notes,is_employee,is_authority,is_staff,Is_Active,CreatedBy,created_at) VALUES (@user_id,@guest_pk_id,@school_id,@title,@slug,@notes,@is_employee,@is_authority,@is_staff,@is_Active,@createdBy,@created_at)";
            var parameters = new DynamicParameters();
            parameters.Add("user_id", role.UserId, DbType.Int32);
            parameters.Add("guest_pk_id", role.GuestPkId, DbType.Int32);
            parameters.Add("school_id", role.SchoolId, DbType.Int32);
            parameters.Add("title", role.Title, DbType.String);
            parameters.Add("slug", role.Slug, DbType.String);
            parameters.Add("notes", role.Notes, DbType.String);
            parameters.Add("is_employee", role.IsEmployee, DbType.Int32);
            parameters.Add("is_authority", role.IsAuthority, DbType.Int32);
            parameters.Add("is_staff", role.IsStaff, DbType.Int32);
            parameters.Add("is_Active", 1);
            parameters.Add("createdBy", role.UserId, DbType.Int32);
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

        public async Task<int> DeleteRole(int id)
        {
            var query = "update Roles set is_active = 0 where Id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }

        public async Task<List<Roles>> GetRole()
        {
            var query = " SELECT id, title, slug, COALESCE(is_employee, 0) AS isEmployee,COALESCE(is_authority, 0) AS isAuthority,COALESCE(is_staff, 0) AS isStaff,COALESCE(is_active, 0) AS isActive FROM roles;";
            using (var connection = _context.CreateConnection())
            {
                var roles = await connection.QueryAsync<Roles>(query);
                return roles.ToList();
            }
        }

        public async Task<Roles> GetRoleById(int id)
        {
            var query = " SELECT id, title, slug,notes, COALESCE(is_employee, 0) AS isEmployee,COALESCE(is_authority, 0) AS isAuthority,COALESCE(is_staff, 0) AS isStaff,COALESCE(is_active, 0) AS isActive  FROM roles where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var role = await connection.QueryFirstOrDefaultAsync<Roles>(query, new { id });

                return role;
            }
        }

        public async Task<int> UpdateRole(Roles role)
        {
            var query = "UPDATE Roles SET user_id = @user_id,Guest_pk_Id=@guest_pk_Id,School_Id=@school_Id,Title=@title,Slug=@slug, Notes = @notes,Is_employee=@is_employee,is_authority=@is_authority,is_staff=@is_staff, Is_Active = @is_Active, updatedby = @updatedby, updated_at = @updated_at WHERE id = @id";
            var parameters = new DynamicParameters();
            parameters.Add("user_id", role.UserId, DbType.Int32);
            parameters.Add("guest_pk_id", role.GuestPkId, DbType.Int32);
            parameters.Add("school_id", role.SchoolId, DbType.Int32);
            parameters.Add("title", role.Title, DbType.String);
            parameters.Add("slug", role.Slug, DbType.String);
            parameters.Add("notes", role.Notes, DbType.String);
            parameters.Add("Is_employee", role.IsEmployee, DbType.Boolean);
            parameters.Add("is_authority", role.IsAuthority, DbType.Boolean);
            parameters.Add("is_staff", role.IsStaff, DbType.Boolean);
            parameters.Add("is_Active", role.IsActive, DbType.Boolean);
            parameters.Add("updatedby", role.UserId, DbType.Int32);
            parameters.Add("updated_at", DateTime.Now);
            parameters.Add("id", role.Id, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return result; // This returns the number of rows affected
            }
        }
       
    }
}
