var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var budgetMatrix = new Dictionary<string, Dictionary<string, decimal>>
{
    ["Alex"] = new()
    {
        ["Marketing expenses"] = 50_000_000,
        ["Event expenses"] = 75_000_000,
        ["Office expenses"] = 30_000_000
    },

    ["Alice"] = new()
    {
        ["Marketing expenses"] = 5_000_000,
        ["Event expenses"] = 3_000_000,
        ["Office expenses"] = 10_000_000
    },

    ["Andrew"] = new()
    {
        ["Marketing expenses"] = 15_000_000,
        ["Event expenses"] = 10_000_000,
        ["Office expenses"] = 8_000_000
    },

    ["John"] = new()
    {
        ["Marketing expenses"] = 2_000_000,
        ["Event expenses"] = 1_000_000,
        ["Office expenses"] = 3_000_000
    },

    ["Kate"] = new()
    {
        ["Marketing expenses"] = 40_000_000,
        ["Event expenses"] = 25_000_000,
        ["Office expenses"] = 5_000_000
    },

    ["Tom"] = new()
    {
        ["Marketing expenses"] = 8_000_000,
        ["Event expenses"] = 5_000_000,
        ["Office expenses"] = 2_000_000
    },

    ["Supervisor"] = new()
    {
        ["Marketing expenses"] = 200_000_000,
        ["Event expenses"] = 300_000_000,
        ["Office expenses"] = 100_000_000
    }
};

app.MapPost("/api/budget/check",
    (BudgetCheckRequest request) =>
    {
        if (!budgetMatrix.ContainsKey(request.Contact))
        {
            return Results.BadRequest(new
            {
                message = $"Unknown contact: {request.Contact}"
            });
        }

        if (!budgetMatrix[request.Contact]
            .ContainsKey(request.ExpenseCategory))
        {
            return Results.BadRequest(new
            {
                message = $"Unknown expense category: {request.ExpenseCategory}"
            });
        }

        var limitBudget =
            budgetMatrix[request.Contact][request.ExpenseCategory];

        var isApproved =
            request.AmountRequested <= limitBudget;

        return Results.Ok(new BudgetCheckResponse(
            IsApproved: isApproved,
            LimitBudget: limitBudget,
            Message: isApproved
                ? "Budget available for this request."
                : "Requested amount exceeds budget limit."
        ));
    })
.WithName("CheckBudget");

var port = Environment.GetEnvironmentVariable("PORT");

if (!string.IsNullOrEmpty(port))
{
    app.Urls.Add($"http://*:{port}");
}

app.Run();

public record BudgetCheckRequest(
    string Contact,
    decimal AmountRequested,
    string ExpenseCategory
);

public record BudgetCheckResponse(
    bool IsApproved,
    decimal LimitBudget,
    string Message
);