using System;
using System.Threading.Tasks;
using Xunit;
using CryptoTrackApp.src.services;
using RandomDataGenerator.Randomizers;
using RandomDataGenerator.FieldOptions;

namespace CryptoTrackTest {


public class UserServicesTest
{
   
    /* [Fact]
    public void AddUser()
    {
      var rand = new Random();

      var randomizerFirstName = RandomizerFactory.GetRandomizer(new FieldOptionsFirstName());
      string userName = randomizerFirstName.Generate();
      
      var randomizerEmail = RandomizerFactory.GetRandomizer(new FieldOptionsEmailAddress());
      string email = randomizerEmail.Generate();

      var randomizerPassword = RandomizerFactory
	.GetRandomizer(new FieldOptionsText
	    {
	      Min = 10,
	      Max = 20,
	      UseUppercase = true,
	      UseLowercase = true,
	      UseNumber = true,
	      UseSpecial = true
	    });

      String password = randomizerPassword.Generate();
      
      var randomizerBirthDate = RandomizerFactory.GetRandomizer(new FieldOptionsDateTime{
	    From = new DateTime(1980, 1, 1),
	    To = new DateTime(2006, 12, 31)
	    
	  });

      DateTime? birthDate = randomizerBirthDate.Generate();
    
      IUserServices services = new UserServices();

      AppResponse response = services.AddUser(email, password, userName, birthDate.Value);
      Console.WriteLine("-----App Response: " + response.message);
      Assert.Equal(response.status, "Success");


    }

    [Fact]
    public void EmailNotAvailable(){

      String email = "pehuen@gmail.com";

      IUserServices services = new UserServices();
      Task<AppResponse> response = services.IsEmailAvailable(email);
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine(response.Result.message);
      Console.ResetColor();
      Assert.True(response.Result.status == "Failure");
    }

    [Fact]
    public void EmailAvailable() {

      String email = "pehuenroldan@gmail.com";

      IUserServices services = new UserServices();
      Task<AppResponse> response = services.IsEmailAvailable(email);
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine(response.Result.message);
      Console.ResetColor();
      Assert.True(response.Result.status == "Success");
    } */
}

}
