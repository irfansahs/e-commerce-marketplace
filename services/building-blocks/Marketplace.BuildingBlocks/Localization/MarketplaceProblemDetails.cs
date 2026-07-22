using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Marketplace.BuildingBlocks.Localization;

public static class MarketplaceProblemDetails
{
    public static IResult Create(
        string errorCode,
        int statusCode,
        string localizedTitle,
        string? localizedDetail = null,
        string? instance = null)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = localizedTitle,
            Detail = localizedDetail,
            Type = BuildTypeUri(errorCode),
            Instance = instance
        };

        problem.Extensions["errorCode"] = errorCode;

        return Results.Json(problem, statusCode: statusCode, contentType: "application/problem+json");
    }

    public static IResult FromLocalizer(
        string errorCode,
        int statusCode,
        IStringLocalizer localizer,
        object[]? titleArgs = null,
        string? detailKey = null,
        object[]? detailArgs = null,
        string? instance = null)
    {
        var title = localizer[errorCode, titleArgs ?? Array.Empty<object>()];
        var detail = detailKey is null ? null : localizer[detailKey, detailArgs ?? Array.Empty<object>()].Value;
        return Create(errorCode, statusCode, title, detail, instance);
    }

    public static string BuildTypeUri(string errorCode) =>
        $"https://marketplace/errors/{errorCode.ToLowerInvariant().Replace('_', '-').Replace("--", "-")}";
}
