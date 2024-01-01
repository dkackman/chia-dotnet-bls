namespace chia.dotnet.bls;

public class OperatorError : Exception
{
    public OperatorError(string message) : base(message)
    {
    }
}