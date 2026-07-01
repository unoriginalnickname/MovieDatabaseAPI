public interface ISpecification<T>
{
    IQueryable<T> Apply(IQueryable<T> query);
    ISpecification<T> And(ISpecification<T> other);
}