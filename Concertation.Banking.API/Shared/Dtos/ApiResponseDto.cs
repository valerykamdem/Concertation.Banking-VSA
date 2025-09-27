namespace Concertation.Banking.API.Shared.Dtos;

public record ApiResponseDto<T>(bool Success, T? Data, string[] Errors);
