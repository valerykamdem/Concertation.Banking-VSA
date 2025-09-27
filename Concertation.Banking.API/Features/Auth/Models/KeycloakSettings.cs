namespace Concertation.Banking.API.Features.Auth.Models;

public sealed class KeycloakSettings
{
    // 🔐 Informations générales
    public string BaseUrl { get; set; } = string.Empty;
    public string Realm { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    // 🧑‍💻 Identifiants applicatifs (utilisateur final)
    public string AuthClientId { get; set; } = string.Empty;
    public string AuthClientSecret { get; set; } = string.Empty;

    // 🧑‍💼 Identifiants admin (pour appeler l'API admin de Keycloak)
    public string AdminClientId { get; set; } = string.Empty;
    public string AdminClientSecret { get; set; } = string.Empty;

    // 🌐 URLs calculées (facile à modifier si besoin)
    public string TokenUrl => $"{BaseUrl}/realms/{Realm}/protocol/openid-connect/token";
    public string UserInfoUrl => $"{BaseUrl}/realms/{Realm}/protocol/openid-connect/userinfo";
    public string AdminUrl => $"{BaseUrl}/admin/realms/{Realm}";
    public string MasterTokenUrl => $"{BaseUrl}/realms/master/protocol/openid-connect/token";

    // ⚙️ Options supplémentaires (optionnelles mais utiles)
    public bool UseHttpsForRedirects { get; set; } = false;
    public TimeSpan DefaultTokenExpiry { get; set; } = TimeSpan.FromMinutes(5);
}
