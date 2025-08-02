using Server.Domain.Abstractions;

namespace Server.Domain.Users;

public static class AddressErrors
{
    public static readonly Error AddressIncomplete = new(
        "Address.AddressIncomplete",
        "All address fields (Country, City, ZipCode, Street) must be provided when updating address"
    );
}
 
