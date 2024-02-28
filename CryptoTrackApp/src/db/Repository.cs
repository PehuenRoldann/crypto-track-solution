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

    public object[] AddUser(User pUser) {
      object[] response = new object[2];
      try
      {

	using (var context = new CryptoTrackAppContext()) {
	  User newUser = context.Add(pUser).Entity;
	  context.SaveChanges();
	  response[1] = newUser;
	}

	response[0] = "Success";

      }
      catch (Exception error)
      {
	Console.WriteLine("~~~~~~~~~~~~~~~~~~Error: " + error.Message);
	response[0] = "Failure";
	response[1] = error.Message;
      }

      return response;
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
