namespace Colir.Exceptions.NotEnoughPermissions;

/// <summary>
/// Thrown when the issuer of the request if not in the room he is trying to interact with
/// </summary>
public class IssuerNotInRoomException : NotEnoughPermissionsException
{
}