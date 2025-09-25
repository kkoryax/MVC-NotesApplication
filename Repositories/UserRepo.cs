using NoteFeature_App.Data;
using NoteFeature_App.Models.DTO;
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
        bool AddUser(RegisterDto? registerDTO);
        bool ValidateUser(string email, string password);

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
            throw new NotImplementedException();
        }

        public List<UserModel> GetUserByID(Guid? userId)
        {
            if (userId == null)
            {
                return null;
            }

            return _db.Users.Where(u => u.UserId == userId && u.FlagActive == true).ToList();
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
                FlagActive = true
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return true;
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
    }
}
