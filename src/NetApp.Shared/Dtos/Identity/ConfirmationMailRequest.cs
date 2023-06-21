namespace NetApp.Dtos;

public class ConfirmationMailRequest
{
	public ConfirmationMailRequest(string userId)
	{
		UserId=userId;
	}
    public string UserId { get; set; }
}
