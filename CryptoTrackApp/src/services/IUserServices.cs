using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTrackApp.src.services
{
    public interface IUserServices
    {
        /*
        Returns a AppResponse with status 200 if the login was successful,
        or 40x if there is a problem with the login.
        */
        /// <summary>
        /// Login an user if it's registered in the database.
        /// </summary>
        /// <param name="pPassword">User's password to login.</param>
        /// <param name="pEmail">User's email password to login.</param>
        /// <returns>
        /// Returns a tuple (int, string). The string is the result message.
        /// The value of the int represents the result:
        /// -1 = unexpected error.
        /// 0 = email not registered.
        /// 1 = incorrect password
        /// 2 = Successful login (the message is the user id).
        /// </returns>
        Task<(int, string)> LoginUser(String pPassword, String pEmail);
	    Task AddUserAsync(string pEmail, string pPassword, string pUserName, DateTime pBirthDate);
	    Task<bool> IsEmailAvailable(string pEmail);
    }
}

