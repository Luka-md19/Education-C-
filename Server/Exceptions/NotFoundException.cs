namespace Server.API.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(String name, object key):base($"Resource of type {name} with ID {key} was not found.")
        {

        }
    }
}
