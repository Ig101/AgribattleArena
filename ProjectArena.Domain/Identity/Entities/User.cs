using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;

namespace ProjectArena.Domain.Identity.Entities
{
    public class User : MongoUser
    {
        public string ViewName { get; set; }
    }
}