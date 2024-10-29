using HostelDB.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace HostelDB.AlfaPruefungDb
{
    public class HostelDbContext : DbContext
    {
        public HostelDbContext(DbContextOptions<HostelDbContext> options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
        public DbSet<User> DbSetUser { get; set; }
        public DbSet<AmsUser> DbSetAmsUser { get; set; }
        public DbSet<ClaimTemplate> DbSetClaimTemplate { get; set; }
        public DbSet<Claim> DbSetClaim { get; set; }
        public DbSet<Post> DbSetPost { get; set; }
        public DbSet<LogMessageEntry> DbSetLogApplication { get; set; }
        public DbSet<UserRoom> DbSetUserRoom { get; set; }
        public DbSet<Room> DbSetRoom { get; set; }
        public DbSet<DutyOrder> DbSetDutyOrder { get; set; }
        public DbSet<Duty> DbSetDuty { get; set; }
        public DbSet<TelegramUser> DbSetTelegramUser { get; set; }

        public List<Claim> GetFilteringDate(string createDate)
        {
            var sql = $"SELECT * FROM Claim WHERE (CreateDate LIKE '%{createDate}%') ";

            var result = DbSetClaim.FromSqlRaw(sql).ToList();
            return result;
        }
        public bool IsClaimDeleteEnabled(int template)
        {
            var sql = $"SELECT * FROM Claim WHERE ClaimTemplateId = {template}";

            var result = DbSetClaim.FromSqlRaw(sql).Count();
            if (result == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public string GetRoomNumber(int userId)
        {
            //var sql = $"SELECT r.NumberRoom FROM [dbo].[UserRoom] ur, [dbo].[Room] r  WHERE ur.UserId = {userId} and r.RoomId = ur.RoomId";

            //var result = Helper<string>.ExecuteSQLQuery(this, sql, x => (string)x[0]).FirstOrDefault();



           var user = DbSetUserRoom.FirstOrDefault(x => x.UserId == userId);
           if (user == null)
           {
               throw new ArgumentException("User Is not find database!");
           }
           var numberRoom = DbSetRoom.FirstOrDefault(x=> x.RoomId == user.RoomId)?.NumberRoom;

            return numberRoom;
        }
     

        public string GetDataByTemplate(int userId, string table, string property)
        {
            var sql = $"SELECT {property} FROM {table} WHERE {table + "Id"} = {userId}";
            
            var result = Helper<string>.ExecuteSQLQuery(this,sql,x => (string) x[0]).FirstOrDefault();
            return result;
        }

        public void ClearUserRoom()
        {
            var sql = @"DELETE FROM UserRoom";
            Database.ExecuteSqlRaw(sql);
        }
        public void ClearOrder()
        {
            var sql = @"DELETE FROM DutyOrderList";
            Database.ExecuteSqlRaw(sql);
        }
        public void ClearUser()
        {
            var sql = @"delete from hostel.AspRefreshTokens;delete from hostel.AspNetUsers where UserId is not null; DELETE FROM hostel.User;";
            Database.ExecuteSqlRaw(sql);
        }
        public void ClearRoom()
        {
            var sql = @"DELETE FROM Room";
            Database.ExecuteSqlRaw(sql);
        }
        public void ClearTeleUsers()
        {
            var sql = @"DELETE FROM TelegramUser";
            Database.ExecuteSqlRaw(sql);
        }

        public List<Post> GetPostPageByStep(int step)
        {
            //var sql = $"SELECT * FROM Post ORDER BY CreateDate DESC OFFSET {step} ROWS FETCH NEXT 20 ROWS ONLY;";

            var result = DbSetPost.OrderByDescending(x => x.CreateDate).Skip(step).Take(20).ToList();
            
            //var result = DbSetPost.FromSqlRaw(sql).AsNoTracking().ToList();
            return result;
        }

        public List<Post> GetLastPost(int count)
        {
            //var sql = $"SELECT TOP {count} * FROM Post ORDER BY CreateDate DESC ";
            var result = DbSetPost.OrderByDescending(x => x.CreateDate).Take(count).ToList();
            //var result = DbSetPost.FromSqlRaw(sql).AsNoTracking().ToList();
            return result;
        }

        public DateTime? GetNearestDateDutyByRoom(string roomId)
        {
            // var sql = @$"SELECT MIN(Date) AS NextDate FROM Duty WHERE RoomNumber = '{roomId}' AND Date >= GETDATE();";
            // var result = Helper<DateTime?>.ExecuteSQLQuery(this,sql,x => (DateTime?) x[0]).FirstOrDefault();
            // return result
            var isAnyDuty = DbSetDuty.Where(x => x.RoomNumber == roomId && x.Date.Date >= DateTime.Now.Date).Any();
            if (isAnyDuty)
            {
                return DbSetDuty.Where(x => x.RoomNumber == roomId && x.Date.Date >= DateTime.Now.Date).Min(y => y.Date);
            }
            else
            {
                return null;
            }
        }

        public string? GetRoomNumberByUser(int userId)
        {
            var sql = @$"select r.NumberRoom from UserRoom ur, Room r where ur.UserId = {userId} and r.RoomId = ur.RoomId";

            var rooms = (from room in DbSetRoom
                join userR in DbSetUserRoom on room.RoomId equals userR.RoomId
                where userR.UserId == userId
                select room.NumberRoom).FirstOrDefault();


            //var result = Helper<string?>.ExecuteSQLQuery(this,sql,x => (string?) x[0]).FirstOrDefault();
            return rooms;
        }
    }

    public static class Helper<T>
    {
        public static List<T> ExecuteSQLQuery(DbContext context, string sql, Func<DbDataReader, T> map)
        {
            using (var cmd = context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                context.Database.OpenConnection();
                using (var result = cmd.ExecuteReader())
                {
                    var List = new List<T>();
                    while (result.Read())
                    {
                        List.Add(map(result));
                    }
                    return List;
                }
                context.Database.CloseConnection();
            }
            return null;
        }
    }


}