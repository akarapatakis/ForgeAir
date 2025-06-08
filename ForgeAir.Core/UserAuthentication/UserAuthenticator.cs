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
    public class UserAuthenticator
    {
        private readonly ForgeAirDbContext _dbContext;
        public UserAuthenticator()
        {
            _dbContext = new ForgeAirDbContext();
        }
        private static bool VerifyPassword(string inputPassword, string storedHashBase64)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHashBase64);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(inputPassword, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] inputHash = pbkdf2.GetBytes(32);

            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != inputHash[i])
                    return false;
            }

            return true;
        }

        public static async Task<bool> isUserBlocked(User user)
        {
            if (user.Role == Database.Models.Enums.UserRole.Blocked)
            {
                return true;
            }
            return false;
        }
        public async Task<User?> Authenticate(User user)
        {
            User? _user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (_user != null && VerifyPassword(user.Password, user.Password))
            {
                return _user;
            }
            else
            {
                return null;
            }

            return null;
        }
    }
}
