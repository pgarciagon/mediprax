using MediPrax.Core.Entities;

namespace MediPrax.UnitTests;

public class BaseEntityTests
{
    private class TestEntity : BaseEntity { }

    [Fact]
    public void NewEntity_HasDefaultValues()
    {
        var entity = new TestEntity();

        Assert.NotEqual(default, entity.CreatedAt);
        Assert.NotEqual(default, entity.UpdatedAt);
        Assert.False(entity.IsDeleted);
        Assert.Null(entity.DeletedAt);
    }

    [Fact]
    public void NewEntity_HasEmptyGuid()
    {
        var entity = new TestEntity();
        Assert.Equal(Guid.Empty, entity.Id);
    }
}
