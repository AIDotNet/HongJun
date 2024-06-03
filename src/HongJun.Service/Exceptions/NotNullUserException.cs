namespace HongJun.Service.Exceptions;

public sealed class NotNullUserException(string message) : Exception(message)
{
}