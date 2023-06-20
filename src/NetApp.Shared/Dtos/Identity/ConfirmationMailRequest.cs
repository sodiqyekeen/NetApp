namespace NetApp.Application.Dtos.Identity;

public class ConfirmationMailRequest
{
	public ConfirmationMailRequest(string userId)
	{
		UserId=userId;
	}
    public string UserId { get; set; }
}
