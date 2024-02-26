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
        
	/// <summary>
	/// This adds a new user to the database.
	/// </summary>
	/// <returns>
	/// A object[] where [0]: status of the query, [1]: message.
	/// </returns>
	public object[] AddUser(string pEmail, string pPassword, string pUserName, DateTime pBirthDate)
	{
	  //AppResponse response = new AppResponse();
          object[] response = new object[2];
	  CryptoTrackAppContext context = new CryptoTrackAppContext();
	  User newUser = new User();
	  newUser.Email = pEmail;
	  newUser.Password = pPassword;
	  newUser.UserName = pUserName;
	  newUser.BirthDate = pBirthDate;
	  newUser.Id = Guid.NewGuid();
	  newUser.Status = true;
	  object[] res = repository.AddUser(newUser);
	  response[0] = res[0];
	  
	  if (res[1].GetType() == typeof(User)){
	    response[1] = "User created succesfuly!";
	  }
	  else {
	    response[1] = res[1];
	  }

	  return response;
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
