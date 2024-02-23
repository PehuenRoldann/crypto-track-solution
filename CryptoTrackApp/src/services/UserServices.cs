using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoTrackApp.src.models;
using CryptoTrackApp.src.db;

namespace CryptoTrackApp.src.services
{
    public class UserServices : IUserServices
    {
	IRepository repository = new Repository();

        //public  AppResponse LoginUser(string pPassword, string pEmail)
        //{
	    
            //AppResponse response = new AppResponse();
            //response.status = "Failure";
	    //response.message = "Login was wrong";
            //return response;
	//}
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

	public AppResponse AddUser(string pEmail, string pPassword, string pUserName, DateTime pBirthDate)
	{
	  AppResponse response = new AppResponse();

	  CryptoTrackAppContext context = new CryptoTrackAppContext();
	  User newUser = new User();
	  newUser.Email = pEmail;
	  newUser.Password = pPassword;
	  newUser.UserName = pUserName;
	  newUser.BirthDate = pBirthDate;
	  newUser.Id = Guid.NewGuid();
	  newUser.Status = true;

	  try{
	    repository.AddUser(newUser);
	    response.status = "Success";
	    response.message = "User created.";

	  } catch (Exception error) {
	    
	    response.status = "Failure";
	    response.message = error.Message;

	  }

	  return response;
	}

	public async Task<AppResponse> IsEmailAvailable(string pEmail)
	{
	  AppResponse response = new AppResponse();
	  if (await repository.ExistEmail(pEmail)) 
	  {
	    response.status = "Failure";
	    response.message = "The email is already registered";
	    return response;
	  }

	  response.status = "Success";
	  response.message = "The email es available";
	  return response;
	}
  }
}
