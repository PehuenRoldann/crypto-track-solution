using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTrackApp.src.models;
using CryptoTrackApp.src.db;
using CryptoTrackApp.src.utils;

namespace CryptoTrackApp.src.services
{
    public class UserServices : IUserServices
    {
	IRepository repository = new PostgreRepository();
	Logger _logger = new Logger();

	/// <summary>
	/// Login an user if it's registered in the database.
	/// </summary>
	/// <param name="pPassword">User's password to login.</param>
	/// <param name="pEmail">User's email password to login.</param>
	/// <returns>
	/// A string with the user's Id if the loggin is success. \n
	/// Null if there is not user with the given email.
	/// </returns>
	/// <exception cref="InvalidOperationException"> If the passwords doesn't match. </exception>
	/// <exception cref="Exception">If there has been an unexpected error while login.</exception>
	public async Task<(int, string)> LoginUser(string pPassword, string pEmail)
	{
	  _logger.Log("[EXEC - Operation LoginUser at UserServices]");
	  string opResult = "";
	  int opValue = -1;

	  try
	  {
	    User? user = await repository.GetUserAsync(pEmail);

		if (user == null) {
			opValue = 0;
			opResult = "There is no user registered with that email.";
		}
		else if (user.Password != pPassword) {
			opValue = 1;
			opResult = "The password is wrong.";
		}
		else {
			opValue = 2;
			opResult = user.Id.ToString();
		}

		_logger.Log($"[RESULT - Operation LoginUser at UserServices - Result: [opValue: {opValue}; opResult: {opResult}]]");

	  }
	  catch (Exception error)
	  {
		_logger.Log($"[ERROR - Operation LoginUser at UserServices - Message: {error.Message}]");
		opResult = "Unexpected error while login, try again or contact to support.";
	  }

	  return (opValue, opResult);

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
		_logger.Log($"[ERROR - IsEmailAvailable at UserServices - message: {error.Message}]");
	    throw new Exception(error.Message);
	  }
	}
  }
}
