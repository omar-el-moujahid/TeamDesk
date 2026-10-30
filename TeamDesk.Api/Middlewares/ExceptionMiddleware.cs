using Microsoft.AspNetCore.Mvc;

namespace TeamDesk.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;                 // le middleware suivant dans le pipeline
        private readonly ILogger<ExceptionMiddleware> _logger;  // pour écrire l'erreur dans les logs
        private readonly IHostEnvironment _env;                 // pour savoir si on est en Development

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)      // appelée à CHAQUE requête
        {
            try
            {
                await _next(context);                           // on laisse la requête continuer
            }
            catch (Exception ex)                                // toute exception non gérée remonte jusqu'ici
            {
                _logger.LogError(ex, "Erreur non gérée sur {Path}", context.Request.Path);

                if (context.Response.HasStarted) throw;         // réponse déjà envoyée : on ne peut plus la modifier

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var problem = new ProblemDetails                // format standard des erreurs d'API
                {
                    Status = 500,
                    Title = "Une erreur interne est survenue.",
                    Detail = _env.IsDevelopment() ? ex.Message : null   // détail visible en dev seulement
                };

                await context.Response.WriteAsJsonAsync(problem, options: null,
                    contentType: "application/problem+json");
            }
        }
    }
}