using System.Threading.Tasks;
using CryptoTrackApp.src.models;

namespace CryptoTrackApp.src.db {

  public interface IRepository {

    void AddUser(User pUser);
    User? Login(string pEmail, string pPassword);
    Task<bool> ExistEmail(string pEmail);
    
  }
}
