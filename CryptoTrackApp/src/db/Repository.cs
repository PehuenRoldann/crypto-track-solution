using System;
using System.Threading.Tasks;
using System.Linq;
using Npgsql;
using CryptoTrackApp.src.models;
using Microsoft.EntityFrameworkCore;

namespace CryptoTrackApp.src.db {
  public class Repository : IRepository {
    
    public async Task<User?> Login (string pEmail, string pPassword) {

      try {
	using (var context = new CryptoTrackAppContext()) {
	return await context.Users.FirstOrDefaultAsync(u => u.Email == pEmail);

	}
      } 
      catch (Exception error) {
	  Console.WriteLine(error.Message);
	  throw new Exception(
	      "An unexpected error has happend while loggin. Try Again. \n" +
	      "If the problem persist, contact the support."
	      );

      }
    }
    public void AddUser(User pUser) {
      
      try
      {

	using (var context = new CryptoTrackAppContext()) {
	  context.Add(pUser);
	  context.SaveChanges();
	}

      }
      catch (Exception error)
      {
	Console.WriteLine("~~~~~~~~~~~~~~~~~~Error: " + error.Message);
	throw new Exception(error.Message);
      }

    }

    public async Task<bool> ExistEmail(string pEmail) {
      
      try 
      {
	using (var context = new CryptoTrackAppContext())
	{
	  return await context.Users.AnyAsync(u => u.Email == pEmail);
	}
      }
      catch (Exception error)
      {
	throw new Exception(error.Message);
      }
    }

  }
}
