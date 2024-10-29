namespace Suo.Autorization.SingleService.Infrastructure.Models;

public class ResponceException
{
    private string _exceptionType;
    public Exception Exception { get; set; }

    public string ExceptionType
    {
        get => _exceptionType;
        set => _exceptionType = value;
    }

    public bool IsDbUpdateConcurrencyException { get; set; } = false;

    public ResponceException(Exception exception)
    {
        Exception = exception;
        _exceptionType = Exception.GetType().FullName;
    }

    //public static void AggregateExceptions(ResponceException exception)
    //{

    //    switch (exception.ExceptionType)
    //    {
    //        case "System.NotImplementedException":
    //            {
    //                throw exception.Exception as NotImplementedException;
    //                break;
    //            }
    //        case "Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException":
    //            {
    //                throw exception.Exception as DbUpdateConcurrencyException;
    //                break;
    //            }
    //        case "System.NullReferenceException":
    //            {
    //                throw exception.Exception as NullReferenceException;
    //                break;

    //            }
    //        case "Microsoft.EntityFrameworkCore.DbUpdateException":
    //            {
    //                throw exception.Exception as DbUpdateException;
    //                break;
    //            }
    //        default:
    //            {
    //                throw exception.Exception;
    //                break;
    //            }
    //    }
    //}
}