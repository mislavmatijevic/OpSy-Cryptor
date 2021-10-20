using MongoDB.Driver;
using OpSy_Cryptor.common;
using OpSy_Cryptor.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace OpSy_Cryptor.database
{
    class MongoDBConnect
    {
        MongoClient DbClient { get; set; }

        public MongoDBConnect()
        {
            string mongoPass = ConfigurationManager.AppSettings.Get("MongoDbPass");
            DbClient = new MongoClient($"mongodb+srv://OpSy-Cryptor:{mongoPass}@opsy.yn6sn.mongodb.net/OpSyDB?retryWrites=true&w=majority");
        }

        /*
         * Registers user, returns ObjectID of new user on success.
        */
        public async Task<User> RegisterUserAsync(string username, string password)
        {
            var db = DbClient.GetDatabase("OpSyDB");
            var collections = db.GetCollection<User>("users");

            var salt = Encryption.GetSalt();

            await collections.InsertOneAsync(new User { Username = username, Salt = salt, Password = Encryption.GetPasswordHash(salt + password), PubKey = "123" });
            return collections.AsQueryable().Where(sb => sb.Username == username).ToList()[0];
        }

        private async Task<User> GetUserFromDBAsync(string username, string password)
        {
            var db = DbClient.GetDatabase("OpSyDB");
            var collections = db.GetCollection<User>("users");
            var userObject = await collections.Find(sb => sb.Username == username).ToListAsync();

            if (userObject.Count == 0)
            {
                throw new Exception("Korisnik ne postoji!");
            }

            if (userObject[0].Password != Encryption.GetPasswordHash(userObject[0].Salt + password))
            {
                throw new Exception("Pogrešna lozinka!");
            }

            return userObject[0];
        }

        /*
         * Logs-in user, returns ObjectID of new user on success.
        */
        public async Task<User> LoginUserAsync(string username, string password)
        {
            return await Task.Run(async () => await GetUserFromDBAsync(username, password));
        }
    }
}
