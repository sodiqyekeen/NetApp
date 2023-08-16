using Moq;
using NetApp.Extensions;

namespace NetApp.Tests.Extensions;

public class CollectionExtensionsTests
{
    [Fact]
    public void ContainsAll_ReturnsTrue_WhenAllElementsArePresent()
    {
        // Arrange
        var mainCollection = new List<int> { 1, 2, 3, 4, 5 };
        var subCollection = new int[] { 1, 2 };

        // Act
        var result = mainCollection.ContainsAll(subCollection);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsAll_ReturnsTrue_WhenBothCollectionAreEmpty()
    {
        // Arrange
        var mainCollection = new List<int>();
        var subCollection = new int[0];

        // Act
        var result = mainCollection.ContainsAll(subCollection);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsAll_ReturnsFalse_WhenMainCollectionIsNull()
    {
        // Arrange
        IEnumerable<int>? mainCollection = null;
        var subCollection = new int[] { 1, 2 };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            mainCollection.ContainsAll(subCollection)
        );
    }

    [Fact]
    public void ContainsAll_ReturnsFalse_WhenSubCollectionIsNull()
    {
        // Arrange
        var mainCollection = new List<int> { 1, 2, 3, 4, 5 };
        int[]? subCollection = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            mainCollection.ContainsAll(subCollection)
        );
    }

    [Fact]
    public void ContainsAll_ReturnsFalse_WhenSubCollectionIsNotSubset()
    {
        // Arrange
        var mainCollection = new List<int> { 1, 2, 3, 4, 5 };
        var subCollection = new int[] { 1, 2, 6 };

        // Act
        var result = mainCollection.ContainsAll(subCollection);

        // Assert
        Assert.False(result);
    }

}


