using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoTrackApp.src.services
{
    public class UserServices : IUserServices
    {
        public  AppResponse LoginUser(string pPassword, string pEmail)
        {
	    
            AppResponse response = new AppResponse();
            response.status = 400;
            response.data = new Dictionary<string, string>();
            response.data["message"] = "Email not registered.";
            return response;
       }
    }
}
