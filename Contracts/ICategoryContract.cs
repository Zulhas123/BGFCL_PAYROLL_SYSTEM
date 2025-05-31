using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface ICategoryContract
    {
        public Task<int> CreateCategory(Category category);

        public Task<IEnumerable<Category>> GetCategories();

        public Task<Category> GetCategory(int id);

        public Task<int> RemoveCategory(int id);

        public Task<int> UpdateCategory(Category category);
    }
}
