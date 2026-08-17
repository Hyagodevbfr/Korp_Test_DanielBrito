using Korp.Stock.Domain.Common;

namespace Korp.Stock.Domain.Entities;

public class Product
{
    public int Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int Balance { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private Product()
    {
        
    }

    public static Result<Product> Create(
        string code,
        string description,
        int balance)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result<Product>.Failure("O código do produto é obrigatório.");

        if (string.IsNullOrWhiteSpace(description))
            return Result<Product>.Failure("A descrição do produto é obrigatória.");

        if (balance < 0)
            return Result<Product>.Failure("O saldo do produto não pode ser negativo.");

        var product = new Product
        {
            Code = code.Trim(),
            Description = description.Trim(),
            Balance = balance,
            CreatedAt = DateTime.UtcNow
        };

        return Result<Product>.Success(product);
    }

    public Result Update(string code, string description)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure("O código do produto é obrigatório.");

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure("A descrição do produto é obrigatória.");

        Code = code.Trim();
        Description = description.Trim();
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result AddStock(int quantity)
    {
        if (quantity <= 0)
            return Result.Failure("A quantidade deve ser maior que zero.");

        Balance += quantity;

        return Result.Success();
    }

    public Result RemoveStock(int quantity)
    {
        if (quantity <= 0)
            return Result.Failure("A quantidade deve ser maior que zero.");

        if (Balance < quantity)
            return Result.Failure("Saldo insuficiente.");

        Balance -= quantity;

        return Result.Success();
    }
}
