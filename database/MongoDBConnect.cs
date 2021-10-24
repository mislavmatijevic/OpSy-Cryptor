using MongoDB.Bson;
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

        private MongoDBConnect()
        {
            string mongoPass = ConfigurationManager.AppSettings.Get("MongoDbPass");
            DbClient = new MongoClient($"mongodb+srv://OpSy-Cryptor:{mongoPass}@opsy.yn6sn.mongodb.net/OpSyDB?retryWrites=true&w=majority");
        }

        private static MongoDBConnect instance;

        public static MongoDBConnect GetInstance()
        {
            if (instance is null)
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

            await collections.InsertOneAsync(new User { Username = username, Salt = salt, Password = Convert.ToBase64String(EncryptionClass.GetHashSHA256(Encoding.ASCII.GetBytes(salt + password))), PubKey = EncryptionClass.GetInstance().GetPublicKey });
            return collections.AsQueryable().Where(sb => sb.Username == username).ToList()[0];
        }

        public async Task<User> GetUserByIdAsync(string userObjectID)
        {
            IMongoDatabase db = DbClient.GetDatabase("OpSyDB");
            IMongoCollection<User> collections = db.GetCollection<User>("users");
            IAsyncCursor<User> received = await collections.FindAsync(sb => sb.Id == ObjectId.Parse(userObjectID));
            List<User> listWithUser = await received.ToListAsync();

            if (listWithUser.Count == 0)
            {
                throw new Exception("Korisnik ne postoji!");
            }

            return listWithUser[0];
        }

        private async Task<User> GetUserAsync(string username)
        {
            IMongoDatabase db = DbClient.GetDatabase("OpSyDB");
            IMongoCollection<User> collections = db.GetCollection<User>("users");
            IAsyncCursor<User> received = await collections.FindAsync(sb => sb.Username == username);
            List<User> listWithUser = await received.ToListAsync();

            if (listWithUser.Count == 0)
            {
                throw new Exception("Korisnik ne postoji!");
            }

            return listWithUser[0];
        }

        public async Task<User> LoginUserAsync(string username, string password)
        {
            User foundUser = await GetUserAsync(username);

            if (foundUser.Password != Convert.ToBase64String(EncryptionClass.GetHashSHA256(Encoding.ASCII.GetBytes(foundUser.Salt + password))))
            {
                throw new Exception("Pogrešna lozinka!");
            }

            return foundUser;
        }

        public async Task<List<User>> GetUsersFromDBAsync(string username)
        {
            IMongoDatabase db = DbClient.GetDatabase("OpSyDB");
            IMongoCollection<User> collections = db.GetCollection<User>("users");

            FilterDefinition<User> filter = Builders<User>.Filter.Regex(user => user.Username, username);

            List<User> received = await collections.Aggregate().Match(filter).ToListAsync();
            return received;
        }
    }
}
