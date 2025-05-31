using Contracts;
using Dapper;
using Entities;
using Entities.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class UserRepository:IUserContract
    {
        private readonly BgfclContext _context;

        public UserRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUser()
        {
            var query = "  select * from users";
            using (var connection = _context.CreateConnection())
            {
                var users = await connection.QueryAsync<User>(query);
                return users.ToList();
            }
        }

        public async Task<User> GetUserById(int id)
        {
            var query = "select * from users where UserId = @id";
            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QueryFirstOrDefaultAsync<User>(query, new { id });

                return user; 
            }
        }


        public async Task<int> CreateUser(User user)
        {
            int result = 0;
            var query = "INSERT INTO users (UuId,Guest_pk_Id,School_Id,Username,Password,Email,IsActive,CreatedBy,CreatedDate) VALUES (@uuId,@guest_pk_id,@school_id,@username,@password,@email,@isActive,@createdBy,@createdDate)";
            var parameters = new DynamicParameters();
            parameters.Add("uuId", user.UuId, DbType.Int32);
            parameters.Add("guest_pk_id", user.GuestPkId, DbType.Int32);
            parameters.Add("school_id", user.SchoolId, DbType.Int32);
            parameters.Add("isActive", 1);
            parameters.Add("username", user.Username, DbType.String);
            parameters.Add("password", user.Password, DbType.String);
            parameters.Add("email", user.Email, DbType.String);
            parameters.Add("createdBy", user.Username, DbType.String);
            parameters.Add("createdDate", DateTime.Now, DbType.DateTime);
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

        public async Task<int> UpdateUser(User user)
        {
            var query = "UPDATE users SET UuId = @uuId,Guest_pk_Id=@guest_pk_Id,School_Id=@school_Id,Username=@username,Password=@password, Email = @email, IsActive = @isActive, updatedby = @updatedby, updateddate = @updateddate WHERE UserId = @id";
            var parameters = new DynamicParameters();
            parameters.Add("uuid", user.UuId, DbType.Int32);
            parameters.Add("guest_pk_id", user.GuestPkId, DbType.Int32);
            parameters.Add("school_id", user.SchoolId, DbType.Int32);
            parameters.Add("username", user.Username, DbType.String);
            parameters.Add("password", user.Password, DbType.String);
            parameters.Add("email", user.Email, DbType.String);
            parameters.Add("isActive", user.IsActive, DbType.Boolean);
            parameters.Add("updatedby", user.Username, DbType.String);
            parameters.Add("updateddate", DateTime.Now);
            parameters.Add("id", user.UserId, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return result; // This returns the number of rows affected
            }
        }
        public async Task<int> DeleteUser(int id)
        {
            var query = "update users set isactive = 0 where userId = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }
        // Attendence ========
        public async Task<AttendanceRecord> GetAttendance(int employeeId, DateTime date)
        {
            var query = @"SELECT * FROM AttendanceRecords 
                  WHERE EmployeeId = @employeeId AND CAST(AttendanceDate AS DATE) = @date";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<AttendanceRecord>(query, new { employeeId, date = date.Date });
            }
        }
        public async Task<List<AttendanceRecord>> GetAttendanceByMonthId(int monthId)
        {
            var query = @"SELECT * FROM AttendanceRecords 
                  WHERE MonthId = @monthId";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<AttendanceRecord>(query, new { monthId });
                return result.ToList();
            }
        }


        public async Task SaveAttendance(List<AttendanceRecord> attendanceData)
        {
            if (attendanceData == null || !attendanceData.Any())
                return;

            string deleteQuery = @"
            DELETE FROM AttendanceRecords
            WHERE AttendenceMasterId = @AttendenceMasterId AND MonthId = @MonthId;";

            string insertQuery = @"
            INSERT INTO AttendanceRecords 
            (AttendenceMasterId, MonthId, EmployeeId, JobCode, EmployeeName, DayCount, AttendanceDate, IsPresent)
            VALUES 
            (@AttendenceMasterId, @MonthId, @EmployeeId, @JobCode, @EmployeeName, @DayCount, @AttendanceDate, @IsPresent);";

                using (var connection = _context.CreateConnection())
                {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var firstRecord = attendanceData.First();

                        // Step 1: Delete existing records for the same month and attendance master
                        await connection.ExecuteAsync(deleteQuery, new
                        {
                            AttendenceMasterId = firstRecord.AttendenceMasterId,
                            MonthId = firstRecord.MonthId
                        }, transaction);

                        // Step 2: Insert all new attendance records
                        foreach (var record in attendanceData)
                        {
                            await connection.ExecuteAsync(insertQuery, record, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task UpdateAttendance(List<AttendanceRecord> attendanceData)
        {
            if (attendanceData == null || !attendanceData.Any())
                return;

            string upsertQuery = @"
                IF EXISTS (
                    SELECT 1 FROM AttendanceRecords
                    WHERE AttendenceMasterId = @AttendenceMasterId AND MonthId = @MonthId AND EmployeeId = @EmployeeId AND AttendanceDate = @AttendanceDate
                )
                BEGIN
                    UPDATE AttendanceRecords
                    SET 
                        JobCode = @JobCode,
                        EmployeeName = @EmployeeName,
                        DayCount = @DayCount,
                        IsPresent = @IsPresent
                    WHERE 
                        AttendenceMasterId = @AttendenceMasterId AND 
                        MonthId = @MonthId AND 
                        EmployeeId = @EmployeeId AND 
                        AttendanceDate = @AttendanceDate;
                END
                ELSE
                BEGIN
                    INSERT INTO AttendanceRecords 
                    (AttendenceMasterId, MonthId, EmployeeId, JobCode, EmployeeName, DayCount, AttendanceDate, IsPresent)
                    VALUES 
                    (@AttendenceMasterId, @MonthId, @EmployeeId, @JobCode, @EmployeeName, @DayCount, @AttendanceDate, @IsPresent);
                END";

            using (var connection = _context.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var record in attendanceData)
                        {
                            await connection.ExecuteAsync(upsertQuery, record, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }



        public async Task<int> CreateAttendenceMaster(AttendanceMaster attendence)
        {
            int result = 0;

            var query = @"INSERT INTO AttendanceMaster 
                  (MonthId, StartDate, EndDate, Created_At, Is_Active) 
                  VALUES (@MonthId, @StartDate, @EndDate, @CreatedAt, @IsActive)";

            var parameters = new DynamicParameters();
            parameters.Add("@MonthId", attendence.MonthId, DbType.Int32);
            parameters.Add("@StartDate", attendence.StartDate, DbType.Date);
            parameters.Add("@EndDate", attendence.EndDate, DbType.Date);
            parameters.Add("@CreatedAt", DateTime.Now, DbType.DateTime);
            parameters.Add("@IsActive", 1, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync(query, parameters);
                }
                catch (Exception ex)
                {
                    // Log or rethrow exception if needed
                    throw new Exception("Error inserting attendance master", ex);
                }
            }

            return result;
        }


        public async Task<int> DeleteAttendenceMaster(int id)
        {
            var attendanceRecordsQuery = "DELETE FROM AttendanceRecords WHERE AttendenceMasterId = @id";
            var masterQuery = "DELETE FROM AttendanceMaster WHERE Id = @id";

            using (var connection = (SqlConnection)_context.CreateConnection())
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(attendanceRecordsQuery, new { id }, transaction);
                        var result = await connection.ExecuteAsync(masterQuery, new { id }, transaction);

                        transaction.Commit();
                        return result;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

        }



        public async Task<List<AttendanceMaster>> GetAttendenceMaster()
        {
            var query = "SELECT Id, MonthId, StartDate, EndDate, Is_Active AS IsActive FROM AttendanceMaster;";
            using (var connection = _context.CreateConnection())
            {
                var results = await connection.QueryAsync<AttendanceMaster>(query);
                return results.ToList();
            }
        }



        public async Task<AttendanceMaster> GetAttenceMasterById(int id)
        {
            var query = " SELECT * FROM AttendanceMaster where Id = @id;";
            using (var connection = _context.CreateConnection())
            {
                var attendence = await connection.QueryFirstOrDefaultAsync<AttendanceMaster>(query, new { id });

                return attendence;
            }
        }

        public async Task<int> UpdateAttendenceMaster(AttendanceMaster attendence)
        {
            var query = @"UPDATE AttendanceMaster 
                  SET MonthId = @MonthId, 
                      TotalDay = @TotalDay,
                      StartDate = @StartDate, 
                      EndDate = @EndDate, 
                      Is_Active = @IsActive, 
                      Updated_At = @UpdatedAt 
                  WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("@MonthId", attendence.MonthId, DbType.Int32);
            parameters.Add("@TotalDay", attendence.TotalDay, DbType.Int32);
            parameters.Add("@StartDate", attendence.StartDate, DbType.Date);
            parameters.Add("@EndDate", attendence.EndDate, DbType.Date);
            parameters.Add("@IsActive", attendence.IsActive, DbType.Boolean);
            parameters.Add("@UpdatedAt", DateTime.Now, DbType.DateTime);
            parameters.Add("@Id", attendence.Id, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return result; // number of rows affected
            }
        }

        public async Task<List<AttendanceRecord>> GetAttendanceByMonthAndMaster(int monthId, int attendenceMasterId)
        {
            var query = "SELECT * FROM AttendanceRecords WHERE MonthId = @monthId AND AttendenceMasterId = @attendenceMasterId;";

            using (var connection = _context.CreateConnection())
            {
                var attendanceRecords = await connection.QueryAsync<AttendanceRecord>(query, new { monthId, attendenceMasterId });
                return attendanceRecords.ToList();
            }
        }
        public async Task<List<Dictionary<string, object>>> GetMonthlyAttendancePivotAsListAsync(int monthId)
        {
            var result = new List<Dictionary<string, object>>();

            using (var connection = _context.CreateConnection())
            {
                connection.Open();

                // Step 1: Get dynamic column headers based on distinct AttendanceDate
                const string columnQuery = @"
            SELECT STRING_AGG(QUOTENAME(CONVERT(VARCHAR(10), AttendanceDate, 5)), ',') AS ColList
            FROM (
                SELECT DISTINCT AttendanceDate
                FROM AttendanceRecords
                WHERE MonthId = @MonthId
            ) AS x";

                string columnList;
                using (var cmd = new SqlCommand(columnQuery, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@MonthId", monthId);
                    columnList = (string)(await cmd.ExecuteScalarAsync() ?? "");
                }

                if (string.IsNullOrWhiteSpace(columnList))
                    return result; // No attendance records for the month, return empty list

                // Step 2: Construct pivot query
                string sql = $@"
            SELECT EmployeeName, JobCode, {columnList}
            FROM (
                SELECT 
                    EmployeeName,
                    JobCode,
                    CONVERT(VARCHAR(10), AttendanceDate, 5) AS AttendanceDate,
                    DayCount
                FROM AttendanceRecords
                WHERE MonthId = @MonthId
            ) AS src
            PIVOT (
                MAX(DayCount)
                FOR AttendanceDate IN ({columnList})
            ) AS pvt
            ORDER BY EmployeeName, JobCode";

                // Step 3: Execute pivot query
                using (var cmd = new SqlCommand(sql, (SqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@MonthId", monthId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                            }

                            result.Add(row);
                        }
                    }
                }
            }

            return result;
        }


    }
}
