using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ForgeAir.Core.UserAuthentication
{
    class UserCreator
    {
        private readonly ForgeAirDbContext _dbContext;

        private readonly string _username;
        private readonly string _fullname;
        private string? _password;
        private User _user;
        public UserCreator(string username, string fullname, string password, User executor)
        {
            if (executor.Role != Database.Models.Enums.UserRole.Administrator)
            {
                throw new Exception("Insufficient priviliges.");
            }

            if (username == null || fullname == null || password == null)
            {
                throw new ArgumentNullException(nameof(username));
            }
            _username = username;
            _fullname = fullname;
            _password = password;
            _dbContext = new ForgeAirDbContext();
        }

        public string HashedPassword
        {
            get
            {
                using (var rng = RandomNumberGenerator.Create())
                {
                    byte[] salt = new byte[16];
                    rng.GetBytes(salt);

                    var pbkdf2 = new Rfc2898DeriveBytes(_password, salt, 100_000, HashAlgorithmName.SHA256);
                    byte[] hash = pbkdf2.GetBytes(32);

                    byte[] hashBytes = new byte[48];
                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16, 32);

                    return Convert.ToBase64String(hashBytes);
                }
            }
        }

        private void CreateUser()
        {
            string hashed = HashedPassword;
            if (hashed == null)
            {
                throw new Exception("Password encryption failed.");
            }

            _user = new User
            {
                Username = _username,
                FullName = _fullname,
                Password = hashed
            };

            _password = null;
        }

        private async Task AddToDb(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Create()
        {
            CreateUser();
            await Task.Run(() => AddToDb(_user));
        }
    }
}
