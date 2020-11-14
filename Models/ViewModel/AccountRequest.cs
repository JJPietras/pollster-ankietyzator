using System.Security.Cryptography;
using System.Text;

namespace Ankietyzator.Models.ViewModel
{
    public class AccountRequest
    {
        public string UserName { get; set; }
        public string EMail { get; set; }
        public string Password { get; set; }
        
        public byte[] GetHash()
        {
            using HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(Password));
        }
    }
}