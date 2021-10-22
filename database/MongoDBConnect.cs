using MongoDB.Driver;
using OpSy_Cryptor.common;
using OpSy_Cryptor.model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpSy_Cryptor.database
{
    public class MongoDBConnect
    {
        private MongoClient DbClient { get; set; }

        public MongoDBConnect()
        {
            string mongoPass = ConfigurationManager.AppSettings.Get("MongoDbPass");
            DbClient = new MongoClient($"mongodb+srv://OpSy-Cryptor:{mongoPass}@opsy.yn6sn.mongodb.net/OpSyDB?retryWrites=true&w=majority");
        }

        private static MongoDBConnect instance;

        public static MongoDBConnect GetInstance()
        {
            if (instance is not null)
            {
                instance = new();
            }
            return instance;
        }

        /*
         * Registers user, returns ObjectID of new user on success.
        */
        public async Task<User> RegisterUserAsync(string username, string password)
        {
            IMongoDatabase db = DbClient.GetDatabase("OpSyDB");
            IMongoCollection<User> collections = db.GetCollection<User>("users");

            string salt = EncryptionClass.GetSalt();

            EncryptionClass.GetInstance().GenerateKeys();

            await collections.InsertOneAsync(new User { Username = username, Salt = salt, Password = EncryptionClass.GetHashSHA256(Encoding.ASCII.GetBytes(salt + password)), PubKey = EncryptionClass.GetInstance().GetPublicKey });
            return collections.AsQueryable().Where(sb => sb.Username == username).ToList()[0];
        }

        private async Task<User> GetUserFromDBAsync(string username, string password)
        {
            IMongoDatabase db = DbClient.GetDatabase("OpSyDB");
            IMongoCollection<User> collections = db.GetCollection<User>("users");
            IAsyncCursor<User> received = await collections.FindAsync(sb => sb.Username == username);
            List<User> listWithUser = await received.ToListAsync();

            if (listWithUser.Count == 0)
            {
                throw new Exception("Korisnik ne postoji!");
            }

            if (listWithUser[0].Password != EncryptionClass.GetHashSHA256(Encoding.ASCII.GetBytes(listWithUser[0].Salt + password)))
            {
                throw new Exception("Pogrešna lozinka!");
            }

            return listWithUser[0];
        }

        public async Task<List<User>> GetUsersFromDBAsync(string username)
        {
            IMongoDatabase db = DbClient.GetDatabase("OpSyDB");
            IMongoCollection<User> collections = db.GetCollection<User>("users");
            IAsyncCursor<User> received = await collections.FindAsync(user => user.Username == username);
            return await received.ToListAsync();
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
