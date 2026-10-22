using Domain.Enums;

namespace Domain.Entities;

public class Category
{
    public Guid Id { get; }
    public string Name { get; set; }
    public string RepresentativeColor { get; set; }
    public CategoryIcons? Icon { get; set; } = null;

    public Category(string categoryName, string representativeColor, CategoryIcons? icon = null)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentException("Category name cannot be empty.", nameof(categoryName));

        Id = Guid.NewGuid();
        Name = categoryName;
        RepresentativeColor = representativeColor;
        Icon = icon;
    }
}
