using MongoDB.Driver;
using OpSy_Cryptor.common;
using OpSy_Cryptor.model;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace OpSy_Cryptor.database
{
    internal class MongoDBConnect
    {
        private MongoClient DbClient { get; set; }

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
            IMongoDatabase db = DbClient.GetDatabase("OpSyDB");
            IMongoCollection<User> collections = db.GetCollection<User>("users");

            string salt = Encryption.GetSalt();

            Encryption.Object.GenerateKeys();

            await collections.InsertOneAsync(new User { Username = username, Salt = salt, Password = Encryption.GetHashSHA256(salt + password), PubKey = Encryption.Object.PublicKeyString });
            return collections.AsQueryable().Where(sb => sb.Username == username).ToList()[0];
        }

        private async Task<User> GetUserFromDBAsync(string username, string password)
        {
            IMongoDatabase db = DbClient.GetDatabase("OpSyDB");
            IMongoCollection<User> collections = db.GetCollection<User>("users");
            System.Collections.Generic.List<User> userObject = await collections.Find(sb => sb.Username == username).ToListAsync();

            if (userObject.Count == 0)
            {
                throw new Exception("Korisnik ne postoji!");
            }

            if (userObject[0].Password != Encryption.GetHashSHA256(userObject[0].Salt + password))
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
