using System;

namespace DbWorker.IUnitOfWork
{
    public class SqlClient<T> where T : class
    {
        public T Client { get; set; }
        public bool IsBeginTran { get; set; }
        public Guid Id { get; set; }

        public SqlClient(T client)
        {
            Client = client;
            Id = Guid.NewGuid();
        }
    }
}
