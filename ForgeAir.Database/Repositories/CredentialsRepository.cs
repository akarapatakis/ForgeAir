using CredentialManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Database.Repositories
{
    public static class CredentialsRepository
    {

        public static void SavePassword(string dbName, string password)
        {
            using (var cred = new Credential())
            {
                cred.Password = password;
                cred.Target = dbName;
                cred.Type = CredentialType.Generic;
                cred.PersistanceType = PersistanceType.LocalComputer;
                cred.Save();
            }
        }

        public static string GetPassword(string dbName)
        {
            using (var cred = new Credential())
            {
                cred.Target = dbName;
                cred.Load();
                return cred.Password;
            }
        }
    }
}
