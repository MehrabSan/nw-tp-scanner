namespace TPS.Core.Models;

public class TPSCoreException(string message) : Exception(message);

public class TPSExitException() : Exception();