namespace NetApp.Models;

public interface IResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; }

}

public interface IResponse<T> : IResponse
{
    public T? Data { get; set; }
}

public class Response<T> : IResponse<T>
{
    public bool Succeeded { get; set; }
    public string Message { get; set; }=null!;
    public T? Data { get; set; }

    public static Response<T> Fail(string message) => new Response<T> { Succeeded = false, Message = message };

    public static Response<T> Success(T data, string message = "") => new Response<T> { Data = data, Succeeded = true, Message = message };
}

public class Response : IResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } =null!;
    public static Response Success(string message = "") => new Response { Succeeded = true, Message = message };
    public static Response Fail(string message) => new Response { Succeeded = false, Message = message };

}