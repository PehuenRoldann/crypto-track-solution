using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


public struct AppResponse{
    public string status;
    public string message;
    public Object? data;

    public AppResponse (string status, string message, Object? data = null){
      this.status = status;
      this.message = message;
      this.data = data;
    }
}

namespace CryptoTrackApp.src.services
{
    public interface IUserServices
    {
        /*
        Returns a AppResponse with status 200 if the login was successful,
        or 40x if there is a problem with the login.
        */
        AppResponse LoginUser(String pPassword, String pEmail);
	AppResponse AddUser(string pEmail, string pPassword, string pUserName, DateTime pBirthDate);
	Task<AppResponse> IsEmailAvailable(string pEmail);
    }
}

