namespace TourGuide.DAL.Exceptions;

// Thrown by UnitOfWork.SaveChangesAsync when a RowVersion check fails: someone else changed the row first.
// Lets BLL turn it into a Result without using EF Core types.
public class ConcurrencyException : Exception
{
    public ConcurrencyException(Exception inner)
        : base("The record was changed by someone else.", inner) { }
}
