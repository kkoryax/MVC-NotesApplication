using Microsoft.EntityFrameworkCore;
using NoteFeature_App.Data;
using NoteFeature_App.Models.DTO;
using NoteFeature_App.Models.Note;
using NoteFeature_App.Models.User;
using Services.Helpers;
using System.ComponentModel.DataAnnotations;

namespace NoteFeature_App.Repositories
{
    //service contracts
    public interface IUserRepo
    {
        //Action Method List
        List<UserModel> GetAllUser();
        List<UserModel> GetUserByID(Guid? userId);
        UserModel? GetUserByEmail(string email);
        void DeleteUser(Guid? userId);
        bool AddUser(RegisterDto? registerDTO);
        bool ValidateUser(string email, string password);
        UserPagination GetListUserPagination(UserPagination pagination);

    }

    public class UserRepo : IUserRepo
    {
        #region DBSetting
        private readonly ApplicationDBContext _db;
        //DB Constructor
        public UserRepo(ApplicationDBContext db)
        {
            _db = db;
        }
        #endregion
        public List<UserModel> GetAllUser()
        {
            return _db.Users.Where(n => n.FlagActive == true).ToList();
        }

        public List<UserModel> GetUserByID(Guid? userId)
        {
            if (userId == null)
            {
                return null;
            }

            return _db.Users.Where(u => u.UserId == userId && u.FlagActive == true).ToList();
        }

        public UserModel? GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            return _db.Users.FirstOrDefault(u => u.Email == email && u.FlagActive == true);
        }

        public bool AddUser(RegisterDto? registerDTO)
        {
            if (registerDTO == null)
            {
                throw new ArgumentNullException(nameof(registerDTO));
            }

            ValidationHelper.ModelValidation(registerDTO);

            var check_unique_user = _db.Users.FirstOrDefault(s => s.Email == registerDTO.Email && s.FlagActive == true);

            if (check_unique_user != null)
            {
                return false;
            };

            var user = new UserModel
            {
                UserId = Guid.NewGuid(),
                Email = registerDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password),
                CreatedAt = DateTime.Now,
                Role = registerDTO.Role,
                FlagActive = true
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return true;
        }
        public void DeleteUser(Guid? userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            //GET NOTE BY ID
            UserModel? user_find_by_id = _db.Users.FirstOrDefault(n => n.UserId == userId);

            if (user_find_by_id == null)
            {
                throw new ArgumentException("ไม่พบผู้ใช้ที่ต้องการลบ");
            }

            //SOFT DELETE
            user_find_by_id.FlagActive = false;

            _db.Users.Update(user_find_by_id);
            _db.SaveChanges();
        }

        public bool ValidateUser(string email, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email && u.FlagActive == true);
            if (user == null)
            {
                return false;
            }
            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }

        public UserPagination GetListUserPagination(UserPagination pagination)
        {
            UserPagination Users = new UserPagination();

            var perPage = pagination.PerPage;
            var skip = pagination.Offset;
            var search = pagination.Search ?? string.Empty;

            var sort = pagination.Sort ?? "CreatedAt desc";

            var fromDate = pagination.FromDate.Date;
            var toDate = pagination.ToDate.HasValue ? pagination.ToDate.Value.Date : DateTime.MaxValue.Date;

            var query = _db.Users
                            .AsQueryable();

            query = query.Where(n => n.FlagActive == true);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(n => n.Email.Contains(search));
            }
            if (fromDate != null)
            {
                query = query.Where(n => n.CreatedAt.Date >= fromDate);
            }
            if (toDate != DateTime.MaxValue.Date)
            {
                query = query.Where(n => n.CreatedAt.Date <= toDate);
            }

            // Order query
            // Order query
            if (sort == "CreatedAt desc")
            {
                query = query
                        .OrderByDescending(n => (n.UpdatedAt ?? n.CreatedAt));
            }
            else
            {
                query = query
                        .OrderBy(n => (n.UpdatedAt ?? n.CreatedAt));
            }

            Users.Total = query.Count();

            var result = query
                        .Skip(skip)
                        .Take(perPage)
                        .AsEnumerable()
                        .ToList();
            Users.Users = result;

            return Users;
        }

    }
}
