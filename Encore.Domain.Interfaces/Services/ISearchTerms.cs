namespace Encore.Domain.Interfaces.Services
{
    using System;
    using System.Collections.Generic;

    public interface ISearchTerms : IEnumerable<ISearchTerm> { }

    public enum OperationType { Contains, LessThan, GreaterThan }

    public interface ISearchTerm
    {
        string Property { get; }

        object Value { get; }

        Type Type { get; }

        OperationType Operation { get; }
    }
}
