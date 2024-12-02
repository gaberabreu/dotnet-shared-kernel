using Ardalis.Specification;

namespace SharedKernel;

public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot;
