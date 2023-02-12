namespace GqlPlayground;

public class BookService
{
    [GraphQLDescription("""
        hello
        world
        """)]
    public IEnumerable<Book> GetBooks(int count)
    {
        return Enumerable.Range(1, count)
            .Select(id => new Book { Id = id });
    }
}
