using Flow.Core.Areas.Returns;
using Flow.Core.Demos.AppServer.Common.Seeds;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Flow.Core.Demos.AppServer.Common.ErrorHandlers
{
    public class SqliteDbExceptionHandler : IDbExceptionHandler
    {
        /*
            * The Flow class has implicit operators so you can just return a failure instead of using the static
            * Flow<T>.Failed as of the default/unmatched pattern.
            * You would just add all the types of db exceptions you want to handle appropriately.
        */ 
        public Flow<T> Handle<T>(Exception ex)

            => ex switch
            {
                SqliteException => new Failure.ConnectionFailure("Unable to connect to the sqlite database"),

                DbUpdateException uEx when (uEx.InnerException as SqliteException)?.SqliteErrorCode == 19
                    => new Failure.ConstraintFailure(@"A database constraint violation has occurred due to a possible duplicate identifier. Please check the data and try again."),

                _ => Flow<T>.Failed(new Failure.UnknownFailure("A problem has occurred, please try again", null, 0, true, ex))
            };

    }
}
