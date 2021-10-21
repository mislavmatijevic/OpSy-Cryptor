using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OpSy_Cryptor.model
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
        public string PubKey { get; set; }
        public string PrivateKey { get; }
        public string SecretKey { get; }
    }
}
