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
    public class CategoryRepository:ICategoryContract
    {
        private readonly BgfclContext _context;

        public CategoryRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<int> CreateCategory(Category category)
        {
            int result = 0;
            var query = "INSERT INTO categories (CategoryName,CreatedBy,CreatedDate,IsActive) VALUES (@categoryName,@createdBy,@createdDate,@isActive)";
            var parameters = new DynamicParameters();
            parameters.Add("categoryName", category.CategoryName, DbType.String);
            parameters.Add("createdBy", category.CreatedBy, DbType.String);
            parameters.Add("createdDate", category.CreatedDate, DbType.DateTime);
            parameters.Add("isActive", category.IsActive, DbType.Boolean);
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

        public async Task<IEnumerable<Category>> GetCategories()
        {
            var query = "SELECT * FROM categories where isactive=1";
            using (var connection = _context.CreateConnection())
            {
                var categories = await connection.QueryAsync<Category>(query);
                return categories.ToList();
            }
        }

        public async Task<Category> GetCategory(int id)
        {
            var query = "SELECT * FROM categories WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var category = await connection.QuerySingleOrDefaultAsync<Category>(query, new { id });
                return category;
            }
        }

        public async Task<int> RemoveCategory(int id)
        {
            var query = "update categories set isactive = 0 where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }

        public async Task<int> UpdateCategory(Category category)
        {
            var query = "update categories set categoryname = @categoryname, updatedby = @updatedby, updateddate = @updateddate where id = @id";
            var parameters = new DynamicParameters();
            parameters.Add("categoryname", category.CategoryName, DbType.String);
            parameters.Add("updatedby", category.UpdatedBy, DbType.String);
            parameters.Add("updateddate", category.UpdatedDate, DbType.DateTime);
            parameters.Add("id", category.Id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                return result;
            }
        }

    }
}
