namespace TeamDesk.Api.Services
{
    public enum OperationResult
    {
        Success,     // → 204
        NotFound,    // → 404 : la demande n'existe pas
        Forbidden,   // → 403 : connecté mais pas le droit
        Invalid      // → 400 : donnée incohérente (ex : utilisateur à assigner inconnu)
    }
}
