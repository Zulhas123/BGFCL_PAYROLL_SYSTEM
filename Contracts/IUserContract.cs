using Entities.ViewModels;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUserContract
    {
        public Task<List<User>> GetUser();

        public Task<User> GetUserById(int id);
        public Task<int> CreateUser(User user);
        public Task<int> UpdateUser(User user);
        public Task<int> DeleteUser(int id);
        public Task<AttendanceRecord> GetAttendance(int employeeId, DateTime date);
        public Task<List<AttendanceRecord>> GetAttendanceByMonthId(int monthId);
        public Task<List<Dictionary<string, object>>> GetMonthlyAttendancePivotAsListAsync(int monthId);
        public Task SaveAttendance(List<AttendanceRecord> attendanceData);
        public Task UpdateAttendance(List<AttendanceRecord> attendanceData);
        public Task<int> CreateAttendenceMaster(AttendanceMaster attendence);
        public Task<int> DeleteAttendenceMaster(int id);
        public Task<List<AttendanceMaster>> GetAttendenceMaster();
        public Task<AttendanceMaster> GetAttenceMasterById(int id);
        public Task<int> UpdateAttendenceMaster(AttendanceMaster attendence);
        public Task<List<AttendanceRecord>> GetAttendanceByMonthAndMaster(int monthId, int attendenceMasterId);

    }
}
