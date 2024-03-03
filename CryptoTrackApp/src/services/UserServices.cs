using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTrackApp.src.models;
using CryptoTrackApp.src.db;

namespace CryptoTrackApp.src.services
{
    public class UserServices : IUserServices
    {
	IRepository repository = new PostgreRepository();

	public async  Task<AppResponse> LoginUser(string pPassword, string pEmail) {
	  

	  try
	  {
	    User? user = await repository.Login(pEmail, pPassword);
	    
	    if (user == null) {
	      return new AppResponse(status: "Failure", message: "The proportioned email is not registered.");
	    }

	    if (user.Password != pPassword) {
	      return new AppResponse(status: "Failure", message: "Incorret password.");
	    }
	    
	    return new AppResponse(status: "Success", message: "Login successful.");

	  }
	  catch (Exception error) {
	    Console.WriteLine(error.Message);
	    return new AppResponse(status: "Failure", message: error.Message);
	  }

	}
    
	/// <summary>
	/// Saves a user in the database.
	/// </summary>
	/// <param name="pEmail">User's email</param>
	/// <param name="pPassword">User's password</param>
	/// <param name="pUserName">User's username</param>
	/// <param name="pBirthDate">User's birth date</param>
	/// <returns></returns>
	public async Task AddUserAsync(string pEmail, string pPassword, string pUserName, DateTime pBirthDate)
	{

	  User newUser = new User();
	  newUser.Email = pEmail;
	  newUser.Password = pPassword;
	  newUser.UserName = pUserName;                                                          
	  newUser.BirthDate = pBirthDate;
	  newUser.Id = Guid.NewGuid();
	  newUser.Status = true;
	  
	  try
	  {
		await repository.AddUserAsync(newUser);
	  }
	  catch (Exception error)
	  {
		throw new Exception(error.Message);
	  }
	}
	


	public async Task<bool> IsEmailAvailable(string pEmail)
	{	
	  try {

	    bool res = await repository.ExistEmail(pEmail);
	    return !res;

	  } 
	  catch (Exception error) {
	    Console.WriteLine("Error at CryptoTrackApp.src.services.UserServices: "+ error.Message);
	    throw new Exception(error.Message);
	  }
	}
  }
}
