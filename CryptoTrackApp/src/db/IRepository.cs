using System.Threading.Tasks;
using CryptoTrackApp.src.models;

namespace CryptoTrackApp.src.db {

  public interface IRepository {

    object[] AddUser(User pUser);
    Task<User?> Login(string pEmail, string pPassword);
    Task<bool> ExistEmail(string pEmail);
    
  }
}
