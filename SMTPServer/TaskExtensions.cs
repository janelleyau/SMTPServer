using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMTPServerTesting
{
    public static class TaskExtensions
    {
        public static void WaitWithoutException(this Task task)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException e)
            {
                e.Handle(exception => exception is OperationCanceledException);
            }
        }
    }
}
